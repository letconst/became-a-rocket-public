using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

public sealed partial class EndlessBackgroundHandler : MonoBehaviour
{
    [SerializeField, Header("エンドレス背景として使用する画像")]
    private Sprite backgroundSprite;

    [SerializeField, Header("通常背景のSpriteRenderer")]
    private SpriteRenderer normalBackground;

    private float _screenYLength;
    private float _bgSpriteYLength;
    private float _normalBgTopPos;

    private Camera _cam;

    private readonly List<EndlessBackground> _backgrounds = new();

    private EndlessBackground _topmostBg;
    private EndlessBackground _lowestBg;

    private void Start()
    {
        _cam = Camera.main;

        Assert.IsNotNull(backgroundSprite);
        Assert.IsNotNull(_cam);

        Initialize();
        EventReceiver();
    }

    private void EventReceiver()
    {
        ScoreManager scoreManager = ScoreManager.Instance;

        Assert.IsNotNull(scoreManager, "scoreManager != null");

        // プレイヤーの位置に応じて、背景を動かすイベントを登録
        scoreManager.CurrentScoreChanged
                    // 一つ前の値もほしいためZipでまとめる
                    .Zip(scoreManager.CurrentScoreChanged.Skip(1), static (old, @new) => (old, @new))
                    .Subscribe(OnCurrentScoreChanged)
                    .AddTo(this);
    }

    /// <summary>
    /// プレイヤーのy座標が更新された際に背景を動かすイベント処理
    /// </summary>
    /// <param name="score">更新されたy座標 (old: 1つ前の座標, @new: 現在の座標)</param>
    private void OnCurrentScoreChanged((int old, int @new) score)
    {
        PlayerStatusManager playerStatusManager = PlayerStatusManager.Instance;

        Assert.IsNotNull(playerStatusManager, "playerStatusManager != null");

        // 背景のエンドレス表示をし始めるプレイヤーのy座標
        // 通常背景の上端位置 - プレイヤーの初期位置y + 画面の縦の長さ (距離) + エンドレス背景の長さ (距離)
        float requirePlayerPosY = _normalBgTopPos - playerStatusManager.InitialPosition.y + _screenYLength + _bgSpriteYLength;

        // プレイヤーが通常背景の領域にいる間は処理しない
        if (score.@new < requirePlayerPosY)
            return;

        float oldScoreRemainder = score.old  % _bgSpriteYLength;
        float newScoreRemainder = score.@new % _bgSpriteYLength;

        // プレイヤーが上昇中に背景のつなぎ目を超えたら、最下部の背景を上に移動
        if (newScoreRemainder < oldScoreRemainder && playerStatusManager.MoveDirection == PlayerMoveDirection.Up)
        {
            SetPositionToTopmost(_lowestBg);
        }
        // プレイヤーが下降中に背景のつなぎ目を超えたら、最上部の背景を下に移動
        else if (newScoreRemainder > oldScoreRemainder && playerStatusManager.MoveDirection == PlayerMoveDirection.Down)
        {
            SetPositionToLowest(_topmostBg);
        }
    }

    private void SetPositionToTopmost(EndlessBackground target)
    {
        Assert.IsNotNull(_topmostBg, "_topmostBg != null");
        Assert.IsNotNull(target, "target != null");

        InternalSetPosition(target, _topmostBg);

        target.Prev = _topmostBg;
        _topmostBg  = target;
        _lowestBg   = GetLowestBackground();
        target.Next = null;
    }

    private void SetPositionToLowest(EndlessBackground target)
    {
        Assert.IsNotNull(_lowestBg, "_lowestBg != null");
        Assert.IsNotNull(target, "target != null");

        InternalSetPosition(target, _lowestBg);

        target.Next = _lowestBg;
        _lowestBg   = target;
        _topmostBg  = GetTopmostBackground();
        target.Prev = null;
    }

    private void InternalSetPosition(EndlessBackground target, EndlessBackground oppositeBg)
    {
        Vector3 oppositeBgPos = oppositeBg.Renderer.transform.TransformPoint(oppositeBg.Renderer.sprite.bounds.max);

        Transform bgTrf = target.Renderer.transform;
        oppositeBgPos.x = bgTrf.position.x;
        bgTrf.position  = oppositeBgPos;
    }

    private EndlessBackground GetTopmostBackground()
    {
        EndlessBackground topmost = null;

        foreach (EndlessBackground bg in _backgrounds)
        {
            if (topmost == null)
            {
                topmost = bg;

                continue;
            }

            float topmostY = topmost.Renderer.transform.position.y;
            float bgY      = bg.Renderer.transform.position.y;

            if (topmostY < bgY)
            {
                topmost = bg;
            }
        }

        return topmost;
    }

    private EndlessBackground GetLowestBackground()
    {
        EndlessBackground lowest = null;

        foreach (EndlessBackground bg in _backgrounds)
        {
            if (lowest == null)
            {
                lowest = bg;

                continue;
            }

            float lowestY = lowest.Renderer.transform.position.y;
            float bgY     = bg.Renderer.transform.position.y;

            if (lowestY > bgY)
            {
                lowest = bg;
            }
        }

        return lowest;
    }
}
