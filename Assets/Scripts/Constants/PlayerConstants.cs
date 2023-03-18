/// <summary>
/// プレイヤーに関する定数定義用クラス
/// </summary>
public static class PlayerConstants
{
    public enum RotateDirection
    {
        Left,
        Right,
    }

    // TODO: マスターデータとして管理
    public static readonly float RotateSpeedByInput = 450f;
}
