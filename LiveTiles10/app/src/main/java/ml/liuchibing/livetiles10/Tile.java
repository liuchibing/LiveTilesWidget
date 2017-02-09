package ml.liuchibing.livetiles10;

import java.util.Dictionary;

/**
 * Description: 一个动态磁贴的信息
 * Created by charlie on 2017/2/9.
 */

public class Tile {
    public static final String EXTRA_RSS_URL = "RssUrl";
    public static final String EXTRA_NOTIFICATION_COUNT = "NotificationCount";

    public Dictionary<String, String> ExtraInfo;

    private String _label;
    private String _packageName;
    private TileType _type;
    private int _tileColor;
    private int _id;

    public String get_label() {
        return _label;
    }

    public void set_label(String _label) {
        this._label = _label;
    }

    public String get_packageName() {
        return _packageName;
    }

    public void set_packageName(String _packageName) {
        this._packageName = _packageName;
    }

    public TileType get_type() {
        return _type;
    }

    public void set_type(TileType _type) {
        this._type = _type;
    }

    public int get_tileColor() {
        return _tileColor;
    }

    public void set_tileColor(int _tileColor) {
        this._tileColor = _tileColor;
    }

    public int get_id() {
        return _id;
    }

    public void set_id(int _id) {
        this._id = _id;
    }

}
