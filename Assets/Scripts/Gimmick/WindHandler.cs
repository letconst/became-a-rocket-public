using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

public sealed partial class WindHandler : GimmickHandlerBase, IInitializableGimmick
{
    public enum WindDirection
    {
        Left,
        Right,
    }

    [SerializeField]
    private WindDirection windDirection;

    [SerializeField]
    private SpriteRenderer windSprite;

#if UNITY_EDITOR
    [SerializeField]
    private bool isOverrideRotateSpeed;

    [SerializeField]
    private float overrideRotateSpeed;

    [SerializeField]
    private bool isOverrideMoveSpeed;

    [SerializeField]
    private float overrideMoveSpeed;
#endif

    private float _posFromCenter;

    private float RotateSpeed
    {
        get
        {
#if UNITY_EDITOR
            if (isOverrideRotateSpeed)
            {
                return overrideRotateSpeed;
            }
#endif

            return GameConstants.RotateSpeedByWind;
        }
    }

    private float MoveSpeed
    {
        get
        {
#if UNITY_EDITOR
            if (isOverrideMoveSpeed)
            {
                return overrideMoveSpeed;
            }
#endif

            return GameConstants.MoveSpeedByWind;
        }
    }

    private void Awake()
    {
        Setup();
    }

    protected override void Start()
    {
        base.Start();

        EventReceiver();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState.Value != GameState.InGame)
            return;

        // プレイヤーと接触したらイベント発行
        if (GimmickUtility.IsContactedToPlayer(SelfCollider))
        {
            GameManager.Instance.PlayerBroker.Publish(
                GameEvent.Player.OnWindContacted.Get(windDirection, RotateSpeed, MoveSpeed));
        }
    }

    private void Setup()
    {
        Assert.IsNotNull(windSprite, "windSprite != null");

        _posFromCenter = Mathf.Abs(windSprite.transform.position.x);

        // 風向きに応じて画像の反転設定
        windSprite.flipX = GetWindDirectionFlip(windDirection);
    }

    /// <summary>
    /// 指定の風向きへのフリップboolを取得する
    /// </summary>
    /// <param name="dir">風向き</param>
    /// <returns></returns>
    private bool GetWindDirectionFlip(WindDirection dir)
    {
        return dir switch
        {
            WindDirection.Left  => true,
            WindDirection.Right => false,
        };
    }

    private void EventReceiver()
    {
        windSprite.OnBecameInvisibleAsObservable()
                  .Where(_ => gameObject.activeSelf)
                  .Subscribe(_ => OnInvisible())
                  .AddTo(this);

#if UNITY_EDITOR
        // デバッグ用
        // windDirectionの値が変わったら、その向きに画像をフリップさせる
        this.ObserveEveryValueChanged(static self => self.windDirection)
            .Skip(1)
            .Subscribe(dir => windSprite.flipX = GetWindDirectionFlip(dir))
            .AddTo(this);
#endif
    }

    private void OnInvisible()
    {
        // カメラの範囲外になった際にプールへの返却要求
        GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnReturnGimmickRequest.Get(GimmickType.Wind, gameObject));
    }

    public void InitializeOnGenerate(GimmickInitializeOptions options, Vector2 position)
    {
        // 風向きをランダムに選択
        windDirection = EnumUtility.GetRandom<WindDirection>();

        Assert.IsNotNull(windSprite, "windSprite != null");

        // 選択された風向きに合わせてspriteを調整
        bool    isFlip    = GetWindDirectionFlip(windDirection);
        Vector2 spritePos = windSprite.transform.position;
        spritePos.x = isFlip ? _posFromCenter : -_posFromCenter;

        windSprite.flipX              = isFlip;
        windSprite.transform.position = spritePos;
    }
}
