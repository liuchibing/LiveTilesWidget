package ml.liuchibing.livetiles10;

import android.content.Context;
import android.content.SharedPreferences;

import com.google.gson.Gson;

/**
 * 负责管理磁贴配置的类.
 * 提供修改磁贴配置的各种方法.
 *
 * @author liuchibing
 *         Created by charlie on 2017/2/10.
 */

public class TilesPreferenceEditor {
    /**
     * 当前配置文件格式的版本.
     */
    public static final int CURRENT_PREFERENCES_VERSION = 1;
    private static final String PREFERENCE_NAME = "ml.liuchibing.livetiles10.tiles";

    private Context context;
    private SharedPreferences preferences;
    private SharedPreferences.Editor editor;

    /**
     * 初始化一个Editor对象.
     *
     * @param creatorContext 对象创建者的Context
     */
    public TilesPreferenceEditor(Context creatorContext) {
        context = creatorContext;
        //获取SharedPreferences及其Editor
        preferences = context.getSharedPreferences(PREFERENCE_NAME, Context.MODE_PRIVATE);
        editor = preferences.edit();
    }
}
