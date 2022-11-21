using UnityEngine;

/// <summary>
/// ゲームで使用されるレイヤーマスクを定義したクラス
/// </summary>
public static class LayerConstants
{
    public static readonly int Default = 1 << LayerMask.NameToLayer("Default");

    public static readonly int Player = 1 << LayerMask.NameToLayer("Player");
}
