using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ギミック「カラス」の動作処理を行うクラス
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public sealed class CrowHandler : GimmickHandlerBase, IInitializableGimmick, IMovableGimmick
{
    [SerializeField, Header("接触処理を行うCollider")]
    private Collider2D contactCollider;

    [SerializeField, Header("移動速度 (秒)")]
    private float moveSpeedInSec;

    [SerializeField, Header("移動処理が開始されるプレイヤーとの距離")]
    private float beginMoveRange;

    private SpriteRenderer _selfRenderer;

    private readonly SerialDisposable _scoreObserveDisposable = new();

    private SpriteRenderer SelfRenderer
    {
        get
        {
            if (!_selfRenderer)
            {
                _selfRenderer = GetComponent<SpriteRenderer>();
            }

            return _selfRenderer;
        }
    }

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
            GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnPushGimmickContacted.Get(MoveMethod, MoveSpeedInSec));
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
#if UNITY_EDITOR
        // エディターでプレイ終了時にもイベント発行され、GMインスタンスが消えるので存在チェック
        if (!GameManager.Instance)
            return;
#endif

        IsMoving = false;

        // プールへの返却イベント発行
        GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnReturnGimmickRequest.Get(GimmickType.Crow, gameObject));
    }

    public void InitializeOnGenerate(GimmickInitializeOptions options, Vector2 position)
    {
        // 移動方向選択
        MoveMethod = options.MoveMethod;

        // 移動方向に応じてSpriteを反転するか判定
        bool isFlip = MoveMethod switch
        {
            GimmickMoveMethod.ToLeft  => false,
            GimmickMoveMethod.ToRight => true
        };

        SelfRenderer.flipX = isFlip;

        // 接触判定colliderも反転を反映
        contactCollider.transform.localScale = new Vector3(isFlip ? -1f : 1f, 1f, 1f);

        // 座標設定
        transform.position = position;

        // スコア変動に応じてギミックの移動処理発火するタイミング監視を購読
        _scoreObserveDisposable.Disposable = GimmickUtility.ReachToBeginMovePositionDisposable(transform, beginMoveRange, () =>
        {
            IsMoving = true;

            _scoreObserveDisposable.Disposable.Dispose();
            GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnBeginGimmickMovement.Get(GimmickType.Crow));
        });
    }

    public void UpdateTransform()
    {
        transform.position += GimmickUtility.GetNextPositionDelta(MoveMethod, MoveSpeedInSec);
    }
}
