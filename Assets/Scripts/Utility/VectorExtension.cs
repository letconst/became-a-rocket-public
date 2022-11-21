using UnityEngine;

public static class VectorExtension
{
    /// <summary>
    /// <see cref="Vector3"/>と<see cref="Vector2"/>を加算する
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 Add(this Vector3 a, Vector2 b)
    {
        return new Vector3(a.x + b.x, a.y + b.y, a.z);
    }

    /// <summary>
    /// <see cref="Vector2"/>と<see cref="Vector3"/>を加算する
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 Add(this Vector2 a, Vector3 b)
    {
        return new Vector3(a.x + b.x, a.y + b.y, b.z);
    }

    /// <summary>
    /// 絶対値とした<see cref="Vector3"/>を取得する
    /// </summary>
    /// <param name="source">絶対値にする対象</param>
    /// <returns></returns>
    public static Vector3 Abs(this Vector3 source)
    {
        return new Vector3(Mathf.Abs(source.x), Mathf.Abs(source.y), Mathf.Abs(source.z));
    }
}
