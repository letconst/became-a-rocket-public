public enum GameState
{
    None,

    /// <summary>ゲーム開始準備</summary>
    Ready,

    /// <summary>ゲーム中</summary>
    InGame,

    /// <summary>ゲームが終了し、リザルトへ遷移するまで</summary>
    Finish,
}
