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

    [SerializeField]
    private RawImage gaugeOverlayRawImage;

    [SerializeField]
    private Animation blinkAnimation;

    /// <summary>
    /// 現在のゲージスライダーの値
    /// </summary>
    public float GaugeSliderValue
    {
        get
        {
            Assert.IsNotNull(gaugeSlider, "gaugeSlider != null");

            return gaugeSlider.value;
        }
    }

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

    /// <summary>
    /// オーバーレイ画像のUVオフセット値を加算する
    /// </summary>
    /// <param name="newOffset">加算する量</param>
    public void AddGaugeOverlayOffset(Vector2 newOffset)
    {
        Assert.IsNotNull(gaugeOverlayRawImage, "gaugeOverlayImage != null");

        Rect uvRect = gaugeOverlayRawImage.uvRect;
        uvRect.x                    = Mathf.Repeat(uvRect.x + newOffset.x, 1);
        uvRect.y                    = Mathf.Repeat(uvRect.y + newOffset.y, 1);
        gaugeOverlayRawImage.uvRect = uvRect;
    }

    /// <summary>
    /// アウトラインの点滅アニメーションの再生状態を設定する
    /// </summary>
    /// <param name="isAnimate"></param>
    public void SetBlinkOutlineAnimationStatus(bool isAnimate)
    {
        if (isAnimate)
        {
            blinkAnimation.Play();
        }
        else
        {
            blinkAnimation.Stop();

            // アウトラインをリセット
            blinkAnimation.clip.SampleAnimation(blinkAnimation.gameObject, blinkAnimation.clip.length / 2f);
        }
    }
}
