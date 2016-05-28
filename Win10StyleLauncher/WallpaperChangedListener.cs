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
using Android.Util;
using static Android.Support.V7.Graphics.Palette;
//using static Android.Support.V7.Graphics.Palette;

namespace LiveTilesWidget
{
    /// <summary>
    /// 当壁纸更改时更新磁贴颜色
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionWallpaperChanged })]
    public class WallpaperChangedListener : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("wallpaperChanged", "received");

            //获取壁纸，并由此生成调色板
            WallpaperManager wall = WallpaperManager.GetInstance(context);
            Palette palette = Palette.From(((BitmapDrawable)wall.Drawable).Bitmap).Generate();

            //获取存储的磁贴信息
            var preference = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            var editor = preference.Edit();

            //将壁纸主色调写入存储
            int color;
            color = palette.GetLightVibrantColor(-1);
            if (color == -1)
            {
                color = Codes.GetMainColor(new List<Swatch>()
                {
                    palette.LightVibrantSwatch,
                    palette.LightMutedSwatch,
                    palette.DarkMutedSwatch,
                    palette.DarkVibrantSwatch,
                    palette.VibrantSwatch,
                    palette.MutedSwatch
                });
                //
                if (color == -1)
                {
                    color = Codes.GetMainColor(palette.Swatches);
                }
            }
            editor.PutInt("AutoTileColor", color);
            editor.Commit();

            //更新所有磁贴
            foreach (var item in preference.GetStringSet("Ids", new List<string>()))
            {
                Codes.UpdateTiles(Convert.ToInt32(item), context, null);
            }
        }
    }
}