using UniRx;
using UnityEngine;

/// <summary>
/// スコアUIのPresenterクラス
/// </summary>
[RequireComponent(typeof(ScoreView))]
[RequireComponent(typeof(ScoreManager))]
public sealed class ScorePresenter : MonoBehaviour
{
    private ScoreView    _view;
    private ScoreManager _model;

    private void Start()
    {
        _view  = GetComponent<ScoreView>();
        _model = ScoreManager.Instance;

        // スコアの変更を反映
        GameManager.Instance.GameBroker.Receive<GameEvent.ScoreChangeRequest>()
                   .Subscribe(OnScoreChangeRequest)
                   .AddTo(this);
    }

    private void OnScoreChangeRequest(GameEvent.ScoreChangeRequest data)
    {
        _view.SetScoreText(data.NewScore.ToString("00000"));
        _model.SetScore(data.NewScore);
    }
}
