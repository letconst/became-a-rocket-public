using UnityEngine;

[System.Serializable]
public sealed class BackgroundImage
{
    [SerializeField, Header("背景画像")]
    private Sprite mainImage;

    /// <summary>背景画像</summary>
    public Sprite MainImage => mainImage;

    [SerializeField, Header("次の背景画像との間に配置するフェード背景画像")]
    private Sprite fadeImageForNext;

    /// <summary>次の背景画像との間に配置するフェード背景画像</summary>
    public Sprite FadeImageForNext => fadeImageForNext;

    [SerializeField, Header("生成する最小範囲")]
    private int generateMinRange;

    /// <summary>生成する最小範囲</summary>
    public int GenerateMinRange => generateMinRange;

    [SerializeField, Header("生成する最大範囲")]
    private int generateMaxRange;

    /// <summary>生成する最大範囲</summary>
    public int GenerateMaxRange => generateMaxRange;
}
