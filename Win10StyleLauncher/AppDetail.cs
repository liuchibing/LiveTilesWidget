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
using Android.Graphics.Drawables;

namespace LiveTilesWidget
{
    /// <summary>
    /// 一个可以被动态磁贴小部件启动的应用的信息,或一个动态磁贴的配置信息
    /// </summary>
    public class AppDetail
    {
        /// <summary>
        /// 显示的标签名称
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// 程序包名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 图标
        /// </summary>
        public Drawable Icon
        {
            get;
            set;
        }

        /// <summary>
        /// 动态磁贴小部件的Id
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 是否在磁贴上显示最新通知
        /// </summary>
        public bool ShowNotification
        {
            get;
            set;
        }

        /// <summary>
        /// 是否从壁纸中自动获取适合的背景色
        /// </summary>
        public bool AutoTileColor
        {
            get;
            set;
        }

        /// <summary>
        /// 加载当前对象所代表的应用的图标到Icon属性
        /// </summary>
        /// <param name="context"></param>
        public void LoadIcon(Context context)
        {
            AppDetail app = null;
            foreach (var item in Codes.LoadApps(context.PackageManager))
            {
                if (item.Name == this.Name && item.Label == this.Label)
                {
                    app = item;
                    break;
                }
            }
            if (app != null)
            {
                this.Icon = app.Icon;
            }
        }
    }
}