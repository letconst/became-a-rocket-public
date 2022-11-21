using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public sealed class FuelGaugeView : MonoBehaviour
{
    [SerializeField]
    private Slider gaugeSlider;

    [SerializeField]
    private Image gaugeFillImage;

    /// <summary>
    /// ゲージの値が変更されたことを通知するObserver
    /// </summary>
    public System.IObservable<float> OnGaugeValueChanged
    {
        get
        {
            Assert.IsNotNull(gaugeSlider, "gaugeSlider != null");

            return gaugeSlider.OnValueChangedAsObservable();
        }
    }

    /// <summary>
    /// 燃料ゲージの色を設定する
    /// </summary>
    /// <param name="newColor">設定する色</param>
    public void SetGaugeColor(in Color newColor)
    {
        Assert.IsNotNull(gaugeFillImage, "gaugeFillImage != null");

        gaugeFillImage.color = newColor;
    }

    /// <summary>
    /// 燃料ゲージUIの値を設定する
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetGaugeValue(float value)
    {
        Assert.IsNotNull(gaugeSlider, "gaugeSlider != null");

        gaugeSlider.value = value;
    }

    /// <summary>
    /// 燃料ゲージの最大値を設定する
    /// </summary>
    /// <param name="maxValue">最大値</param>
    public void SetGaugeMaxValue(float maxValue)
    {
        Assert.IsNotNull(gaugeSlider, "gaugeSlider != null");

        gaugeSlider.maxValue = maxValue;
    }

    /// <summary>
    /// 燃料ゲージの最小値を設定する
    /// </summary>
    /// <param name="minValue">最小値</param>
    public void SetGaugeMinValue(float minValue)
    {
        Assert.IsNotNull(gaugeSlider, "gaugeSlider != null");

        gaugeSlider.minValue = minValue;
    }
}
