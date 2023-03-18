using UniRx;
using UnityEngine.Assertions;

/// <summary>
/// ギミックに関するサウンドを制御するクラス
/// </summary>
public sealed class GimmickSoundHandler : System.IDisposable
{
    private SoundManager _soundManager;

    private CompositeDisposable _soundEventDisposable = new();

    public GimmickSoundHandler(GameManager gameManager, SoundManager soundManager)
    {
        Assert.IsNotNull(gameManager, "gameManager != null");
        Assert.IsNotNull(soundManager, "soundManager != null");

        _soundManager = soundManager;

        // ギミック移動開始時のイベント登録
        gameManager.PlayerBroker.Receive<GameEvent.Player.OnBeginGimmickMovement>()
                   .Subscribe(OnBeginGimmickMovement)
                   .AddTo(_soundEventDisposable);

        // 雨雲との接触時のインベント登録
        gameManager.PlayerBroker.Receive<GameEvent.Player.OnRainCloudContacted>()
                   .Subscribe(OnRainCloudContacted)
                   .AddTo(_soundEventDisposable);
    }

    private void OnBeginGimmickMovement(GameEvent.Player.OnBeginGimmickMovement data)
    {
        SoundDef seToPlay = data.Type switch
        {
            GimmickType.Crow   => SoundDef.CrowBegin,
            GimmickType.Meteor => SoundDef.MeteorBegin,
            GimmickType.UFO    => SoundDef.UFOBegin
        };

        _soundManager.PlaySound(seToPlay);
    }

    private void OnRainCloudContacted(GameEvent.Player.OnRainCloudContacted data)
    {
        _soundManager.PlaySound(SoundDef.RainCloudAqcuire);
    }

    public void Dispose()
    {
        _soundEventDisposable.Dispose();

        _soundManager         = null;
        _soundEventDisposable = null;
    }
}
