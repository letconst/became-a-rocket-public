/// <summary>
/// ゲーム中のイベントの定義用クラス
/// </summary>
public static class GameEvent
{
    /// <summary>
    /// プレイヤー関連のイベント用クラス
    /// </summary>
    public static class Player
    {
        /// <summary>ギミックのオブジェクトプールへの返却リクエストイベント</summary>
        public sealed class OnReturnGimmickRequest :
            EventMessage<OnReturnGimmickRequest, GimmickType, UnityEngine.GameObject>
        {
            /// <summary>プールへ戻すギミックの種類</summary>
            public GimmickType Type => param1;

            /// <summary>プールへ戻すギミックのGameObject</summary>
            public UnityEngine.GameObject ObjectToReturn => param2;
        }

        /// <summary>
        /// 移動系ギミックが移動し始めた際のイベント
        /// </summary>
        public sealed class OnBeginGimmickMovement : EventMessage<OnBeginGimmickMovement, GimmickType>
        {
            /// <summary>ギミックの種類</summary>
            public GimmickType Type => param1;
        }

        /// <summary>風との接触時のイベント</summary>
        public sealed class OnWindContacted : EventMessage<OnWindContacted, WindHandler.WindDirection, float, float>
        {
            /// <summary>風向き</summary>
            public WindHandler.WindDirection Direction => param1;

            /// <summary>回転速度</summary>
            public float RotateSpeed => param2;

            /// <summary>左右への移動速度</summary>
            public float MoveSpeed => param3;
        }

        /// <summary>雨雲との接触時のイベント</summary>
        public sealed class OnRainCloudContacted : EventMessage<OnRainCloudContacted, float>
        {
            /// <summary>燃料の回復量</summary>
            public float HealAmount => param1;
        }

        /// <summary>押される障害物ギミックとの接触時のイベント</summary>
        public sealed class OnPushGimmickContacted : EventMessage<OnPushGimmickContacted, GimmickMoveMethod, float>
        {
            /// <summary>移動方向</summary>
            public GimmickMoveMethod MoveMethod => param1;

            /// <summary>移動速度 (秒)</summary>
            public float MoveSpeedInSec => param2;
        }

        /// <summary>隕石との接触時のイベント</summary>
        public sealed class OnMeteorContacted : EventMessage<OnMeteorContacted, GimmickMoveMethod, float, UnityEngine.Vector3>
        {
            /// <summary>移動方向</summary>
            public GimmickMoveMethod MoveMethod => param1;

            /// <summary>移動速度 (秒)</summary>
            public float MoveSpeedInSec => param2;

            /// <summary>隕石の座標</summary>
            public UnityEngine.Vector3 Position => param3;
        }

        /// <summary>場外に出た際のイベント</summary>
        public sealed class OnOutOfField : EventMessage<OnOutOfField>
        {
        }
    }

    public static class Input
    {
        /// <summary>回転</summary>
        public sealed class OnRotateRequest : EventMessage<OnRotateRequest, PlayerConstants.RotateDirection, float>
        {
            /// <summary>回転方向</summary>
            public PlayerConstants.RotateDirection Direction => param1;

            /// <summary>回転速度</summary>
            public float RotateSpeed => param2;
        }
    }

    /// <summary>スコア変更リクエストのイベント</summary>
    public sealed class ScoreChangeRequest : EventMessage<ScoreChangeRequest, int>
    {
        /// <summary>変更後のスコア</summary>
        public int NewScore => param1;
    }

    /// <summary>燃料ゲージが0になった際のイベント</summary>
    public sealed class OnFuelEmptied : EventMessage<OnFuelEmptied>
    {
    }

    /// <summary>ゲームステート変更要求のイベント</summary>
    public sealed class OnStateChangeRequest :
        EventMessage<OnStateChangeRequest, GameState, System.Action, System.Action>
    {
        /// <summary>変更したいステート</summary>
        public GameState ToChangeState => param1;

        /// <summary>ステートが無事変更された際のコールバック</summary>
        public System.Action OnAccepted => param2;

        /// <summary>ステートが変更されなかった際のコールバック</summary>
        public System.Action OnRejected => param3;
    }

    /// <summary>ゲームの初期化が完了した際のイベント</summary>
    public sealed class OnGameInitialized : EventMessage<OnGameInitialized>
    {
    }
}
