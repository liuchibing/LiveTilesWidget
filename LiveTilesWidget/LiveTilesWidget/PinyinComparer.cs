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
    /*
     * 感谢CSDN博主“xiaanming”的文章 http://blog.csdn.net/xiaanming/article/details/12684155 
     * 本程序部分代码是经过学习该文章的Java代码后以C#语言写出的。
     */
    public class PinyinComparer : IComparer<TileDetail>//Comparator<SortModel> {
    {
        public int Compare(TileDetail x, TileDetail y)
        {
            //这里主要是用来对ListView里面的数据根据ABCDEFG...来排序
            if (y.GetSortLetters() == "#")
            {
                return -1;
            }
            else if (x.GetSortLetters() == "#")
            {
                return 1;
            }
            else
            {
                return x.GetSortLetters().CompareTo(y.GetSortLetters());
            }
        }
    }
}