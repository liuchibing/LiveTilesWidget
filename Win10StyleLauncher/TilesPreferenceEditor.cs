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

using Newtonsoft.Json;

namespace LiveTilesWidget
{
    /// <summary>
    /// 提供修改磁贴配置的各种方法
    /// </summary>
    public class TilesPreferenceEditor
    {
        public TilesPreferenceEditor(Context context)
        {
            //获取SharedPreferences及其Editor
            preferences = context.GetSharedPreferences("tiles", FileCreationMode.Private);
            editor = preferences.Edit();
            tiles = new List<string>(preferences.GetStringSet("Tiles", new List<string>()));
            Tiles = new List<AppDetail>();

            //反序列化磁贴配置
            foreach (var item in tiles)
            {
                Tiles.Add(JsonConvert.DeserializeObject<AppDetail>(item));
            }
        }

        //SharedPreferences及其Editor
        private ISharedPreferences preferences;
        private ISharedPreferencesEditor editor;
        //存储磁贴设置的StringSet
        private List<string> tiles;

        /// <summary>
        /// 所有的磁贴的配置信息
        /// </summary>
        public List<AppDetail> Tiles
        {
            get;
            set;
        }

        /// <summary>
        /// 获取具有指定Id的磁贴小部件的配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppDetail GetTileById(int id)
        {
            foreach (var item in Tiles)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 保存所有对磁贴配置的修改
        /// </summary>
        public void CommitChanges()
        {
            //序列化磁贴配置
            List<string> list = new List<string>();
            foreach (var item in Tiles)
            {
                //item.Icon = null;//清空Icon属性，防止出错
                list.Add(JsonConvert.SerializeObject(item));
            }
            editor.PutStringSet("Tiles", list);
            editor.Commit();
        }
    }
}