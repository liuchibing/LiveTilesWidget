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
using Android.Appwidget;

namespace LiveTilesWidget
{
    [Activity(Label = "设置磁贴小部件", Name = "com.LiveTilesWidget.TileSetting", Exported = true)]
    public class TileSetting : Activity
    {
        int count = 1;
        //标记此次运行是否是初始化过程(Configuration Activity)
        private bool isInitialize = false;
        //当前正在设置的磁贴的实例对象
        private AppDetail tile;
        private TilesPreferenceEditor editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //从Extra中获取要进行自定义设置的AppWidgetId
            var id = Intent.GetIntExtra("id", -1);
            if (id == -1)
            {
                //若没有传入id信息则尝试以初始化过程的方式取得id，否则退出
                id = Intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, -1);
                isInitialize = true;
                if (id == -1)
                {
                    Finish();
                }
            }
            //防止意外退出
            Intent i = new Intent();
            i.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);
            SetResult(Result.Canceled, i);

            SetContentView(Resource.Layout.TileSettings);

            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            CheckBox checkAutoColor = FindViewById<CheckBox>(Resource.Id.checkAutoColor);
            Button btnRefresh = FindViewById<Button>(Resource.Id.btnRefresh);

            //获取存储的磁贴信息
            editor = new TilesPreferenceEditor(this);

            if (isInitialize)
            {
                tile = new AppDetail();
                tile.ShowNotification = true;
                checkShowNotif.Checked = true;
                tile.AutoTileColor = true;
                checkAutoColor.Checked = true;
                btnRefresh.Enabled = false;
            }
            else
            {
                tile = editor.GetTileById(id);
                if (tile == null)
                {
                    Finish();
                }
                try
                {
                    //设置选择应用按钮的文字为存储记录中此磁贴当前指向的应用
                    btnChooseApp.Text = tile.Label;
                    //设置是否允许显示通知选择框的状态为存储记录中此磁贴当前是否允许显示通知的状态
                    checkShowNotif.Checked = tile.ShowNotification;
                    checkAutoColor.Checked = tile.AutoTileColor;
                }
                catch { }
            }

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
                tile.ShowNotification = checkShowNotif.Checked;
            };
            checkAutoColor.CheckedChange += (sender, e) =>
            {
                tile.AutoTileColor = checkAutoColor.Checked;
            };

            //点击按钮时保存更改并立即刷新磁贴
            btnRefresh.Click += (sender, e) =>
             {
                 if (isInitialize)
                 {
                     tile.Id = id;
                     editor.Tiles.Add(tile);
                 }
                 editor.CommitChanges();
                 Codes.UpdateTiles(id, this, null);
                 Intent result = new Intent();
                 i.PutExtra(AppWidgetManager.ExtraAppwidgetId, id);
                 SetResult(Result.Ok, result);
                 Finish();
             };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 0://应用选择界面的请求码
                    string label = data.GetStringExtra("Label");
                    FindViewById<Button>(Resource.Id.btnChooseApp).Text = label ?? "设置应用";
                    tile.Label = label;
                    tile.Name = data.GetStringExtra("Name");
                    FindViewById<Button>(Resource.Id.btnRefresh).Enabled = (label != null);
                    break;
                case 1://颜色选择界面的请求码
                    break;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (!isInitialize)
            {
                var option = menu.Add(0, 0, 0, "Debug Delete");
                option.SetShowAsAction(ShowAsAction.CollapseActionView);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0://Debug Delete
                    AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    dialog.SetIcon(Resource.Drawable.Icon);
                    dialog.SetTitle("确定重置？");
                    dialog.SetMessage("此功能仅当在开发过程中使用，请慎重。");
                    dialog.SetCancelable(true);
                    dialog.SetPositiveButton("确定", (sender, e) =>
                    {
                        //清除当前磁贴的数据
                        if (!isInitialize)
                        {
                            editor.Tiles.Remove(tile);
                            editor.CommitChanges();
                        }
                        Finish();
                    });
                    dialog.SetNegativeButton("取消", (sender, e) => { });
                    dialog.Show();
                    break;
            }

            return base.OnMenuItemSelected(featureId, item);
        }
    }
}