using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Appwidget;
using Android.Content.PM;
using Android.Graphics.Drawables;

namespace LiveTilesWidget
{
    public static class Codes
    {
        /// <summary>
        /// 首次创建一个动态磁贴时进行的初始化，所需参数为照抄AppWidgetProvider.OnUpdate()的所有参数即可。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appWidgetManager"></param>
        /// <param name="appWidgetIds"></param>
        public static void InitializeTile(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            //创建RemoteViews对象，并设置初始化值
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //设置内容
            views.SetTextViewText(Resource.Id.tileLabel, "设置此磁贴" + appWidgetIds[0]);
            views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            //设置点击时执行的意图
            Intent intent = new Intent(context, typeof(TileSetting));
            intent.PutExtra("id", appWidgetIds[0]);//将Id传给Activity以便进行设置
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //将新的磁贴的ID添加到SharedPreference中
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();
            editor.PutString("Ids", preference.GetString("Ids", "") + appWidgetIds[0] + ",");

            //推送appWidget更新
            appWidgetManager.UpdateAppWidget(appWidgetIds[0], views);
        }

        /// <summary>
        /// 加载所有可被启动的应用
        /// </summary>
        /// <param name="manager">调用此方法的上下文的PackageManager属性</param>
        /// <returns>可被启动的应用的列表</returns>
        public static List<AppDetail> LoadApps(PackageManager manager)
        {
            List<AppDetail> apps = new List<AppDetail>();
            Intent intentApps = new Intent(Intent.ActionMain, null);
            intentApps.AddCategory(Intent.CategoryLauncher);
            var availableActivities = manager.QueryIntentActivities(intentApps, 0);
            foreach (var item in availableActivities)
            {
                AppDetail app = new AppDetail();
                app.Label = item.LoadLabel(manager);
                app.Name = item.ActivityInfo.PackageName;
                app.Icon = item.ActivityInfo.LoadIcon(manager);
                apps.Add(app);
            }
            return apps;
        }

        /// <summary>
        /// 刷新磁贴小部件
        /// </summary>
        /// <param name="Id">小部件的Id</param>
        /// <param name="context">当前上下文</param>
        /// <param name="Notification">要在磁贴上显示的最新通知，没有则为null</param>
        public static void UpdateTiles(int Id, Context context, string Notification)
        {
            //读取记录的磁贴设置
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);

            //创建RemoteViews对象
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
            //设置应用名称
            views.SetTextViewText(Resource.Id.tileLabel, preference.GetString(Id + "Label", "错误"));
            //设置图标
            AppDetail app = null;
            foreach (var item in LoadApps(context.PackageManager))
            {
                if (item.Name == preference.GetString(Id + "Name", null))
                {
                    app = item;
                    break;
                }
            }
            if (app != null)
            {
                views.SetImageViewBitmap(Resource.Id.tileIcon, ((BitmapDrawable)app.Icon).Bitmap);
            }
            //设置通知内容
            if (Notification == null)
            {
                views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Invisible);
            }
            else
            {
                views.SetViewVisibility(Resource.Id.tileNotification, ViewStates.Visible);
                views.SetTextViewText(Resource.Id.tileNotification, Notification);
            }
            //设置点击时执行的意图
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(app.Name);
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //推送更新
            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            manager.UpdateAppWidget(Id, views);
        }
    }
}