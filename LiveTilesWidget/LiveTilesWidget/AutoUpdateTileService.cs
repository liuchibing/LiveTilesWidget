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
using System.Xml;
using Android.Graphics;
using System.Net;
using Android.Util;

namespace LiveTilesWidget
{
    /// <summary>
    /// 被定时调起进行更新动态磁贴上的rss内容的服务
    /// </summary>
    [Service(Label = "动态磁贴10磁贴自动刷新服务")]
    [IntentFilter(new string[] { "com.LiveTilesWidget.UpdateRss" })]
    public class AutoUpdateTileService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            Log.Debug("AutoUpdateTileService", "Running");
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                switch (item.TileType)
                {
                    case LiveTileType.Rss:
                        string text;
                        Bitmap img;
                        Codes.ReadRss(item.RssUrl, out text, out img);
                        //推送小部件更新
                        Codes.UpdateTiles(item.Id, this, text, img);
                        break;
                }
            }
        }

    }
}