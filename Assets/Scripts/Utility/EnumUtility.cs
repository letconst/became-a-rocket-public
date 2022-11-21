using System;
using System.Collections.Generic;

public static class EnumUtility
{
    /// <summary>
    /// 指定のEnumの値数を取得する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int GetCount<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// 指定のEnumに対応するindexの値を取得する
    /// </summary>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T IndexToValue<T>(int index) where T : Enum
    {
        return (T) Enum.ToObject(typeof(T), index);
    }

    /// <summary>
    /// ランダムな指定のEnumの値を取得する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandom<T>() where T : Enum
    {
        int rand = UnityEngine.Random.Range(0, GetCount<T>());

        return IndexToValue<T>(rand);
    }

    /// <summary>
    /// 指定のEnumから、指定の値を除いた値をランダムに取得する
    /// </summary>
    /// <param name="excludeValue">選出から除外したい値</param>
    /// <typeparam name="T">選出対象のEnum</typeparam>
    /// <returns></returns>
    public static T GetRandomExcept<T>(params T[] excludeValue) where T : Enum
    {
        int        enumCount      = GetCount<T>();
        IList<int> excludeIndexes = GetValueIndexes(excludeValue);
        List<int>  includeIndexes = new();

        for (int i = 0; i < enumCount; i++)
        {
            if (excludeIndexes.Contains(i))
                continue;

            includeIndexes.Add(i);
        }

        int rand = UnityEngine.Random.Range(0, includeIndexes.Count);

        return IndexToValue<T>(includeIndexes[rand]);
    }

    /// <summary>
    /// 指定のEnumの値からランダムに値を取得する
    /// </summary>
    /// <param name="includeValues">選出させたい値</param>
    /// <typeparam name="T">選出対象のEnum</typeparam>
    /// <returns></returns>
    public static T GetRandomBy<T>(params T[] includeValues) where T : Enum
    {
        // 選出する値のインデックスを取得
        IList<int> includeIndexes = GetValueIndexes(includeValues);

        int rand = UnityEngine.Random.Range(0, includeIndexes.Count);

        return IndexToValue<T>(includeIndexes[rand]);
    }

    /// <summary>
    /// 指定のEnum値のインデックスを取得する
    /// </summary>
    /// <param name="enumValues"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static IList<int> GetValueIndexes<T>(IEnumerable<T> enumValues) where T : Enum
    {
        List<int> result = new();

        foreach (T enumValue in enumValues)
        {
            result.Add(Convert.ToInt32(enumValue));
        }

        return result;
    }
}
