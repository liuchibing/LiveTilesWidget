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
                dialog.SetMessage("请在弹出的界面中打开\"动态磁贴10通知探测服务\"的开关，并点击弹出的安全提示中的确认按钮");
                dialog.SetPositiveButton("前往设置", this);
                dialog.Show();
            }

            //清除所有preference数据
            FindViewById<Button>(Resource.Id.btnReset).Click += (sender, e) =>
            {
                var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
                var editor = preference.Edit();
                editor.Clear();
                editor.Commit();
            };

            //发送通知测试
            FindViewById<Button>(Resource.Id.btnNotif).Click += (sender, e) =>
            {


                var notify = new Notification.Builder(this);
                notify.SetContentTitle((Servicerunning) ? "Hello!" : "error");
                notify.SetContentText("world!");
                notify.SetTicker((Servicerunning) ? "test" : "error");
                notify.SetContentInfo("test info");
                notify.SetDefaults(NotificationDefaults.Sound);
                notify.SetSmallIcon(Resource.Drawable.Icon);
                ((NotificationManager)GetSystemService(NotificationService)).Notify(1, notify.Build());
            };

            //将壁纸设置为今天的必应首页图片
            FindViewById<Button>(Resource.Id.btnSetWallpaper).Click += (sender, e) =>
             {
                 StartService(new Intent(this, typeof(SetWallpaper)));
             };

            Log.Debug("main", "created");
        }

        protected override void OnResume()
        {
            base.OnResume();

            //在任何重新进入应用的情况下重新加载磁贴列表
            LoadTiles();
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
            TilesPreferenceEditor editor = null;
            await Task.Run(() =>
            {
                //加载所有记录的磁贴
                editor = new TilesPreferenceEditor(this);
                //加载图标
                foreach (var item in editor.Tiles)
                {
                    item.LoadIcon(this);
                }
            });

            listTiles.Adapter = new AppListAdapter(this, Resource.Layout.AppPickerItems, editor.Tiles.ToArray());
            listTiles.ItemClick += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(TileSetting));
                intent.PutExtra("id", editor.Tiles[e.Position].Id);
                StartActivity(intent);
            };
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
    }
}

