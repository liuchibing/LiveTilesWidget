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
    [Service]
    [IntentFilter(new string[] { "com.LiveTilesWidget.UpdateRss" })]
    public class ReadRss : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            Log.Debug("ReadRss", "Running");
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                if (item.TileType == LiveTileType.Rss)
                {
                    //http://api.ithome.com/xml/newslist/news.xml
                    string text = null;
                    string imgUrl = null;
                    //开始rss解析
                    try
                    {
                        using (XmlTextReader reader = new XmlTextReader(item.RssUrl))
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
                    Bitmap img = null;
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
                    //推送小部件更新
                    Codes.UpdateTiles(item.Id, this, text, img);
                }
            }
        }
    }
}