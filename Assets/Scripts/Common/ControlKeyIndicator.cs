using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class ControlKeyIndicator : MonoBehaviour
{
    [SerializeField, Header("左回転操作キー画像のSpriteRenderer")]
    private SpriteRenderer leftKeyRender;

    [SerializeField, Header("右回転操作キー画像のSpriteRenderer")]
    private SpriteRenderer rightKeyRenderer;

    [SerializeField, Header("操作キー画像を追従する対象のTransform")]
    private Transform followTargetTrf;

    [SerializeField, Header("ゲーム開始からキー画像を表示する秒数")]
    private float indicateSeconds;

    [SerializeField, Header("表示時間経過後のフェードアウトする秒数")]
    private float keysFadeoutSeconds;

    [SerializeField, Header("ギャーくんからの左右キー画像のオフセット位置")]
    private Vector2 keysOffsetPosition;

    private readonly Dictionary<Transform, Vector2> _keysOffset = new();

    private Transform _leftKeyTrf;
    private Transform _rightKeyTrf;

    private IDisposable _followDisposable;
    private IDisposable _countdownDisposable;

    private void Start()
    {
        Assert.IsNotNull(leftKeyRender, "leftRender != null");
        Assert.IsNotNull(rightKeyRenderer, "rightRenderer != null");
        Assert.IsNotNull(followTargetTrf, "followTarget != null");

        _leftKeyTrf  = leftKeyRender.transform;
        _rightKeyTrf = rightKeyRenderer.transform;

        // オフセット位置のxを絶対値に
        keysOffsetPosition.x = Mathf.Abs(keysOffsetPosition.x);

        // 各オフセット位置を記憶
        _keysOffset.Add(_leftKeyTrf, new Vector2(-keysOffsetPosition.x, keysOffsetPosition.y));
        _keysOffset.Add(_rightKeyTrf, keysOffsetPosition);

        // 各種操作キー画像を、追従対象からのオフセット位置に配置
        FollowKeyPosition(_leftKeyTrf);
        FollowKeyPosition(_rightKeyTrf);

        // 追従処理登録
        _followDisposable = this.LateUpdateAsObservable()
                                .Subscribe(_ => UpdateFollow())
                                .AddTo(this);

        // 表示時間経過後に非表示とする処理登録
        _countdownDisposable = this.UpdateAsObservable()
                                   .Where(static _ => GameManager.Instance.CurrentState.Value == GameState.InGame)
                                   .Take(1)
                                   .Subscribe(_ => CountdownToHideIndicator().Forget())
                                   .AddTo(this);
    }

    /// <summary>
    /// 指定したTransformの位置を、追従対象座標にオフセットを加算した場所に設定する
    /// </summary>
    /// <param name="targetToFollow">追従させるキー画像のTransform</param>
    private void FollowKeyPosition(Transform targetToFollow)
    {
        Assert.IsNotNull(targetToFollow, "targetToFollow != null");
        Assert.IsNotNull(followTargetTrf, "followTarget != null");

        Vector2 offset = _keysOffset[targetToFollow];
        targetToFollow.position = followTargetTrf.position.Add(offset);
    }

    private void UpdateFollow()
    {
        FollowKeyPosition(_leftKeyTrf);
        FollowKeyPosition(_rightKeyTrf);
    }

    private async UniTaskVoid CountdownToHideIndicator()
    {
        float countdownTimeElapsed = 0f;

        await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate())
        {
            countdownTimeElapsed += Time.deltaTime;

            if (countdownTimeElapsed >= indicateSeconds)
            {
                OnIndicateTimeIsOver().Forget();

                break;
            }
        }
    }

    private async UniTaskVoid OnIndicateTimeIsOver()
    {
        // 左右キー画像のフェードアウトを待機
        await UniTask.WhenAll(FadeTransition.FadeOut(leftKeyRender, keysFadeoutSeconds),
                              FadeTransition.FadeOut(rightKeyRenderer, keysFadeoutSeconds));

        // 追従および非表示までのカウント処理をdispose
        _followDisposable.Dispose();
        _countdownDisposable.Dispose();

        _followDisposable    = null;
        _countdownDisposable = null;
    }
}
