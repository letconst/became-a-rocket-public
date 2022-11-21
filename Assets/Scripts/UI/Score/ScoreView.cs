using TMPro;
using UnityEngine;

public sealed class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public void SetScoreText(string newScore)
    {
        scoreText.text = newScore;
    }
}
