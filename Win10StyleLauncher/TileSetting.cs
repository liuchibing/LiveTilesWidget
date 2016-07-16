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
using Android.Graphics;
using Android.Util;

namespace LiveTilesWidget
{
    [Activity(Label = "设置磁贴小部件", Name = "com.LiveTilesWidget.TileSetting", Exported = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTask, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
    public class TileSetting : Activity
    {
        int count = 1;
        //标记此次运行是否是初始化过程(Configuration Activity)
        private bool isInitialize = false;
        //当前正在设置的磁贴的实例对象
        private TileDetail tile;
        private TilesPreferenceEditor editor;
        private int id;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //从Extra中获取要进行自定义设置的AppWidgetId
            id = Intent.GetIntExtra("id", -1);
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

            //使通知栏与应用标题栏颜色一致
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            }

            //页面上的控件
            Button btnChooseApp = FindViewById<Button>(Resource.Id.btnChooseApp);
            CheckBox checkShowNotif = FindViewById<CheckBox>(Resource.Id.checkShowNotif);
            CheckBox checkShowNotifIcon = FindViewById<CheckBox>(Resource.Id.checkShowNotifIcon);
            RadioGroup radioGroupTileType = FindViewById<RadioGroup>(Resource.Id.radioGroupTileType);
            EditText editRssUrl = FindViewById<EditText>(Resource.Id.editRssUrl);
            Button btnColor = FindViewById<Button>(Resource.Id.btnColor);

            //获取存储的磁贴信息
            editor = new TilesPreferenceEditor(this);

            if (isInitialize)
            {
                tile = new TileDetail();
                tile.ShowNotification = true;
                checkShowNotif.Checked = true;
                tile.ShowNotifIcon = false;
                checkShowNotifIcon.Checked = false;
                tile.TileType = LiveTileType.None;
                radioGroupTileType.Check(Resource.Id.radioTypeNone);
                editRssUrl.Visibility = ViewStates.Gone;
                tile.TileColor = -1;
                btnColor.SetBackgroundColor(new Color(editor.AutoTileColor));
                btnColor.Text = "自动从壁纸取色";
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
                    checkShowNotifIcon.Checked = tile.ShowNotifIcon;
                    switch (tile.TileType)
                    {
                        case LiveTileType.None:
                            radioGroupTileType.Check(Resource.Id.radioTypeNone);
                            editRssUrl.Visibility = ViewStates.Gone;
                            break;
                        case LiveTileType.Rss:
                            radioGroupTileType.Check(Resource.Id.radioTypeRss);
                            editRssUrl.Visibility = ViewStates.Visible;
                            editRssUrl.Text = tile.RssUrl;
                            break;
                    }
                    int color;
                    string colorName = "";
                    switch (tile.TileColor)
                    {
                        case -1://自动
                            color = editor.AutoTileColor;
                            colorName = "自动从壁纸取色";
                            break;
                        case -2://全局默认色
                            color = editor.DefaultTileColor;
                            colorName = "全局自定义颜色";
                            break;
                        default:
                            color = tile.TileColor;
                            break;
                    }
                    btnColor.Text = colorName;
                    btnColor.SetBackgroundColor(new Color(color));
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
            //点击按钮时跳转到选择颜色的Activity
            btnColor.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(ColorPicker));
                StartActivityForResult(intent, 1);
            };

            //勾选状态更改时保存设置
            checkShowNotif.CheckedChange += (sender, e) =>
            {
                tile.ShowNotification = checkShowNotif.Checked;
            };
            checkShowNotifIcon.CheckedChange += (sender, e) =>
            {
                tile.ShowNotifIcon = checkShowNotifIcon.Checked;
            };
            radioGroupTileType.CheckedChange += (sender, e) =>
            {
                switch (radioGroupTileType.CheckedRadioButtonId)
                {
                    case Resource.Id.radioTypeNone://无内容
                        tile.TileType = LiveTileType.None;
                        editRssUrl.Visibility = ViewStates.Gone;
                        break;
                    case Resource.Id.radioTypeRss://Rss
                        tile.TileType = LiveTileType.Rss;
                        editRssUrl.Visibility = ViewStates.Visible;
                        if (editRssUrl.Text != "")
                        {
                            tile.RssUrl = editRssUrl.Text;
                        }
                        break;
                }
            };
            editRssUrl.TextChanged += (sender, e) =>
            {
                tile.RssUrl = editRssUrl.Text;
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case 0://应用选择界面的请求码
                        string label = data.GetStringExtra("Label");
                        FindViewById<Button>(Resource.Id.btnChooseApp).Text = label ?? "设置应用";
                        tile.Label = label;
                        tile.Name = data.GetStringExtra("Name");
                        //tile.Icon = (Bitmap)data.GetParcelableExtra("Icon");
                        if (label != null && isInitialize)
                        {
                            var optFinish = _menu.Add(0, 0, 0, "保存");
                            optFinish.SetShowAsAction(ShowAsAction.Always);
                        }
                        //根据一些应用自动推荐动态磁贴类型
                        switch (label)
                        {
                            case "动态磁贴10":
                            case "IT之家":
                                FindViewById<RadioGroup>(Resource.Id.radioGroupTileType).Check(Resource.Id.radioTypeRss);
                                FindViewById<EditText>(Resource.Id.editRssUrl).Text = @"http://api.ithome.com/xml/newslist/news.xml";
                                break;
                        }
                        break;
                    case 1://颜色选择界面的请求码
                        tile.TileColor = data.GetIntExtra("Color", -1);
                        int color;
                        string colorName = "";
                        switch (tile.TileColor)
                        {
                            case -1://自动
                                color = editor.AutoTileColor;
                                colorName = "自动从壁纸取色";
                                break;
                            case -2://全局默认色
                                color = editor.DefaultTileColor;
                                colorName = "全局自定义颜色";
                                break;
                            default:
                                color = tile.TileColor;
                                break;
                        }
                        var button = FindViewById<Button>(Resource.Id.btnColor);
                        button.Text = colorName;
                        button.SetBackgroundColor(new Color(color));
                        break;
                }
            }
        }

        /// <summary>
        /// 存储action bar menu上的保存按钮的对象供其它地方使用
        /// </summary>
        private IMenu _menu;

        /// <summary>
        /// 创建action bar菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            _menu = menu;
            if (!isInitialize)
            {
                var optFinish = menu.Add(0, 0, 0, "保存");
                optFinish.SetShowAsAction(ShowAsAction.Always);
                var option = menu.Add(0, 1, 1, "Debug Delete");
                option.SetShowAsAction(ShowAsAction.CollapseActionView);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0://保存 点击按钮时保存更改并立即刷新磁贴
                    if (isInitialize)
                    {
                        tile.Id = id;
                        editor.Tiles.Add(tile);
                    }
                    editor.CommitChanges();
                    Codes.UpdateTiles(id, this, null, null);
                    Intent iRss = new Intent(this, typeof(ReadRss));
                    iRss.SetAction("com.LiveTilesWidget.UpdateRss");
                    StartService(iRss);

                    Intent result = new Intent();
                    Bundle b = new Bundle();
                    b.PutInt(AppWidgetManager.ExtraAppwidgetId, id);
                    result.PutExtras(b);
                    SetResult(Result.Ok, result);

                    Log.Debug("LiveTileWidget", "TileSettingReturned");
                    Finish();

                    break;

                case 1://Debug Delete
                    AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    dialog.SetIcon(Resource.Drawable.Icon);
                    dialog.SetTitle("删除此条配置？");
                    dialog.SetMessage("此功能仅当在开发过程中使用，请慎重。");
                    dialog.SetCancelable(true);
                    dialog.SetPositiveButton("确定删除", (sender, e) =>
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