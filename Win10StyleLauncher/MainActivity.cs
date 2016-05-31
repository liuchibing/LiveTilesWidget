using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Service.Notification;
using Android.Graphics;
using System.Threading;
using Android.Support.V7.Graphics;
using System.Threading.Tasks;

namespace LiveTilesWidget
{
    [Activity(Label = "动态磁贴10", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IDialogInterfaceOnClickListener
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //获取正在运行的服务，检查NotificationService是否正在运行
            bool Servicerunning = false;
            ActivityManager manager = (ActivityManager)GetSystemService(ActivityService);
            foreach (var item in manager.GetRunningServices(int.MaxValue))
            {
                Log.Debug("RunningService", item.Service.PackageName);
                if (item.Service.PackageName == PackageName)
                {
                    Servicerunning = true;
                    break;
                }
            }
            if (!Servicerunning)
            {
                AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                dialog.SetIcon(Resource.Drawable.Icon);
                dialog.SetTitle("通知探测服务未运行");
                dialog.SetMessage("请在弹出的界面中打开\"动态磁贴10通知探测服务\"的开关，并点击弹出的安全提示中的允许按钮");
                dialog.SetPositiveButton("前往设置", this);
                dialog.Show();
            }

            Log.Debug("main", "created");
        }

        protected override void OnResume()
        {
            base.OnResume();

            //在任何重新进入应用的情况下重新加载磁贴列表
            LoadTiles();
            //for (int i = 0; i < 20; i++)
            //{
            //    LoadTiles();
            //}
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            //在任何重新进入应用的情况下重新加载磁贴列表
            LoadTiles();
        }

        /// <summary>
        /// 把所有记录的磁贴加载到列表中
        /// </summary>
        private async void LoadTiles()
        {
            ListView listTiles = FindViewById<ListView>(Resource.Id.listTiles);
            listTiles.Visibility = ViewStates.Invisible;
            TilesPreferenceEditor editor = null;
            await Task.Run(() =>
            {
                //加载所有记录的磁贴
                editor = new TilesPreferenceEditor(this);
                ////加载图标
                //foreach (var item in editor.Tiles)
                //{
                //    item.LoadIcon(this);
                //}
            });
            
            if (editor != null && editor.Tiles.Count != 0)
            {
                listTiles.Adapter = new AppListAdapter(this, Resource.Layout.AppPickerItems, editor.Tiles.ToArray());
                listTiles.ItemClick += (sender, e) =>
                {
                    Intent intent = new Intent(this, typeof(TileSetting));
                    intent.PutExtra("id", editor.Tiles[e.Position].Id);
                    StartActivity(intent);
                };
                listTiles.Visibility = ViewStates.Visible;
            }
        }

        /// <summary>
        /// 点击对话框中的前往按钮时打开NotificationListenerService的系统设置界面
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="which"></param>
        public void OnClick(IDialogInterface dialog, int which)
        {
            StartActivity(new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS"));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var option1 = menu.Add(0, 0, 0, "设置壁纸为必应美图");
            option1.SetShowAsAction(ShowAsAction.CollapseActionView);
            var option2 = menu.Add(0, 1, 1, "发送通知测试");
            option2.SetShowAsAction(ShowAsAction.CollapseActionView);
            var option3 = menu.Add(0, 2, 2, "Debug Reset");
            option3.SetShowAsAction(ShowAsAction.CollapseActionView);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0://设置壁纸为必应美图
                    StartService(new Intent(this, typeof(SetWallpaper)));
                    break;
                case 1://发送通知测试
                    var notify = new Notification.Builder(this);
                    notify.SetContentTitle("Hello!");
                    notify.SetContentText("world!");
                    notify.SetTicker("test");
                    notify.SetContentInfo("test info");
                    notify.SetDefaults(NotificationDefaults.Sound);
                    notify.SetSmallIcon(Resource.Drawable.Icon);
                    ((NotificationManager)GetSystemService(NotificationService)).Notify(1, notify.Build());
                    break;
                case 2://Debug Reset
                    AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    dialog.SetIcon(Resource.Drawable.Icon);
                    dialog.SetTitle("确定重置？");
                    dialog.SetMessage("此功能仅当在开发过程中使用，请慎重。");
                    dialog.SetCancelable(true);
                    dialog.SetPositiveButton("确定", (sender, e) =>
                    {
                        //清除所有preference数据
                        var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
                        var editor = preference.Edit();
                        editor.Clear();
                        editor.Commit();
                        LoadTiles();
                    });
                    dialog.SetNegativeButton("取消", (sender, e) => { });
                    dialog.Show();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}

