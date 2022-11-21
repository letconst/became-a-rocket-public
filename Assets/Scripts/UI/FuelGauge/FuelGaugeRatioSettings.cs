using UnityEngine;

[System.Serializable]
public class FuelGaugeRatioSettings
{
    [SerializeField, Header("設定するゲージの比率 (%、これ以下に適用)")]
    private float ratio;

    /// <summary>設定するゲージの比率 (%、これ以下に適用)</summary>
    public float Ratio => ratio;

    [SerializeField, Header("ゲージの色")]
    private Color gaugeColor;

    /// <summary>ゲージの色</summary>
    public Color GaugeColor => gaugeColor;

    [SerializeField, Header("ギャーくんの見た目")]
    private Sprite gyaarSprite;

    /// <summary>ギャーくんの見た目</summary>
    public Sprite GyaarSprite => gyaarSprite;
}
