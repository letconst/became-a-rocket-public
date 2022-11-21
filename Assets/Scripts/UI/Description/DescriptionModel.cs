using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class DescriptionModel : MonoBehaviour
{
    /// <summary>
    /// 説明表示をフェードアウトする
    /// </summary>
    /// <param name="target">対象のCanvasGroup</param>
    /// <param name="fadeTime">フェードアウトする時間 (秒)</param>
    public async UniTask FadeOut(CanvasGroup target, float fadeTime)
    {
        Assert.IsNotNull(target, "target != null");

        await FadeTransition.FadeIn(target, fadeTime);
    }
}
