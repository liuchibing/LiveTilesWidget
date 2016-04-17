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
        }

        public override void OnNotificationPosted(StatusBarNotification sbn)
        {
            //base.OnNotificationPosted(sbn);

            //判断收到的通知是否属于已固定磁贴的应用
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string> { }));
            foreach (var item in ids)
            {
                //是否属于已开启显示通知功能的磁贴的应用
                if (preference.GetBoolean(item + "ShowNotif", true) && sbn.PackageName == preference.GetString(item + "Name", ""))
                {
                    //通过枚举来获得通知内容
                    //View v = sbn.Notification.ContentView.Apply(this, new LinearLayout(this));
                    string text = "";
                    text = sbn.Notification.Extras.GetString(Notification.ExtraTitle, "错误") + '\n' + sbn.Notification.Extras.GetString(Notification.ExtraText, "错误");
                    //EnumGroupViews(v, ref text);

                    //推送动态磁贴小部件更新
                    Codes.UpdateTiles(Convert.ToInt32(item), this, text);
                }
            }
        }

        public override void OnNotificationRemoved(StatusBarNotification sbn)
        {
            //base.OnNotificationRemoved(sbn);

            //判断收到的通知是否属于已固定磁贴的应用
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            List<string> ids = new List<string>(preference.GetStringSet("Ids", new List<string> { }));
            foreach (var item in ids)
            {
                //是否属于已开启显示通知功能的磁贴的应用
                if (preference.GetBoolean(item + "ShowNotif", true) && sbn.PackageName == preference.GetString(item + "Name", ""))
                {
                    //推送小部件更新
                    Codes.UpdateTiles(Convert.ToInt32(item), this, null);
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