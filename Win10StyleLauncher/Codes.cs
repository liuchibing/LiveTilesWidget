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
using Android.Graphics;
using System.Net;
using System.Threading.Tasks;
using Android.Support.V7.Graphics;
using static Android.Support.V7.Graphics.Palette;
using System.Xml;

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
            //设置背景色
            //int color = preference.GetInt("AutoTileColor", 0x000000);
            //Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
            //views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);
            ////设置点击时执行的意图
            Intent intent = new Intent(context, typeof(TileSetting));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetIds[0]);//将Id传给Activity以便进行设置
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            //推送appWidget更新
            appWidgetManager.UpdateAppWidget(appWidgetIds[0], views);
        }

        /// <summary>
        /// 加载所有可被启动的应用
        /// </summary>
        /// <param name="manager">调用此方法的上下文的PackageManager属性</param>
        /// <returns>可被启动的应用的列表</returns>
        public static List<TileDetail> LoadApps(PackageManager manager)
        {
            List<TileDetail> apps = new List<TileDetail>();
            Intent intentApps = new Intent(Intent.ActionMain, null);
            intentApps.AddCategory(Intent.CategoryLauncher);
            var availableActivities = manager.QueryIntentActivities(intentApps, 0);
            foreach (var item in availableActivities)
            {
                TileDetail app = new TileDetail();
                app.Label = item.LoadLabel(manager);
                app.Name = item.ActivityInfo.PackageName;
                app.Icon = ((BitmapDrawable)item.ActivityInfo.LoadIcon(manager)).Bitmap;
                apps.Add(app);
            }
            return apps;
        }

        /// <summary>
        /// 刷新磁贴小部件
        /// </summary>
        /// <param name="id">小部件的Id或存储小部件配置的TileDetail对象，仅当传入int对象时才会正式推送小部件更新，若传入TileDetail对象则被视为请求磁贴预览，不会真的推送小部件更新</param>
        /// <param name="context">当前上下文</param>
        /// <param name="notification">要在磁贴上显示的最新通知，没有则为null</param>
        /// <param name="icon">要在磁贴上显示的通知的图标，没有则为null</param>
        public static RemoteViews UpdateTiles(object id, Context context, object notification, Bitmap icon)
        {
            //读取记录的磁贴设置
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            TileDetail tile = (id is TileDetail) ? id as TileDetail : editor.GetTileById((int)id);

            //根据不同情况创建RemoteViews对象
            RemoteViews views;
            if (notification != null) //使用通知磁贴
            {
                if (notification is string)
                {
                    if (icon != null) //使用带图标的通知磁贴
                    {
                        views = new RemoteViews(context.PackageName, Resource.Layout.IconNotifTile);
                        //设置图标和文字
                        views.SetImageViewBitmap(Resource.Id.tileNotifIcon, icon);
                        views.SetTextViewText(Resource.Id.tileNotifText, (string)notification);
                    }
                    else //使用纯文字的通知磁贴
                    {
                        views = new RemoteViews(context.PackageName, Resource.Layout.TextNotifTile);
                        //设置文字
                        views.SetTextViewText(Resource.Id.tileNotification, (string)notification);
                    }
                }
                else if (notification is Notification) //使用内嵌RemoteViews的通知磁贴
                {
                    views = new RemoteViews(context.PackageName, Resource.Layout.ViewNotifTile);
                    //清空上次残留的Notification内容
                    views.RemoveAllViews(Resource.Id.tileNotifParent);
                    //直接嵌入Notification的RemoteViews
                    views.AddView(Resource.Id.tileNotifParent, ((Notification)notification).ContentView);
                }
                else { return null; }
            }
            else //使用普通磁贴
            {
                views = new RemoteViews(context.PackageName, Resource.Layout.NormalTile);
                //设置图标
                views.SetImageViewBitmap(Resource.Id.tileIcon, tile.Icon);
            }
            if (views == null) { return null; }

            //设置应用名称
            views.SetTextViewText(Resource.Id.tileLabel, tile.Label);
            //设置背景色
            int color;
            switch (tile.TileColor)
            {
                case -1://自动
                    color = editor.AutoTileColor;
                    break;
                case -2://全局默认色
                    color = editor.DefaultTileColor;
                    break;
                default:
                    color = tile.TileColor;
                    break;
            }
            Bitmap bitmap = Bitmap.CreateBitmap(new int[] { color }, 1, 1, Bitmap.Config.Argb8888);
            views.SetImageViewBitmap(Resource.Id.tileBackground, bitmap);

            //设置点击时执行的意图
            Intent intent = context.PackageManager.GetLaunchIntentForPackage(tile.Name);
            PendingIntent pintent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            views.SetOnClickPendingIntent(Resource.Id.tileRoot, pintent);

            if (id is int)
            {
                //推送更新
                AppWidgetManager manager = AppWidgetManager.GetInstance(context);
                manager.UpdateAppWidget((int)id, views);
            }

            return views;
        }

        /// <summary>
        /// 获取今天的必应首页图片
        /// </summary>
        /// <returns></returns>
        public static async Task<Bitmap> GetBingImage()
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] buffer = await wc.DownloadDataTaskAsync(new Uri(@"http://area.sinaapp.com/bingImg"));
                Bitmap img = await BitmapFactory.DecodeByteArrayAsync(buffer, 0, buffer.Length);
                return img;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取Palette中出现最多的颜色
        /// </summary>
        /// <param name="swatches">要分析的palette的swatches属性</param>
        /// <returns></returns>
        public static int GetMainColor(IList<Swatch> swatches)
        {
            int max = -1, color = -1;
            if (swatches[0] != null)
            {
                max = swatches[0].Population;
                color = swatches[0].Rgb;
            }
            foreach (var item in swatches)
            {
                if (item != null)
                {
                    if (item.Population > max)
                    {
                        max = item.Population;
                        color = item.Rgb;
                    }
                }
            }
            return color;
        }

        /// <summary>
        /// 安排定时启动Rss自动更新服务
        /// </summary>
        /// <param name="context"></param>
        public static void ArrangeRssUpdate(Context context)
        {
            Intent i = new Intent(context, typeof(AutoUpdateTileService));
            i.SetAction("com.LiveTilesWidget.UpdateRss");
            PendingIntent pi = PendingIntent.GetService(context, 0, i, PendingIntentFlags.CancelCurrent);
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            am.SetRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime(), 30 * 60 * 1000, pi);
        }

        /// <summary>
        /// 获取SharedPreferences
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ISharedPreferences GetPreferences(Context context)
        {
            return context.GetSharedPreferences(Application.Context.PackageName + ".tiles", FileCreationMode.Private);
        }

        /// <summary>
        /// 迁移旧版preferences数据至新版
        /// </summary>
        /// <param name="originVersion">原来的版本号</param>
        /// <param name="context"></param>
        public static void MovePreferences(int originVersion, Context context)
        {
            switch (originVersion)
            {
                case 0:
                    var old = context.GetSharedPreferences("tiles", FileCreationMode.Private);
                    var pref = GetPreferences(context);
                    var editor = pref.Edit();
                    editor.Clear();
                    foreach (var item in old.All)
                    {
                        if (item.Value is bool)
                        {
                            editor.PutBoolean(item.Key, (bool)item.Value);
                        }
                        else if (item.Value is float)
                        {
                            editor.PutFloat(item.Key, (float)item.Value);
                        }
                        else if (item.Value is int)
                        {
                            editor.PutInt(item.Key, (int)item.Value);
                        }
                        else if (item.Value is long)
                        {
                            editor.PutLong(item.Key, (long)item.Value);
                        }
                        else if (item.Value is string)
                        {
                            editor.PutString(item.Key, (string)item.Value);
                        }
                        else if (item.Value is ICollection<string>)
                        {
                            editor.PutStringSet(item.Key, (ICollection<string>)item.Value);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 从指定rss url读取其中第一项的标题和图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="text"></param>
        /// <param name="img"></param>
        public static void ReadRss(string url, out string text, out Bitmap img)
        {
            //http://api.ithome.com/xml/newslist/news.xml
            text = null;
            string imgUrl = null;
            //开始rss解析
            try
            {
                using (XmlTextReader reader = new XmlTextReader(url))
                {
                    bool read = false;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "entry":
                                case "item":
                                    read = true;
                                    continue;
                                case "title":
                                    if (read == true)
                                    {
                                        text = reader.ReadElementContentAsString();
                                    }
                                    break;
                                //case "summary":
                                //case "description":
                                //    if (read == true)
                                //    {
                                //        text += "\n" + reader.ReadElementContentAsString().Trim();
                                //    }
                                //    break;
                                case "image":
                                    if (read == true)
                                    {
                                        imgUrl = reader.ReadElementContentAsString();
                                    }
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            if (reader.Name == "item" || reader.Name == "entry")
                            {
                                read = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            img = null;
            if (imgUrl != null)
            {
                try
                {
                    WebClient wc = new WebClient();
                    byte[] buffer = wc.DownloadData(imgUrl);
                    img = BitmapFactory.DecodeByteArray(buffer, 0, buffer.Length);
                }
                catch { }
            }
        }
    }
}