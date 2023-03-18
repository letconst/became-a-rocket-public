using UnityEngine;

/// <summary>
/// ゲーム説明表示UIのViewクラス
/// </summary>
public sealed class DescriptionView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    /// <summary>説明表示のCanvasGroup</summary>
    public CanvasGroup CanvasGroup => canvasGroup;
}
