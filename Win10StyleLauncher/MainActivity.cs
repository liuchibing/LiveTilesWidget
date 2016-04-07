using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace LiveTilesWidget
{
    [Activity(Label = "动态磁贴窗口小部件", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button button1 = FindViewById<Button>(Resource.Id.button1);

            button1.Click += Button1_Click;
            var preference = GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TileSetting));
            intent.PutExtra("id", 85);
            StartActivity(intent);
        }
    }
}

