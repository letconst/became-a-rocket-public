/// <summary>
/// ギミックの初期化処理を定義するためのInterface
/// </summary>
public interface IInitializableGimmick
{
    /// <summary>
    /// 生成時の初期化処理
    /// </summary>
    /// <param name="options">生成オプション</param>
    /// <param name="position">生成予定の座標</param>
    public void InitializeOnGenerate(GimmickInitializeOptions options, UnityEngine.Vector2 position);
}
