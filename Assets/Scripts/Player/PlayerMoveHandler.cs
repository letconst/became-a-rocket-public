using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class PlayerMoveHandler : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float flySpeed;

    [SerializeField, Header("X方向の場外に出たとみなす領域の緩和値 (m)")]
    private float outOfFieldJudgeOffset;

    private int _originYPos;

    private readonly Dictionary<GimmickMoveMethod, float> _additionalMoveMethods = new();

    private readonly PlayerMoveVector _moveVector = new();

    private float _horizontalScreenHalfLength;

    private Camera _cam;

    private void Start()
    {
        PlayerStatusManager statusManager = PlayerStatusManager.Instance;
        _cam = Camera.main;

        Assert.IsNotNull(statusManager, "statusManager != null");

        statusManager.InitialPosition = transform.position;
        _originYPos                   = (int) transform.position.y;
        _horizontalScreenHalfLength   = VectorUtility.GetScreenLength(Camera.main).x / 2f;

        _moveVector.AddTo(this);

        EventReceiver();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState.Value == GameState.InGame)
        {
            UpdatePosition();
        }
    }

    private void EventReceiver()
    {
        GameManager gameManager = GameManager.Instance;

        Assert.IsNotNull(gameManager, "gameManager != null");

        // 回転入力受付
        gameManager.InputBroker.Receive<GameEvent.Input.OnRotateRequest>()
                   .Subscribe(OnRotateRequest)
                   .AddTo(this);

        // 風との接触処理
        gameManager.PlayerBroker.Receive<GameEvent.Player.OnWindContacted>()
                   .Subscribe(OnWindContacted)
                   .AddTo(this);

        // 押されるギミックとの接触処理
        gameManager.PlayerBroker.Receive<GameEvent.Player.OnPushGimmickContacted>()
                   .Subscribe(OnPushGimmickContacted)
                   .AddTo(this);

        // 隕石との接触処理
        gameManager.PlayerBroker.Receive<GameEvent.Player.OnMeteorContacted>()
                   .Subscribe(OnMeteorContacted)
                   .AddTo(this);

        // y座標の変更をスコアに反映通知
        this.ObserveEveryValueChanged(static self => self.transform.position.y)
            .Where(_ => gameManager.CurrentState.Value == GameState.InGame)
            .Select(y => (int) y - _originYPos)
            .DistinctUntilChanged()
            .Subscribe(OnYPositionChanged)
            .AddTo(this);

        // x座標が一定を超えていたら場外イベント発行
        this.ObserveEveryValueChanged(static self => self.transform.position.x)
            .Where(_ => gameManager.CurrentState.Value == GameState.InGame)
            .DistinctUntilChanged()
            .Subscribe(OnXPositionChanged)
            .AddTo(this);
    }

    private void UpdatePosition()
    {
        // 飛行移動の差分座標
        _moveVector.BaseVector = transform.up * (flySpeed * Time.deltaTime);

        AddDeltaPosition(_moveVector.CurrentVector);

        _moveVector.Clear();
    }

    private void OnRotateRequest(GameEvent.Input.OnRotateRequest data)
    {
        Rotate(data.RotateSpeed);
    }

    private void OnWindContacted(GameEvent.Player.OnWindContacted data)
    {
        float rotateSpeed = data.Direction switch
        {
            WindHandler.WindDirection.Left  => data.RotateSpeed,
            WindHandler.WindDirection.Right => -data.RotateSpeed
        };

        Vector3 moveDir = data.Direction switch
        {
            WindHandler.WindDirection.Left  => -Vector3.right,
            WindHandler.WindDirection.Right => Vector3.right
        };

        AddPosition(moveDir, data.MoveSpeed);
        Rotate(rotateSpeed);
    }

    private void OnPushGimmickContacted(GameEvent.Player.OnPushGimmickContacted data)
    {
        _moveVector.AddVectorByGimmick(data.MoveMethod, data.MoveSpeedInSec);
    }

    private void OnMeteorContacted(GameEvent.Player.OnMeteorContacted data)
    {
        Vector3 vectorToAdd = Vector3.zero;

        Vector3 meteorVector = GimmickUtility.GetNextPositionDelta(data.MoveMethod, data.MoveSpeedInSec);

        // 隕石の進行方向から見たプレイヤーへの角度を取得
        Vector3 dirFromMeteor = transform.position - data.Position;
        float   angleFromMeteor = Vector2.SignedAngle(dirFromMeteor, meteorVector.normalized);

        // 角度に応じて押し出す方向を選択
        if (angleFromMeteor > 0f)
        {
            if (data.MoveMethod == GimmickMoveMethod.ToBottomLeft)
            {
                vectorToAdd.x = meteorVector.x;
            }

            if (data.MoveMethod == GimmickMoveMethod.ToBottomRight)
            {
                vectorToAdd.y = meteorVector.y;
            }
        }
        else
        {
            if (data.MoveMethod == GimmickMoveMethod.ToBottomLeft)
            {
                vectorToAdd.y = meteorVector.y;
            }

            if (data.MoveMethod == GimmickMoveMethod.ToBottomRight)
            {
                vectorToAdd.x = meteorVector.x;
            }
        }

        _moveVector.AddVectorByGimmick(vectorToAdd);
    }

    private void OnYPositionChanged(int y)
    {
        // yの変更をScoreManagerに通知
        GameManager.Instance.GameBroker.Publish(GameEvent.ScoreChangeRequest.Get(y));

        ScoreManager scoreManager = ScoreManager.Instance;

        // yが最大スコアを下回っていたら (下降中)
        if (y < scoreManager.MaxScore)
        {
            float screenY   = VectorUtility.GetScreenLength(_cam).y;
            int   scoreDiff = scoreManager.MaxScore - y;

            // プレイヤーのy座標が "最大スコアとの差分 - 画面縦半分 - オフセット" を超えていたら場外イベント発行
            if (scoreDiff - screenY / 2f - outOfFieldJudgeOffset > 0f)
            {
                GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnOutOfField.Get());
            }
        }
    }

    private void OnXPositionChanged(float x)
    {
        float absPlayerXPos = Mathf.Abs(x);

        // プレイヤーのx座標が "画面のx + オフセット" を超えたら場外に出たとみなし、イベント発行
        if (absPlayerXPos > _horizontalScreenHalfLength + outOfFieldJudgeOffset)
        {
            GameManager.Instance.PlayerBroker.Publish(GameEvent.Player.OnOutOfField.Get());
        }
    }

    /// <summary>
    /// 自身の座標に指定の方向ベクトルと速度との計算結果を加算し、座標更新を行う
    /// </summary>
    /// <param name="direction">加算する座標の方向ベクトル</param>
    /// <param name="speed">加算する座標の速度 (大きさ)</param>
    private void AddPosition(Vector3 direction, float speed)
    {
        transform.position += direction * (speed * Time.deltaTime);
    }

    /// <summary>
    /// 自身の座標に指定の差分座標を加算し、座標更新を行う
    /// </summary>
    /// <param name="deltaPosition">加算する差分座標</param>
    private void AddDeltaPosition(Vector3 deltaPosition)
    {
        transform.position += deltaPosition;
    }

    private void Rotate(float rotateSpeed)
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
