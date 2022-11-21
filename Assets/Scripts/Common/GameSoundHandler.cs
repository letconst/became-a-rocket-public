using UniRx;

public sealed class GameSoundHandler : System.IDisposable
{
    private SoundManager _soundManager;

    private CompositeDisposable _soundEventDisposable = new();

    public GameSoundHandler(GameManager gameManager, SoundManager soundManager, ScoreManager scoreManager)
    {
        _soundManager = soundManager;

        gameManager.CurrentState.DistinctUntilChanged().Subscribe(OnStateChanged).AddTo(_soundEventDisposable);

        scoreManager.MaxScoreChanged.Skip(1).Subscribe(OnMaxScoreChanged).AddTo(_soundEventDisposable);
    }

    private void OnStateChanged(GameState newState)
    {
        // ゲームオーバー時にSEを鳴らす
        if (newState != GameState.Finish)
            return;

        _soundManager.PlaySound(SoundDef.GameOver);
    }

    private void OnMaxScoreChanged(int newMaxScore)
    {
        // スコア500ごとに歓声SEを鳴らす
        if (newMaxScore % 500 == 0)
        {
            _soundManager.PlaySound(SoundDef.ScoreReached);
        }
    }

    public void Dispose()
    {
        _soundEventDisposable.Dispose();

        _soundManager         = null;
        _soundEventDisposable = null;
    }
}
