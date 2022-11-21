using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// ギミック用のユーティリティクラス
/// </summary>
public static class GimmickUtility
{
    private static Camera _cam;

    private static Camera Cam
    {
        get
        {
            if (_cam)
                return _cam;

            return _cam = Camera.main;
        }
    }

    private static Vector2 _screenLength;

    private static Vector2 ScreenLength
    {
        get
        {
            if (_screenLength != Vector2.zero)
                return _screenLength;

            return _screenLength = VectorUtility.GetScreenLength(Cam);
        }
    }

    /// <summary>
    /// 画面外に生成されるギミック位置の、画面端からの割合 (内側外側に適用)
    /// </summary>
    private static readonly float PositionOffsetRatio = .2f;

    /// <summary>
    /// 指定の値の範囲の乱数を取得する
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public static float GetRandomRange(float range)
    {
        return Random.Range(-range, range);
    }

    /// <summary>
    /// 確率の判定を行う
    /// </summary>
    /// <param name="frequency">確率 (0-1)</param>
    /// <returns>判定結果</returns>
    public static bool JudgeProbability(float frequency)
    {
        // 確率が0ならそのままfalse
        if (Mathf.Approximately(frequency, 0f))
            return false;

        // 1ならそのままtrue
        if (Mathf.Approximately(frequency, 1f))
            return true;

        float rate = Random.value;

        if (rate < frequency)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 指定の<see cref="GimmickGenerateOptions.XPosition"/>に対応するx座標を取得する
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static float GetXPosition(GimmickXPosition targetPos)
    {
        // TODO: 残りの値の用意
        return targetPos switch
        {
            GimmickXPosition.Center       => 0f,
            GimmickXPosition.Random       => GetRandomRange(ScreenLength.x / 2f - ScreenLength.x * PositionOffsetRatio),
            GimmickXPosition.LeftOutside  => -(ScreenLength.x / 2f) - ScreenLength.x * PositionOffsetRatio,
            GimmickXPosition.LeftInside   => -(ScreenLength.x / 2f) + ScreenLength.x * PositionOffsetRatio,
            GimmickXPosition.RightOutside => ScreenLength.x / 2f + ScreenLength.x * PositionOffsetRatio,
            GimmickXPosition.RightInside  => ScreenLength.x / 2f - ScreenLength.x * PositionOffsetRatio,
        };
    }

    /// <summary>
    /// 指定の<see cref="GimmickGenerateOptions.YPosition"/>に対応するy座標を取得する
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static float GetYPosition(GimmickYPosition targetPos)
    {
        // TODO: 残りの値の用意
        return targetPos switch
        {
            GimmickYPosition.SameAltitude  => 0f,
            GimmickYPosition.Random        => 0f,
            GimmickYPosition.TopOutside    => 0f,
            GimmickYPosition.TopInside     => 0f,
            GimmickYPosition.BottomOutside => 0f,
            GimmickYPosition.BottomInside  => 0f,
            _                              => 0f
        };
    }

    /// <summary>
    /// 次フレームへの移動後の差分座標を取得する
    /// </summary>
    /// <param name="moveMethod">移動方法</param>
    /// <param name="moveSpeed">移動速度</param>
    /// <returns></returns>
    public static Vector3 GetNextPositionDelta(GimmickMoveMethod moveMethod, float moveSpeed)
    {
        float moveDelta = Time.deltaTime * moveSpeed;

        // TODO: 残りの値の用意
        Vector3 deltaPos = moveMethod switch
        {
            GimmickMoveMethod.None          => Vector3.zero,
            GimmickMoveMethod.ToTop         => new Vector3(0f, moveDelta),
            GimmickMoveMethod.ToTopRight    => new Vector3(moveDelta / 2f, moveDelta / 2f),
            GimmickMoveMethod.ToRight       => new Vector3(moveDelta, 0f),
            GimmickMoveMethod.ToBottomRight => new Vector3(moveDelta / 2f, -moveDelta / 2f),
            GimmickMoveMethod.ToBottom      => new Vector3(0f, -moveDelta),
            GimmickMoveMethod.ToBottomLeft  => new Vector3(-moveDelta / 2f, -moveDelta / 2f),
            GimmickMoveMethod.ToLeft        => new Vector3(-moveDelta, 0f),
            GimmickMoveMethod.ToTopLeft     => new Vector3(-moveDelta / 2f, moveDelta / 2f),
            _                               => new Vector3()
        };

        return deltaPos;
    }

    /// <summary>
    /// ギミックが移動処理を始めるまでの監視への購読を行う
    /// </summary>
    /// <param name="selfTrf">ギミックのTransform</param>
    /// <param name="beginMoveRange">移動処理が開始されるプレイヤーとの距離</param>
    /// <param name="onReached">移動処理を開始する位置となった際の処理</param>
    /// <returns></returns>
    public static System.IDisposable ReachToBeginMovePositionDisposable(Transform selfTrf, float beginMoveRange,
                                                                        System.Action onReached)
    {
        return ScoreManager.Instance.CurrentScoreChanged
                           .TakeWhile(_ => selfTrf.gameObject.activeSelf)
                           .DistinctUntilChanged()
                           .Subscribe(newScore =>
                           {
                               float distanceToPlayer = selfTrf.position.y - newScore;

                               if (distanceToPlayer <= beginMoveRange)
                               {
                                   onReached?.Invoke();
                               }
                           });
    }

    /// <summary>
    /// 指定のColliderがプレイヤーと接触しているかを取得する
    /// </summary>
    /// <param name="gimmickCollider">ギミックのCollider</param>
    /// <returns>プレイヤーと接触しているか</returns>
    public static bool IsContactedToPlayer(Collider2D gimmickCollider)
    {
        List<Collider2D> contactResult = new();

        Physics2D.OverlapCollider(gimmickCollider, GameConstants.PlayerContactFilter, contactResult);

        return contactResult.Count > 0;
    }
}
