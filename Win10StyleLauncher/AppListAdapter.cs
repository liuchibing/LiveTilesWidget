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
using Java.Lang;

namespace LiveTilesWidget
{
    /// <summary>
    /// 用于适配以AppDetail为内容的ListView
    /// </summary>
    public class AppListAdapter : ArrayAdapter<AppDetail>, ISectionIndexer
    {
        Dictionary<string, int> alphaindex;
        Java.Lang.Object[] sectionsObjects;
        string[] sections;

        public AppListAdapter(Context context, int textViewResourceId, AppDetail[] objects)
            : base(context, textViewResourceId, objects)
        {
            alphaindex = new Dictionary<string, int>();

            //获取每种关键字的起始数据索引
            for (int i = 0; i < objects.Length; i++)
            {
                //objects[i].Label[0].ToString();
                string key = objects[i].GetSortLetters();
                if (!alphaindex.ContainsKey(key))
                {
                    alphaindex.Add(key, i);
                }
            }

            //将关键字转换成数据
            sections = new string[alphaindex.Keys.Count];
            alphaindex.Keys.CopyTo(sections, 0);

            //将关键字转换成Java.Lang.String类型
            sectionsObjects = new Java.Lang.Object[alphaindex.Keys.Count];
            for (int i = 0; i < sections.Length; i++)
            {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }

        }

        public int GetPositionForSection(int sectionIndex)
        {
            //根据关键字索引获取关键字，然后在根据关键字从alphaindex获取对应的value，即该关键字的起始数据索引
            return alphaindex[sections[sectionIndex]];

        }

        public int GetSectionForPosition(int position)
        {
            int preposition = 0;
            //循环关键字
            for (int i = 0; i < sections.Length; i++)
            {
                //判断当前的索引是否在i所在关键字的范围内
                if (GetPositionForSection(i) > position)
                    break;
                preposition = i;
            }
            return preposition;
        }

        public Java.Lang.Object[] GetSections()
        {
            return sectionsObjects;
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