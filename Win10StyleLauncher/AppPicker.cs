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
    /// 选择磁贴所指向的应用
    /// </summary>
    [Activity(Label = "选择一个应用")]
    public class AppPicker : ListActivity
    {
        protected List<AppDetail> apps;
        private int id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //加载应用列表
            apps = Codes.LoadApps(PackageManager);
            //显示应用
            ListAdapter = new AppListAdapter(this, Resource.Layout.AppPickerItems, apps.ToArray());

            //从Extra中获取要进行自定义设置的AppWidgetId
            id = Intent.GetIntExtra("id", -1);
            if (id == -1)
            {
                Finish();
            }
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //将所选的应用信息保存到SharedPreferences中以供保存
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();
            editor.PutString(id + "Label", apps[position].Label);
            editor.PutString(id + "Name", apps[position].Name);
            editor.Commit();

            //返回应用的Label
            Intent i = new Intent();
            i.PutExtra("Label", apps[position].Label);
            SetResult(Result.Ok, i);
            Finish();
            //Intent intent = PackageManager.GetLaunchIntentForPackage(apps[position].Name);
            //StartActivity(intent);
        }
    }
}