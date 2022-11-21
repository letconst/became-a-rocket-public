using UnityEngine;

public sealed class DescriptionView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    /// <summary>説明表示のCanvasGroup</summary>
    public CanvasGroup CanvasGroup => canvasGroup;
}
