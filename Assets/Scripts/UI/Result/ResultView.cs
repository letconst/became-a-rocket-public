using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// リザルトUIのViewクラス
/// </summary>
public sealed class ResultView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup resultCanvasGroup;

    [SerializeField]
    private TextMeshProUGUI scoreValueText;

    [SerializeField]
    private TextMeshProUGUI rankValueText;

    [SerializeField]
    private Transform rankingRecordsParent;

    [SerializeField]
    private Button retryButton;

    [SerializeField]
    private Button titleButton;

    /// <summary>リザルト表示用のCanvasGroup</summary>
    public CanvasGroup ResultCanvasGroup => resultCanvasGroup;

    /// <summary>ランキングリストの親オブジェクト</summary>
    public Transform RankingRecordsParent => rankingRecordsParent;

    /// <summary>リトライボタンをクリックした際のobserver</summary>
    public IObservable<Unit> OnRetryClicked => retryButton.OnClickAsObservable();

    /// <summary>タイトルボタンをクリックした際のobserver</summary>
    public IObservable<Unit> OnTitleClicked => titleButton.OnClickAsObservable();

    /// <summary>
    /// リザルトのスコアテキストを設定する
    /// </summary>
    /// <param name="score">設定するスコア</param>
    public void SetScoreValueText(int score)
    {
        Assert.IsNotNull(scoreValueText, "scoreValueText != null");

        scoreValueText.text = GameConstants.ScoreFormat.Format(score.ToString());
    }

    /// <summary>
    /// リザルトのランクテキストを設定する
    /// </summary>
    /// <param name="rank">設定するランク</param>
    public void SetRankValueText(int rank)
    {
        Assert.IsNotNull(rankValueText, "rankValueText != null");

        rankValueText.text = rank.ToString();
    }

    /// <summary>
    /// リザルトのボタンに選択をフォーカスさせる
    /// </summary>
    public void SetFocusToButton()
    {
        Assert.IsNotNull(retryButton, "retryButton != null");

        EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
    }
}
