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

namespace LiveTilesWidget
{
    [Activity(Label = "选择一个颜色")]
    public class ColorPicker : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //防止意外退出
            SetResult(Result.Canceled);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ColorPicker);

            int color;
            TilesPreferenceEditor editor = new TilesPreferenceEditor(this, true);
            //如果是从Main启动的
            if (Intent.GetBooleanExtra("IsSettingDefaultColor", false))
            {
                FindViewById<RelativeLayout>(Resource.Id.colorAuto).Visibility = ViewStates.Gone;
                FindViewById<RelativeLayout>(Resource.Id.colorDefault).Visibility = ViewStates.Gone;
                color = Resource.Color.lightblue500;
            }
            else
            {
                //填充自动色和全局色
                Bitmap bitmap = Bitmap.CreateBitmap(new int[] { editor.AutoTileColor }, 1, 1, Bitmap.Config.Argb8888);
                FindViewById<ImageView>(Resource.Id.imgAuto).SetImageBitmap(bitmap);
                bitmap = Bitmap.CreateBitmap(new int[] { editor.DefaultTileColor }, 1, 1, Bitmap.Config.Argb8888);
                FindViewById<ImageView>(Resource.Id.imgDefault).SetImageBitmap(bitmap);

                color = -1;
            }
            //结束时执行
            Action finish = () =>
            {                
                Intent i = new Intent();
                i.PutExtra("Color", color);
                SetResult(Result.Ok, i);
                Finish();
            };

            //监听事件
            FindViewById<RelativeLayout>(Resource.Id.colorAuto).Click += (sender, e) =>
            {
                color = -1;//自动
                finish();
            };
            FindViewById<RelativeLayout>(Resource.Id.colorDefault).Click += (sender, e) =>
            {
                color = -2;//全局自定义色
                finish();
            };
            EventHandler colorImgClick = (sender, e) =>
            {
                try
                {
                    ImageView v = sender as ImageView;
                    color = (v.Drawable as ColorDrawable).Color.ToArgb();
                    finish();
                }
                catch
                {
                    finish();
                }
            };
            GridLayout root = FindViewById<GridLayout>(Resource.Id.colorPickerRoot);
            for (int i = 0; i < root.ChildCount; i++)
            {
                View v = root.GetChildAt(i);
                if (v is ImageView)
                {
                    ((ImageView)v).Click += colorImgClick;
                }
            }
        }
    }
}