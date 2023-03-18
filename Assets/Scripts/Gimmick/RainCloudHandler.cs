using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <summary>
/// ギミック「雨雲」の動作処理を行うクラス
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public sealed class RainCloudHandler : GimmickHandlerBase
{
    private SpriteRenderer _selfRenderer;

    private float _healAmount;

    protected override void Start()
    {
        base.Start();

        _selfRenderer = GetComponent<SpriteRenderer>();
        _healAmount   = 30f; // TODO: 現状定数のため、別の方法で値をセットする

        EventReceiver();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState.Value != GameState.InGame)
            return;

        // プレイヤーと接触したらイベント発行し、自身を破棄
        if (GimmickUtility.IsContactedToPlayer(SelfCollider) && _selfRenderer.isVisible)
        {
            GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnRainCloudContacted.Get(_healAmount));
            GameManager.Instance.PlayerBroker.Publish(
                GameEvent.Player.OnReturnGimmickRequest.Get(GimmickType.RainCloud, gameObject));
        }
    }

    private void EventReceiver()
    {
        this.OnBecameInvisibleAsObservable()
            .Where(_ => gameObject.activeSelf)
            .Subscribe(_ => OnInvisible())
            .AddTo(this);
    }

    private void OnInvisible()
    {
        // プールへの返却イベント発行
        GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnReturnGimmickRequest.Get(GimmickType.RainCloud, gameObject));
    }
}
