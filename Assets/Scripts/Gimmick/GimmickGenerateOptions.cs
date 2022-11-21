using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初期化時のx座標
/// </summary>
public enum GimmickXPosition
{
    /// <summary>中央</summary>
    Center,

    /// <summary>ランダム</summary>
    Random,

    /// <summary>画面左の外側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    LeftOutside,

    /// <summary>画面左の内側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    LeftInside,

    /// <summary>画面右の外側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    RightOutside,

    /// <summary>画面右の内側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    RightInside,
}

/// <summary>
/// 初期化時のy座標
/// </summary>
public enum GimmickYPosition
{
    /// <summary>生成判定が行われた高度</summary>
    SameAltitude,

    /// <summary>ランダム。廃止予定</summary>
    [System.Obsolete]
    Random,

    /// <summary>画面上の外側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    TopOutside,

    /// <summary>画面上の内側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    TopInside,

    /// <summary>画面下の外側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    BottomOutside,

    /// <summary>画面下の内側。位置は、画面端から一定割合離れた場所となる (<see cref="GimmickUtility.PositionOffsetRatio"/>を参照)</summary>
    BottomInside,
}

/// <summary>
/// 移動方法
/// </summary>
public enum GimmickMoveMethod
{
    /// <summary>移動しない</summary>
    None,

    /// <summary>上方向への移動</summary>
    ToTop,

    /// <summary>右上方向への移動</summary>
    ToTopRight,

    /// <summary>右方向への移動</summary>
    ToRight,

    /// <summary>右下方向への移動</summary>
    ToBottomRight,

    /// <summary>下方向への移動</summary>
    ToBottom,

    /// <summary>左下方向への移動</summary>
    ToBottomLeft,

    /// <summary>左方向への移動</summary>
    ToLeft,

    /// <summary>左上方向への移動</summary>
    ToTopLeft,
}

[System.Serializable]
public sealed class GimmickInitializeOptions
{
    [SerializeField, Header("初期化時のx座標")]
    private GimmickXPosition xPos;

    [SerializeField, Header("初期化時のy座標")]
    private GimmickYPosition yPos;

    [SerializeField, Header("移動方法")]
    private GimmickMoveMethod moveMethod;

    /// <summary>初期化時のx座標</summary>
    public GimmickXPosition XPosition => xPos;

    /// <summary>初期化時のy座標</summary>
    public GimmickYPosition YPosition => yPos;

    /// <summary>移動方法</summary>
    public GimmickMoveMethod MoveMethod => moveMethod;
}

[System.Serializable]
public sealed class GimmickGenerateOptions
{
    [HideInInspector]
    public string name;

    // TODO: エディタ拡張として、enumのように選択させたい
    [SerializeField, Header("生成対象のギミック")]
    private MasterGimmick target;

    /// <summary>生成対象のギミック</summary>
    public MasterGimmick Target => target;

    [SerializeField, Header("生成頻度 (%)"), Range(0, 100)]
    private float generationFrequency;

    /// <summary>生成頻度</summary>
    public float GenerationFrequency => generationFrequency / 100;

    [SerializeField, Header("生成の最小間隔")]
    private int generationMinInterval;

    /// <summary>生成の最小間隔</summary>
    public int GenerationMinInterval => generationMinInterval;

    [SerializeField, Header("生成の最大間隔")]
    private int generationMaxInterval;

    /// <summary>生成の最大間隔</summary>
    public int GenerationMaxInterval => generationMaxInterval;

    [SerializeField, Header("初期化情報")]
    private GimmickInitializeOptions[] initializeOptions;

    /// <summary>初期化情報</summary>
    public IReadOnlyList<GimmickInitializeOptions> InitializeOptions => initializeOptions;
}
