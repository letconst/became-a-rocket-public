using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// UnityのVector系に関するユーティリティクラス
/// </summary>
public static class VectorUtility
{
    /// <summary>
    /// 画面座標の中心を取得する
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetCenterScreenPosition()
    {
        var centerPos = new Vector2(Screen.width / 2f, Screen.height / 2f);

        return centerPos;
    }

    /// <summary>
    /// 画面の縦横の長さを、ワールド座標の距離として取得する
    /// </summary>
    /// <param name="camera">長さとする対象のカメラ</param>
    /// <returns></returns>
    public static Vector2 GetScreenLength(Camera camera)
    {
        Assert.IsNotNull(camera);

        Vector2 leftBottomPos = camera.ScreenToWorldPoint(Vector3.zero);
        Vector2 rightTopPos   = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        var     length        = new Vector2(rightTopPos.x - leftBottomPos.x, rightTopPos.y - leftBottomPos.y);

        return length;
    }
}
