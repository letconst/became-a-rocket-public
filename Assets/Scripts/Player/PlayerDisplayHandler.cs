using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// ギャー君の表示制御を行うクラス
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public sealed class PlayerDisplayHandler : MonoBehaviour
{
    private SpriteRenderer   _selfRenderer;
    private FuelGaugeManager _fuelGaugeManager;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _selfRenderer     = GetComponent<SpriteRenderer>();
        _fuelGaugeManager = FuelGaugeManager.Instance;

        Assert.IsNotNull(_fuelGaugeManager, "_fuelGaugeManager != null");

        // 燃料ゲージ残量の変化イベントを受付
        _fuelGaugeManager.FuelAmountRP.Subscribe(OnFuelAmountChanged).AddTo(this);
    }

    private void OnFuelAmountChanged(float newAmount)
    {
        // 設定する値のclamping
        newAmount = _fuelGaugeManager.GetClampedAmount();

        FuelGaugeRatioSettings targetSettings = null;

        // 残量に応じてギャーくんの見た目を変える
        foreach (FuelGaugeRatioSettings settings in _fuelGaugeManager.FuelGaugeRatioSettingsList)
        {
            if (settings.Ratio < newAmount)
                continue;

            targetSettings = settings;
        }

        _selfRenderer.sprite = targetSettings?.GyaarSprite;
    }
}
