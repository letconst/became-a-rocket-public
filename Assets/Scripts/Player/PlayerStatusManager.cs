using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ギャー君の状態を管理するクラス
/// </summary>
public sealed class PlayerStatusManager : SingletonMonoBehaviour<PlayerStatusManager>
{
    /// <summary>
    /// プレイヤーの初期位置
    /// </summary>
    public Vector2 InitialPosition { get; set; }

    /// <summary>
    /// 現在のプレイヤーの移動している方向
    /// </summary>
    public PlayerMoveDirection MoveDirection { get; private set; }

    private void Start()
    {
        EventReceiver();
    }

    private void EventReceiver()
    {
        ScoreManager scoreManager = ScoreManager.Instance;

        Assert.IsNotNull(scoreManager, "scoreManager != null");

        // スコアの増減によってプレイヤーの移動方向を取得
        scoreManager.CurrentScoreChanged
                    .Zip(scoreManager.CurrentScoreChanged.Skip(1), static (old, @new) => (old, @new))
                    .Subscribe(OnCurrentScoreChanged)
                    .AddTo(this);
    }

    private void OnCurrentScoreChanged((int old, int @new) score)
    {
        // スコアが加算なら上昇判定
        if (score.old < score.@new)
        {
            MoveDirection = PlayerMoveDirection.Up;
        }
        // 減算なら下降判定
        else if (score.old > score.@new)
        {
            MoveDirection = PlayerMoveDirection.Down;
        }
        // 同じなら滞空判定
        else if (score.old == score.@new)
        {
            MoveDirection = PlayerMoveDirection.Idle;
        }
    }
}
