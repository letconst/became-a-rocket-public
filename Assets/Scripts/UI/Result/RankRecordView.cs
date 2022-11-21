using TMPro;
using UnityEngine;

public sealed class RankRecordView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI rankText;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// ランクテキストを設定する
    /// </summary>
    /// <param name="rank">設定するランク</param>
    public void SetRankText(int rank)
    {
        rankText.text = StringUtility.GetRankingRecordLabel(rank);
    }

    /// <summary>
    /// スコアテキストを設定する
    /// </summary>
    /// <param name="score">設定するスコア</param>
    public void SetScoreText(int score)
    {
        scoreText.text = GameConstants.ScoreFormat.Format(score.ToString());
    }
}
