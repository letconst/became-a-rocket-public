using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ゲーム説明表示UIのPresenterクラス
/// </summary>
public sealed class DescriptionPresenter : MonoBehaviour
{
    [SerializeField]
    private DescriptionView view;

    [SerializeField]
    private DescriptionModel model;

    [SerializeField, Header("説明表示をフェードアウトする秒数")]
    private float fadeOutSeconds;

    private void Start()
    {
        Assert.IsNotNull(view, "_view != null");

        view.CanvasGroup.alpha = 1f;

        // ゲームステートがReady以外になったら説明を非表示にする
        GameManager.Instance.CurrentState
                   .Skip(1)
                   .Where(static newState => newState != GameState.Ready)
                   .Take(1)
                   .Subscribe(_ => OnGameStateChanged().Forget())
                   .AddTo(this);
    }

    private async UniTaskVoid OnGameStateChanged()
    {
        Assert.IsNotNull(view, "_view != null");
        Assert.IsNotNull(model, "_model != null");

        await model.FadeOut(view.CanvasGroup, fadeOutSeconds);

        gameObject.SetActive(false);
    }
}
