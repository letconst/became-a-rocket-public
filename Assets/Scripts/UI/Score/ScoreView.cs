using TMPro;
using UnityEngine;

/// <summary>
/// スコアUIのViewクラス
/// </summary>
public sealed class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    /// <summary>
    /// スコアテキストを設定する
    /// </summary>
    /// <param name="newScore"></param>
    public void SetScoreText(string newScore)
    {
        scoreText.text = newScore;
    }
}
