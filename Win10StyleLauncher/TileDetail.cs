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
using System.ComponentModel;

namespace LiveTilesWidget
{
    /// <summary>
    /// 一个可以被动态磁贴小部件启动的应用的信息,或一个动态磁贴的配置信息
    /// </summary>
    public class TileDetail : INotifyPropertyChanged
    {
        //Fields
        private string _label;
        private string _name;
        private bool _showNotification;
        private int _tileColor;
        private bool _showNotifIcon;
        private LiveTileType _tileType;
        private string _rssUrl;

        //Properties
        /// <summary>
        /// 显示的标签名称
        /// </summary>
        public string Label
        {
            get
            {
                return _label;
            }

            set
            {
                _label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            }
        }

        /// <summary>
        /// 程序包名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public Bitmap Icon
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
            get
            {
                return _showNotification;
            }

            set
            {
                _showNotification = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNotification"));
            }
        }

        /// <summary>
        /// 磁贴的背景色，-1为自动，-2为全局自定义色
        /// </summary>
        public int TileColor
        {
            get
            {
                return _tileColor;
            }

            set
            {
                _tileColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileColor"));
            }
        }

        /// <summary>
        /// 是否在有通知时显示通知的图标
        /// </summary>
        public bool ShowNotifIcon
        {
            get
            {
                return _showNotifIcon;
            }

            set
            {
                _showNotifIcon = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowNotifIcon"));
            }
        }

        /// <summary>
        /// 当前动态磁贴动态内容的类型
        /// </summary>
        public LiveTileType TileType
        {
            get
            {
                return _tileType;
            }

            set
            {
                _tileType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TileType"));
            }
        }

        /// <summary>
        /// 用于更新动态磁贴动态内容的Rss Url
        /// </summary>
        public string RssUrl
        {
            get
            {
                return _rssUrl;
            }

            set
            {
                _rssUrl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RssUrl"));
            }
        }

        //Events
        public event PropertyChangedEventHandler PropertyChanged;

        //Methods
        /// <summary>
        /// 加载当前对象所代表的应用的图标到Icon属性
        /// </summary>
        /// <param name="context"></param>
        public void LoadIcon(Context context)
        {
            Icon = ((BitmapDrawable)context.PackageManager.GetActivityIcon(context.PackageManager.GetLaunchIntentForPackage(Name))).Bitmap;
        }

        /// <summary>
        /// 获取用于在ListView中排序检索的（拼音）首字母
        /// </summary>
        /// <returns></returns>
        public string GetSortLetters()
        {
            // 正则表达式，判断首字母是否是英文字母  
            Regex reg = new Regex("[A-Z]");

            //汉字转换成拼音
            string pinyin = CharacterParser.GetCharSpellCode(Label.Substring(0, 1));

            // 正则表达式，判断首字母是否是英文字母  
            if (reg.IsMatch(pinyin))
            {
                return pinyin;
            }
            else
            {
                return "#";
            }
        }
    }

    /// <summary>
    /// 动态磁贴动态内容的类型
    /// </summary>
    public enum LiveTileType
    {
        /// <summary>
        /// 仅按照磁贴设置，在有通知时显示通知内容，无其他动态内容
        /// </summary>
        None,
        /// <summary>
        /// 从指定RSS源更新动态内容
        /// </summary>
        Rss
    }
}