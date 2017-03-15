using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveTilesWidget
{
    /// <summary>
    /// 提供修改磁贴配置的各种方法
    /// </summary>
    public class TilesPreferenceEditor
    {
        /// <summary>
        /// 当前的Preferences数据格式版本号
        /// </summary>
        private const int CurrentPreferencesVersion = 2;

        /// <summary>
        /// 正常初始化一个新实例
        /// </summary>
        /// <param name="context"></param>
        public TilesPreferenceEditor(Context context) : this(context, false)
        {
        }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dontReadTiles">如果指定为true则不会将JSON格式存储的磁贴配置读取为对象</param>
        public TilesPreferenceEditor(Context context, bool dontReadTiles)
        {
            _context = context;
            //获取SharedPreferences及其Editor
            _preferences = Codes.GetPreferences(context);
            _editor = _preferences.Edit();
            //自动迁移旧版preferences数据至新版
            if (PreferencesVersion < CurrentPreferencesVersion)
            {
                Codes.MovePreferences(PreferencesVersion, context);
                PreferencesVersion = CurrentPreferencesVersion;
            }
            _tiles = new List<string>(_preferences.GetStringSet("Tiles", new List<string>()));
            Tiles = new List<TileDetail>();

            _dontReadTiles = dontReadTiles;
            if (!dontReadTiles)
            {
                //反序列化磁贴配置
                foreach (var item in _tiles)
                {
                    try
                    {
                        TileDetail tile = JsonConvert.DeserializeObject<TileDetail>(item);
                        //加载图标
                        tile.LoadIcon(context);
                        Tiles.Add(tile);
                    }
                    catch { }
                }
            }
        }

        //SharedPreferences及其Editor
        private ISharedPreferences _preferences;
        private ISharedPreferencesEditor _editor;
        //存储磁贴设置的StringSet
        private List<string> _tiles;
        //创建了当前对象的上下文
        private Context _context;
        //是否将JSON格式存储的磁贴配置读取为对象
        private bool _dontReadTiles;

        /// <summary>
        /// 所有的磁贴的配置信息
        /// </summary>
        public List<TileDetail> Tiles
        {
            get;
            set;
        }

        /// <summary>
        /// 获取具有指定Id的磁贴小部件的配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TileDetail GetTileById(int id)
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
            if (!_dontReadTiles)
            {
                //序列化磁贴配置
                List<string> list = new List<string>();
                foreach (var item in Tiles)
                {
                    item.Icon = null;//清空Icon属性，防止出错
                    list.Add(JsonConvert.SerializeObject(item));
                }
                _editor.PutStringSet("Tiles", list);
            }
                _editor.Commit();
        }

        /// <summary>
        /// SharedPreferences使用的数据格式的版本号
        /// </summary>
        public int PreferencesVersion
        {
            get
            {
                return _preferences.GetInt("PreferencesVersion", 0);
            }
            set
            {
                _editor.PutInt("PreferencesVersion", value);
                _editor.Commit();
            }
        }

        /// <summary>
        /// 全局默认磁贴背景色，更改会自动保存
        /// </summary>
        public int DefaultTileColor
        {
            get
            {
                return _preferences.GetInt("DefaultTileColor", _context.GetColor(Resource.Color.lightblue500));
            }
            set
            {
                _editor.PutInt("DefaultTileColor", value);
                _editor.Commit();
            }
        }

        /// <summary>
        /// 从壁纸中自动取得的磁贴背景色，更改会自动保存
        /// </summary>
        public int AutoTileColor
        {
            get
            {
                return _preferences.GetInt("AutoTileColor", _context.GetColor(Resource.Color.lightblue500));
            }
            set
            {
                _editor.PutInt("AutoTileColor", value);
                _editor.Commit();
            }
        }
    }
}