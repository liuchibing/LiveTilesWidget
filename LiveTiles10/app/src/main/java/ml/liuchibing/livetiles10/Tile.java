package ml.liuchibing.livetiles10;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.util.Log;

import java.util.Dictionary;

/**
 * 一个动态磁贴的信息.
 *
 * @author liuchibing
 *         Created by charlie on 2017/2/9.
 */

public class Tile {
    public static final String EXTRA_RSS_URL = "RssUrl";
    public static final String EXTRA_NOTIFICATION_COUNT = "NotificationCount";
    public static final String EXTRA_RECORD_NOTIFICATION_COUNT = "RecordNotificationCount";

    public Dictionary<String, String> ExtraInfo;

    private Bitmap icon;
    private String label;
    private String packageName;
    private TileType type;
    private int tileColor;
    private int id;

    public void LoadIcon(Context context) {
        try {
            icon = ((BitmapDrawable) context.getPackageManager().getActivityIcon(context.getPackageManager().getLaunchIntentForPackage(getPackageName()))).getBitmap();
        } catch (Exception e) {
            Log.d("LoadIconError", "error:", e);
        }
    }

    public Bitmap getIcon() {
        return icon;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getLabel() {
        return label;
    }

    public void setLabel(String label) {
        this.label = label;
    }

    public String getPackageName() {
        return packageName;
    }

    public void setPackageName(String packageName) {
        this.packageName = packageName;
    }

    public TileType getType() {
        return type;
    }

    public void setType(TileType type) {
        this.type = type;
    }

    public int getTileColor() {
        return tileColor;
    }

    public void setTileColor(int tileColor) {
        this.tileColor = tileColor;
    }

}
