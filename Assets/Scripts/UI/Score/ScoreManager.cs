using System;
using UniRx;

/// <summary>
/// スコアを管理するクラス。UIのModelクラスも兼用
/// </summary>
public sealed class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    private readonly ReactiveProperty<int> _currentScore = new();

    /// <summary>現在スコアの変更Observable</summary>
    public IObservable<int> CurrentScoreChanged => _currentScore;

    /// <summary>現在のスコア (メートル)</summary>
    public int CurrentScore => _currentScore.Value;

    private readonly ReactiveProperty<int> _maxScore = new();

    /// <summary>最大スコアの変更Observable</summary>
    public IObservable<int> MaxScoreChanged => _maxScore;

    /// <summary>最大スコア (メートル)</summary>
    public int MaxScore => _maxScore.Value;

    public void SetScore(int newScore)
    {
        _currentScore.Value = newScore;

        if (newScore > _maxScore.Value)
        {
            _maxScore.Value = newScore;
        }
    }
}
