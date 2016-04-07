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

namespace LiveTilesWidget
{
    [BroadcastReceiver(Label = "Wide Live Tile")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/wide_tile")]
    public class WideTileProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            //读取磁贴信息的存储
            ContextWrapper contextWrapper = new ContextWrapper(context);
            var preference = contextWrapper.GetSharedPreferences("tiles", FileCreationMode.Private);
            //仅当磁贴ID未记录在存储中时才进行初始化
            if (!preference.Contains(appWidgetIds[0].ToString()+"Label"))
            {
                Codes.InitializeTile(context, appWidgetManager, appWidgetIds);
            }
        }
    }
}