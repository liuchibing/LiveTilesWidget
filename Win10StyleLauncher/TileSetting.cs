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
    [Activity(Label = "设置磁贴小部件")]
    public class TileSetting : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TileSettings);

            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            Button btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);

            //从Extra中获取要进行自定义设置的AppWidgetId
            var id = Intent.GetIntExtra("id", -1);
            if (id == -1)
            {
                Finish();//若没有传入id信息则退出
            }

            //获取存储的磁贴信息
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();

            //设置选择应用按钮的文字为存储记录中此磁贴当前指向的应用
            btnChooseApp.Text = preference.GetString(id.ToString() + "Label", "设置应用");
            //设置是否允许显示通知选择框的状态为存储记录中此磁贴当前是否允许显示通知的状态
            checkShowNotif.Checked = preference.GetBoolean(id.ToString() + "ShowNotif", true);

            //点击按钮时跳转到选择应用的Activity
            btnChooseApp.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AppPicker));
                intent.PutExtra("id", id);//把要设置的磁贴的Id传给Activity
                StartActivityForResult(intent, 0);
            };

            //勾选状态更改时保存设置
            checkShowNotif.CheckedChange += (sender, e) =>
            {
                editor.PutBoolean(id + "ShowNotif", checkShowNotif.Checked);
                editor.Commit();
            };

            //点击按钮时立即刷新磁贴
            btnRefresh.Click += (sender, e) =>
            {
                Codes.UpdateTiles(id, this, null);
                Finish();
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 0://应用选择界面的请求码
                    FindViewById<Button>(Resource.Id.btnChooseApp).Text = data.GetStringExtra("Label") ?? "设置应用";
                    break;
                case 1://颜色选择界面的请求码
                    break;
            }
        }
    }
}