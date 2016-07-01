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
using Android.Graphics;
using System.Text.RegularExpressions;

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
        public byte[] Icon
        {
            get;
            set;
        }
        //private Bitmap icon;

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
        /// 加载当前对象所代表的应用的图标
        /// </summary>
        /// <param name="context"></param>
        public Bitmap LoadIcon(Context context)
        {
            return ((BitmapDrawable)context.PackageManager.GetActivityIcon(context.PackageManager.GetLaunchIntentForPackage(Name))).Bitmap;
        }

        /// <summary>
        /// 获取用于在ListView中排序检索的（拼音）首字母
        /// </summary>
        /// <returns></returns>
        public string GetSortLetters()
        {
            // 正则表达式，判断首字母是否是英文字母  
            Regex reg = new Regex("[A-Z]");
            if (reg.IsMatch(Label.Substring(0, 1).ToUpper()))
            {
                return Label.Substring(0, 1).ToUpper();
            }
            //汉字转换成拼音
            string pinyin = CharacterParser.StrConvertToPinyin(Label.Substring(0, 1));
            string sortString = pinyin.Substring(0, 1).ToUpper();

            // 正则表达式，判断首字母是否是英文字母  
            if (reg.IsMatch(sortString))
            {
                return sortString;
            }
            else
            {
                return "#";
            }

        }
    }
}