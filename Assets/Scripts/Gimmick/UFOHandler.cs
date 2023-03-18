using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ギミック「UFO」の動作処理を行うクラス
/// </summary>
public sealed class UFOHandler : GimmickHandlerBase, IInitializableGimmick, IMovableGimmick
{
    [SerializeField, Header("接触判定を行うCollider")]
    private Collider2D contactCollider;

    [SerializeField, Header("移動速度 (秒)")]
    private float moveSpeedInSec;

    [SerializeField, Header("ゆらゆら揺れる速度 (秒)")]
    private float swingSpeedInSec;

    [SerializeField, Header("揺れる幅 (m)")]
    private float swingWidth;

    [SerializeField, Header("移動処理が開始されるプレイヤーとの距離")]
    private float beginMoveRange;

    private float _baseXPosToMovement;

    private readonly SerialDisposable _scoreObserveDisposable = new();

    public float             MoveSpeedInSec => moveSpeedInSec;
    public bool              IsMoving       { get; private set; }
    public GimmickMoveMethod MoveMethod     { get; private set; }

    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(contactCollider, "contactCollider != null");

        EventReceiver();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState.Value != GameState.InGame)
            return;

        if (GimmickUtility.IsContactedToPlayer(contactCollider))
        {
            // 接触イベント発行
            GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnPushGimmickContacted.Get(MoveMethod, moveSpeedInSec));
        }

        if (IsMoving)
        {
            UpdateTransform();
        }
    }

    private void EventReceiver()
    {
        this.OnBecameInvisibleAsObservable()
            .Subscribe(_ => OnInvisible())
            .AddTo(this);
    }

    private void OnInvisible()
    {
        IsMoving = false;

        // プールへの返却イベント発行
        GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnReturnGimmickRequest.Get(GimmickType.UFO, gameObject));
    }

    public void InitializeOnGenerate(GimmickInitializeOptions options, Vector2 position)
    {
        // 座標設定
        transform.position = position;

        _baseXPosToMovement = position.x;

        MoveMethod = options.MoveMethod;

        // スコア変動に応じてギミックの移動処理発火するタイミング監視を購読
        _scoreObserveDisposable.Disposable = GimmickUtility.ReachToBeginMovePositionDisposable(transform, beginMoveRange, () =>
        {
            IsMoving = true;

            _scoreObserveDisposable.Disposable.Dispose();
            GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnBeginGimmickMovement.Get(GimmickType.UFO));
        });
    }

    public void UpdateTransform()
    {
        // 座標更新
        // x: sin波によるゆらゆら
        float freq  = 1f / swingSpeedInSec;
        float width = 1f / swingWidth;
        float sin   = Mathf.Sin(2 * Mathf.PI * freq * Time.time) / width;

        Vector3 newPos = GimmickUtility.GetNextPositionDelta(MoveMethod, moveSpeedInSec);
        newPos.x           =  _baseXPosToMovement + sin;
        newPos.y           += transform.position.y;
        transform.position =  newPos;
    }
}
