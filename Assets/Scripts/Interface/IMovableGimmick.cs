/// <summary>
/// 移動や回転などを行うギミックが実装するInterface
/// </summary>
public interface IMovableGimmick
{
    /// <summary>
    /// 移動速度 (秒)
    /// </summary>
    public float MoveSpeedInSec { get; }

    /// <summary>
    /// 移動を行っているか
    /// </summary>
    public bool IsMoving { get; }

    /// <summary>
    /// 移動方法
    /// </summary>
    public GimmickMoveMethod MoveMethod { get; }

    /// <summary>
    /// <see cref="UnityEngine.Transform"/>の更新処理
    /// </summary>
    public void UpdateTransform();
}
