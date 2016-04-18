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

namespace LiveTilesWidget
{
    [Activity(Label = "动态磁贴10", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

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
                
                var notify = new Notification.Builder(this);
                notify.SetContentTitle("hello!");
                notify.SetContentText("world!");
                notify.SetTicker((Servicerunning) ? "test" : "error");
                notify.SetContentInfo("test info");
                notify.SetDefaults(NotificationDefaults.Sound);
                notify.SetSmallIcon(Resource.Drawable.Icon);
                ((NotificationManager)GetSystemService(NotificationService)).Notify(1, notify.Build());
            };

            //启动通知监视服务
            //StartService(new Intent(this, typeof(NotificationService)));
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
        private void LoadTiles()
        {
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();

            ListView listTiles = FindViewById<ListView>(Resource.Id.listTiles);

            //加载所有记录的磁贴
            List<AppDetail> apps = Codes.LoadApps(PackageManager);
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string> { }));
            List<AppDetail> tiles = new List<AppDetail>();
            foreach (var id in ids)
            {
                AppDetail app = new AppDetail();
                app.Label = preference.GetString(id + "Label", "未设置");
                app.Icon = ApplicationInfo.LoadIcon(PackageManager);//设置默认图标
                foreach (var item in apps)//尝试寻找正确的图标
                {
                    if (item.Name == preference.GetString(id + "Name", null))
                    {
                        app.Icon = item.Icon;
                        break;
                    }
                }
                app.Name = id;
                tiles.Add(app);
            }
            listTiles.Adapter = new AppListAdapter(this, Resource.Layout.AppPickerItems, tiles.ToArray());
            listTiles.ItemClick += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(TileSetting));
                intent.PutExtra("id", Convert.ToInt32(tiles[e.Position].Name));
                StartActivity(intent);
            };
        }
    }
}

