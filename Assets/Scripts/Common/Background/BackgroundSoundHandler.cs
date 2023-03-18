using Cysharp.Threading.Tasks;
using UniRx;

/// <summary>
/// 背景画像に関するサウンドを制御するクラス
/// </summary>
public sealed class BackgroundSoundHandler : System.IDisposable
{
    private readonly int _startCrossFadeOffset;

    private SoundManager _soundManager;
    private ScoreManager _scoreManager;

    private CompositeDisposable _soundEventDisposable = new();

    public BackgroundSoundHandler(SoundManager soundManager, ScoreManager scoreManager, int startCrossFadeOffset)
    {
        _soundManager         = soundManager;
        _scoreManager         = scoreManager;
        _startCrossFadeOffset = startCrossFadeOffset;
    }

    /// <summary>
    /// 指定のスコア (高度) に到達した際のBGM切り替えを登録する
    /// </summary>
    /// <param name="score">切り替えるスコア (高度)</param>
    /// <param name="nextMusic">切り替えるBGM</param>
    public void RegisterMusicSwitching(int score, MusicDef nextMusic)
    {
        _scoreManager.MaxScoreChanged.Skip(1)
                     .DistinctUntilChanged()
                     .Where(newScore => newScore >= score - _startCrossFadeOffset)
                     .Take(1)
                     .Subscribe(_ =>
                     {
                         _soundManager.CrossFadeMusic(nextMusic, 2f).Forget();
                     })
                     .AddTo(_soundEventDisposable);
    }

    public void Dispose()
    {
        _soundEventDisposable.Dispose();

        _soundManager         = null;
        _scoreManager         = null;
        _soundEventDisposable = null;
    }
}
