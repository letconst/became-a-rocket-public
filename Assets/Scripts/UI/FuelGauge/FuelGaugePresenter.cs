using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(FuelGaugeView))]
public sealed class FuelGaugePresenter : MonoBehaviour
{
    private FuelGaugeView    _view;
    private FuelGaugeManager _model;

    private void Start()
    {
        _view  = GetComponent<FuelGaugeView>();
        _model = FuelGaugeManager.Instance;

        Assert.IsNotNull(_view, "view != null");
        Assert.IsNotNull(_model, "model != null");

        // ゲージの初期値設定
        _view.SetGaugeMaxValue(_model.GaugeMaxValue);
        _view.SetGaugeMinValue(_model.GaugeMinValue);
        _view.SetGaugeValue(_model.GaugeMaxValue);

        // 残燃料の変更をviewに反映
        _model.FuelAmountRP.Subscribe(OnFuelAmountChanged).AddTo(this);
    }

    private void OnFuelAmountChanged(float newAmount)
    {
        Assert.IsNotNull(_view, "view != null");
        Assert.IsNotNull(_model, "model != null");

        // 設定する値のclamping
        newAmount = _model.GetClampedAmount();

        // ゲージ残量設定
        _view.SetGaugeValue(newAmount);

        // 残量に応じて色を変化させる
        FuelGaugeRatioSettings targetSettings = null;

        foreach (FuelGaugeRatioSettings settings in _model.FuelGaugeRatioSettingsList)
        {
            if (settings.Ratio < newAmount)
                continue;

            targetSettings = settings;
        }

        _view.SetGaugeColor(targetSettings.GaugeColor);
    }
}
