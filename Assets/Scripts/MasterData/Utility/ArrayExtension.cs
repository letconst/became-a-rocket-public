using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension
{
    /// <summary>
    /// 配列から要素をランダムに取得する
    /// </summary>
    /// <param name="source">対象の配列</param>
    /// <returns></returns>
    public static T GetRandom<T>(this IList<T> source)
    {
        return source[Random.Range(0, source.Count)];
    }

    /// <summary>
    /// 配列から要素をランダムに取得する
    /// </summary>
    /// <param name="source">対象の配列</param>
    /// <returns></returns>
    public static T GetRandom<T>(this IReadOnlyList<T> source)
    {
        return source[Random.Range(0, source.Count)];
    }
}
