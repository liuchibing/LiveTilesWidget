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
    [BroadcastReceiver(Label = "动态磁贴")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/normal_tile")]
    public class NormalTileProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            //读取磁贴信息的存储
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            //仅当磁贴ID未记录在存储中时才进行初始化
            if (editor.GetTileById(appWidgetIds[0]) == null)
            {
                Codes.InitializeTile(context, appWidgetManager, appWidgetIds);
            }
            else
            {
                Codes.UpdateTiles(appWidgetIds[0], context, null);
            }
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);

            //删除小部件时移除记录
            TilesPreferenceEditor editor = new TilesPreferenceEditor(context);
            if (editor.GetTileById(appWidgetIds[0]) != null)
            {
                editor.Tiles.Remove(editor.GetTileById(appWidgetIds[0]));
                editor.CommitChanges();
            }
        }
    }
}