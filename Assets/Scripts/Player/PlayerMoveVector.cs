using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの移動方向を計算するクラス
/// </summary>
public sealed class PlayerMoveVector : System.IDisposable
{
    /// <summary>
    /// プレイヤーの飛行による通常移動ベクトル
    /// </summary>
    public Vector3 BaseVector;

    private Dictionary<GimmickMoveMethod, float> _addedGimmickMoveMethods = new();

    private List<Vector3> _gimmickMoveVectors = new();

    /// <summary>
    /// 現時点で追加されているベクトル情報から、最終的なベクトルを取得する
    /// </summary>
    public Vector3 CurrentVector
    {
        get
        {
            Vector3 vectorToReturn = Vector3.zero;

            // ギミックによる移動情報をベクトルに加算
            Vector3 vectorByGimmick = GetMaxVectorByGimmick();

            // ベクトルのxyを、絶対値の高いものに設定
            if (Mathf.Abs(BaseVector.x) > Mathf.Abs(vectorToReturn.x))
            {
                vectorToReturn.x = BaseVector.x;
            }

            if (Mathf.Abs(BaseVector.y) > Mathf.Abs(vectorToReturn.y))
            {
                vectorToReturn.y = BaseVector.y;
            }

            // ギミックによる押し出しが優先されるため、押し出しがあれば上書き
            if (!Mathf.Approximately(vectorByGimmick.x, 0f))
            {
                vectorToReturn.x = vectorByGimmick.x;
            }

            if (!Mathf.Approximately(vectorByGimmick.y, 0f))
            {
                vectorToReturn.y = vectorByGimmick.y;
            }

            return vectorToReturn;
        }
    }

    /// <summary>
    /// ギミックによる移動ベクトルを追加する
    /// </summary>
    /// <param name="moveMethod">移動方向</param>
    /// <param name="speed">移動速度 (秒)</param>
    public void AddVectorByGimmick(GimmickMoveMethod moveMethod, float speed)
    {
        // すでに同じ方向の追加移動がある場合、速度を上書き
        if (_addedGimmickMoveMethods.ContainsKey(moveMethod))
        {
            // 既存側の速度の方が早ければ上書きしない
            if (_addedGimmickMoveMethods[moveMethod] >= speed)
                return;

            _addedGimmickMoveMethods[moveMethod] = speed;
        }
        // なければ、新規追加
        else
        {
            _addedGimmickMoveMethods.Add(moveMethod, speed);
        }
    }

    /// <summary>
    /// ギミックによる移動ベクトルを追加する
    /// </summary>
    /// <param name="vector">移動ベクトル</param>
    public void AddVectorByGimmick(in Vector3 vector)
    {
        _gimmickMoveVectors.Add(vector);
    }

    /// <summary>
    /// ギミックによる移動ベクトルの最大値を取得する。最大値は、絶対値としたもので選択される
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMaxVectorByGimmick()
    {
        Vector3 vectorToReturn = Vector3.zero;

        foreach ((GimmickMoveMethod moveMethod, float speed) in _addedGimmickMoveMethods)
        {
            Vector3 moveDelta = GimmickUtility.GetNextPositionDelta(moveMethod, speed);
            GetMaxVector(moveDelta, ref vectorToReturn);
        }

        foreach (Vector3 vector in _gimmickMoveVectors)
        {
            GetMaxVector(vector, ref vectorToReturn);
        }

        return vectorToReturn;
    }

    /// <summary>
    /// 指定のベクトルから、絶対値の大きいものとしたベクトルを取得する
    /// </summary>
    /// <param name="compareVector">比較対象</param>
    /// <param name="returnVector">結果として返されるベクトル。<see cref="compareVector"/>と比較された上で代入される</param>
    private void GetMaxVector(in Vector3 compareVector, ref Vector3 returnVector)
    {
        if (Mathf.Abs(compareVector.x) > Mathf.Abs(returnVector.x))
        {
            returnVector.x = compareVector.x;
        }

        if (Mathf.Abs(compareVector.y) > Mathf.Abs(returnVector.y))
        {
            returnVector.y = compareVector.y;
        }
    }

    /// <summary>
    /// 追加されたベクトル情報を消去する
    /// </summary>
    public void Clear()
    {
        _gimmickMoveVectors.Clear();
        _addedGimmickMoveMethods.Clear();
    }

    public void Dispose()
    {
        Clear();

        BaseVector               = Vector3.zero;
        _gimmickMoveVectors      = null;
        _addedGimmickMoveMethods = null;
    }
}
