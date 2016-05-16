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
using Android.Support.V7.Graphics;
using Android.Graphics.Drawables;

namespace LiveTilesWidget
{
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionWallpaperChanged })]
    public class WallpaperChangedListener : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            WallpaperManager wall = WallpaperManager.GetInstance(context);
            Palette palette = Palette.From(((BitmapDrawable)wall.Drawable).Bitmap).Generate();
            
            
        }
    }
}