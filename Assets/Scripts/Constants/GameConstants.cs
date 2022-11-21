using UnityEngine;

public static class GameConstants
{
    // TODO: マスターデータとして管理
    public static readonly float RotateSpeedByWind = 150f;

    public static readonly float MoveSpeedByWind = 5f;

    public static readonly int BackgroundOrderInLayer = -2;

    public static readonly ContactFilter2D PlayerContactFilter = new()
    {
        useTriggers  = true,
        useLayerMask = true,
        layerMask    = LayerConstants.Player
    };

    public static readonly string ScoreFormat = "{0} M";

    public static readonly string LocalRankingKey = "LOCAL_RANKING";
}
