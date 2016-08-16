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
using Android.Service.Notification;
using Android.Util;

namespace LiveTilesWidget
{
    [Service(Label = "动态磁贴10通知探测服务", Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE")]
    [IntentFilter(new string[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationService : NotificationListenerService
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug("notif", "creating");
            //因为此服务会随系统启动，因此可以在此处:
            //刷新所有磁贴
            var editor = new TilesPreferenceEditor(this);
            foreach(var item in editor.Tiles)
            {
                Codes.UpdateTiles(item.Id, this, null, null);
            }
            //安排自动更新Rss，省去了监听系统启动完成的麻烦
            Codes.ArrangeRssUpdate(this);
        }

        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            //base.OnNotificationPosted(sbn);

            //判断收到的通知是否属于已固定磁贴的应用
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                //是否属于已开启显示通知功能的磁贴的应用
                if (item.ShowNotification && sbn.PackageName == item.Name)
                {
                    //判断收到的通知是否是可解析的标准格式
                    if (sbn.Notification.Extras.GetString(Notification.ExtraTitle) != null)
                    {
                        string text = sbn.Notification.Extras.GetString(Notification.ExtraTitle, "错误") + '\n' + sbn.Notification.Extras.GetString(Notification.ExtraText, "错误");
                        //推送动态磁贴小部件更新
                        if (item.ShowNotifIcon) //是否允许显示图标
                        {
                            Codes.UpdateTiles(item.Id, this, text, sbn.Notification.LargeIcon);
                        }
                        else
                        {
                            Codes.UpdateTiles(item.Id, this, text, null);
                        }
                    }
                    else
                    {
                        //否则直接照搬通知内容,推送小部件更新
                        Codes.UpdateTiles(item.Id, this, sbn.Notification, null);
                    }
                }
            }
        }

        public override void OnNotificationRemoved(StatusBarNotification sbn)
        {
            //base.OnNotificationRemoved(sbn);

            //判断收到的通知是否属于已固定磁贴的应用
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this);
            foreach (var item in editor.Tiles)
            {
                //是否属于已开启显示通知功能的磁贴的应用
                if (item.ShowNotification && sbn.PackageName == item.Name)
                {
                    //推送小部件更新
                    Codes.UpdateTiles(item.Id, this, null, null);
                }
            }
        }

        /// <summary>
        /// 枚举GroupView中的成员，找出其中的TextView
        /// </summary>
        /// <param name="v">要进行枚举的View</param>
        /// <param name="text">被找出的TextView的文本会被追加到此字符串中</param>
        private void EnumGroupViews(View v, ref string text)
        {
            if (v is ViewGroup)
            {
                ViewGroup vg = (ViewGroup)v;
                for (int i = 0; i < vg.ChildCount; i++)
                {
                    View child = vg.GetChildAt(i);
                    if (child is ViewGroup)
                    {
                        //递归处理ViewGroup
                        EnumGroupViews(child, ref text);
                    }
                    else if (child is TextView)
                    {
                        //则解析里面的内容，并追加到text中
                        string str = ((TextView)child).Text;
                        if (str.Length > 0)
                        {
                            text += str + '\n';
                        }
                    }
                }
            }
        }
    }
}