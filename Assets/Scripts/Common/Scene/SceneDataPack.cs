/// <summary>
/// 次のシーンへデータを受け渡すための基底クラス
/// </summary>
public abstract class SceneDataPack
{
    /// <summary>
    /// 1つ前のシーン
    /// </summary>
    public abstract GameScene PrevGameGameScene { get; protected set; }

    protected SceneDataPack(GameScene prevGameScene)
    {
        PrevGameGameScene = prevGameScene;
    }
}

/// <summary>
/// メインゲームシーンへデータを受け渡すためのクラス
/// </summary>
public sealed class ToMainGameSceneDataPack : SceneDataPack
{
    public override GameScene PrevGameGameScene { get; protected set; }

    public ToMainGameSceneDataPack(GameScene prevGameScene) : base(prevGameScene)
    {
    }
}
