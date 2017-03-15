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

namespace LiveTilesWidget
{
    /// <summary>
    /// 监听系统启动完成
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionBootCompleted })]
    class BootListener : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //刷新所有磁贴
            var editor = new TilesPreferenceEditor(context);
            foreach (var item in editor.Tiles)
            {
                Codes.UpdateTiles(item.Id, context, null, null);
            }
            //在此处安排自动更新Rss
            Codes.ArrangeRssUpdate(context);
        }
    }
}