using UnityEngine;

public static class PlayerPrefsUtility
{
    /// <summary>
    /// 指定のオブジェクトをJSONとして<see cref="PlayerPrefs"/>に保存する
    /// </summary>
    /// <param name="key">保存するキー名</param>
    /// <param name="obj">保存対象</param>
    /// <typeparam name="T"></typeparam>
    public static void SetObject<T>(string key, T obj) where T : class
    {
        string json = JsonUtility.ToJson(obj);

        PlayerPrefs.SetString(key, json);
    }

    /// <summary>
    /// 指定のキー名のオブジェクトを<see cref="PlayerPrefs"/>から取得する
    /// </summary>
    /// <param name="key">取得するキー名</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetObject<T>(string key) where T : class
    {
        string json = PlayerPrefs.GetString(key);
        var    obj  = JsonUtility.FromJson<T>(json);

        return obj;
    }
}
