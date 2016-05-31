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
    /// 用于适配以AppDetail为内容的ListView
    /// </summary>
    public class AppListAdapter : ArrayAdapter<AppDetail>
    {
        public AppListAdapter(Context context, int textViewResourceId, AppDetail[] objects)
            : base(context, textViewResourceId, objects)
        {

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if (v == null)
            {
                v = ((Activity)Context).LayoutInflater.Inflate(Resource.Layout.AppPickerItems, null);
            }

            ImageView appIcon = v.FindViewById<ImageView>(Resource.Id.item_app_icon);
            appIcon.SetImageBitmap(GetItem(position).Icon);

            TextView appLabel = v.FindViewById<TextView>(Resource.Id.item_app_label);
            appLabel.Text = GetItem(position).Label;

            return v;
        }
    }
}