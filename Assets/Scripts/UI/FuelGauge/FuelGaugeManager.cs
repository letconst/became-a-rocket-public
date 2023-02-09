using System.Collections.Generic;
using UniRx;
using UnityEngine;

public sealed class FuelGaugeManager : SingletonMonoBehaviour<FuelGaugeManager>
{
    [SerializeField, Header("燃料ゲージの最大値")]
    private float gaugeMaxValue;

    [SerializeField, Header("燃料ゲージの最小値")]
    private float gaugeMinValue;

    [SerializeField, Header("1秒間に減少するゲージの量")]
    private float gaugeDecreaseAmountPerSec;

    [SerializeField, Header("燃料ゲージのオーバーレイが動く速度 (秒間)")]
    private float gaugeOverlayAnimationSpeedInSec;

    [SerializeField, Header("ゲージの残量ごとの設定")]
    private FuelGaugeRatioSettings[] gaugeRatioSettings;

    private readonly ReactiveProperty<float> _fuelAmountRP = new();

    /// <summary>燃料ゲージの最大値</summary>
    public float GaugeMaxValue => gaugeMaxValue;

    /// <summary>燃料ゲージの最小値</summary>
    public float GaugeMinValue => gaugeMinValue;

    /// <summary>ゲージの残量ごとの設定</summary>
    public IReadOnlyList<FuelGaugeRatioSettings> FuelGaugeRatioSettingsList => gaugeRatioSettings;

    /// <summary>燃料ゲージ残量のReactiveProperty</summary>
    public IReadOnlyReactiveProperty<float> FuelAmountRP => _fuelAmountRP;

    private void Start()
    {
        _fuelAmountRP.Value = gaugeMaxValue;

        // 設定配列を比率に応じて降順に
        System.Array.Sort(gaugeRatioSettings, static (a, b) => b.Ratio.CompareTo(a.Ratio));

        EventReceiver();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState.Value == GameState.InGame)
        {
            DecreaseFuel();
        }
    }

    private void EventReceiver()
    {
        // プレイヤーの雨雲との接触イベント受付
        GameManager.Instance.PlayerBroker
                   .Receive<GameEvent.Player.OnRainCloudContacted>()
                   .Subscribe(OnRainCloudContacted)
                   .AddTo(this);
    }

    private void OnRainCloudContacted(GameEvent.Player.OnRainCloudContacted data)
    {
        // 加算後の値をclamping
        float healingAmount = Mathf.Clamp(_fuelAmountRP.Value + data.HealAmount, gaugeMinValue, gaugeMaxValue);

        _fuelAmountRP.Value = healingAmount;
    }

    /// <summary>
    /// 燃料ゲージを減少させる
    /// </summary>
    private void DecreaseFuel()
    {
        _fuelAmountRP.Value -= gaugeDecreaseAmountPerSec * Time.deltaTime;

        // 燃料が0になったら専用イベント発行
        if (_fuelAmountRP.Value <= 0)
        {
            GameManager.Instance.GameBroker.Publish(GameEvent.OnFuelEmptied.Get());
        }
    }

    /// <summary>
    /// 燃料ゲージ残量のクランピングした値を取得する
    /// </summary>
    /// <returns></returns>
    public float GetClampedAmount()
    {
        return Mathf.Clamp(_fuelAmountRP.Value, gaugeMinValue, gaugeMaxValue);
    }

    /// <summary>
    /// オーバーレイ画像のUVオフセット値の加算量を取得する
    /// </summary>
    /// <returns></returns>
    public float GetOverlayDeltaOffset()
    {
        return Time.deltaTime * gaugeOverlayAnimationSpeedInSec;
    }
}
