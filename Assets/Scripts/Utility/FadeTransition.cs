using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// UIのフェード処理を提供するクラス
/// </summary>
public static class FadeTransition
{
    /// <summary>
    /// CanvasGroupのalpha値を指定時間かけて徐々に減らす
    /// </summary>
    /// <param name="target">対象のCanvasGroup</param>
    /// <param name="fadeTime">フェード時間 (秒)</param>
    /// <returns></returns>
    public static async UniTask FadeIn(CanvasGroup target, float fadeTime)
    {
        Assert.IsNotNull(target, "target != null");

        target.alpha = 1f;

        while (target.alpha > 0f)
        {
            target.alpha -= Time.deltaTime / fadeTime;
            target.alpha =  Mathf.Clamp01(target.alpha);

            await UniTask.Yield();
        }
    }

    /// <summary>
    /// CanvasGroupのalpha値を指定時間かけて徐々に増やす
    /// </summary>
    /// <param name="target">対象のCanvasGroup</param>
    /// <param name="fadeTime">フェード時間 (秒)</param>
    /// <returns></returns>
    public static async UniTask FadeOut(CanvasGroup target, float fadeTime)
    {
        Assert.IsNotNull(target, "target != null");

        target.alpha = 0f;

        while (target.alpha < 1f)
        {
            target.alpha += Time.deltaTime / fadeTime;
            target.alpha =  Mathf.Clamp01(target.alpha);

            await UniTask.Yield();
        }
    }

    /// <summary>
    /// SpriteRendererのalpha値を指定時間かけて徐々に増やす
    /// </summary>
    /// <param name="target">対象のSpriteRenderer</param>
    /// <param name="fadeTime">フェード時間 (秒)</param>
    /// <returns></returns>
    public static async UniTask FadeIn(SpriteRenderer target, float fadeTime)
    {
        Assert.IsNotNull(target, "target != null");

        Color colorSoFar = target.color;
        colorSoFar.a = 0f;
        target.color = colorSoFar;

        while (target.color.a < 1f)
        {
            colorSoFar.a += Mathf.Clamp01(Time.deltaTime / fadeTime);
            target.color =  colorSoFar;

            await UniTask.Yield();
        }
    }

    /// <summary>
    /// SpriteRendererのalpha値を指定時間かけて徐々に減らす
    /// </summary>
    /// <param name="target">対象のSpriteRenderer</param>
    /// <param name="fadeTime">フェード時間 (秒)</param>
    /// <returns></returns>
    public static async UniTask FadeOut(SpriteRenderer target, float fadeTime)
    {
        Assert.IsNotNull(target, "target != null");

        Color colorSoFar = target.color;
        colorSoFar.a = 1f;
        target.color = colorSoFar;

        while (target.color.a > 0f)
        {
            colorSoFar.a -= Mathf.Clamp01(Time.deltaTime / fadeTime);
            target.color =  colorSoFar;

            await UniTask.Yield();
        }
    }
}
