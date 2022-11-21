using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class ResultPresenter : MonoBehaviour
{
    [SerializeField]
    private ResultView view;

    [SerializeField]
    private ResultModel model;

    [SerializeField]
    private RankRecordView rankRecordPrefab;

    private void Start()
    {
        Assert.IsNotNull(view, "view != null");

        view.ResultCanvasGroup.alpha          = 0f;
        view.ResultCanvasGroup.interactable   = false;
        view.ResultCanvasGroup.blocksRaycasts = false;

        Assert.IsNotNull(model, "model != null");

        // ボタン押下時の処理を登録
        view.OnRetryClicked.Subscribe(_ => model.OnRetryClicked()).AddTo(this);
        view.OnTitleClicked.Subscribe(_ => model.OnTitleClicked()).AddTo(this);

        Assert.IsNotNull(rankRecordPrefab, "rankRecordPrefab != null");

        // ゲームステートがFinishになったら、リザルト表示を実行
        GameManager.Instance.CurrentState
                   .DistinctUntilChanged()
                   .Where(static state => state == GameState.Finish)
                   .Subscribe(_ => OnFinish())
                   .AddTo(this);

        gameObject.SetActive(false);
    }

    private void OnFinish()
    {
        Assert.IsNotNull(view, "view != null");

        // スコア表示
        RenderScore();

        // ランキングに登録および表示
        RenderRankingRecord();

        // ランキング一覧を表示
        RenderRankingList();

        Assert.IsNotNull(model, "model != null");

        // リザルトUIを表示
        gameObject.SetActive(true);
        model.ShowResult(view.ResultCanvasGroup, 1f);
    }

    private void RenderScore()
    {
        // スコアをUIに反映
        view.SetScoreValueText(ScoreManager.Instance.MaxScore);

        view.ResultCanvasGroup.interactable   = true;
        view.ResultCanvasGroup.blocksRaycasts = true;
        view.SetFocusToButton();
    }

    private void RenderRankingRecord()
    {
        var resultRecord = new RankingRecord(ScoreManager.Instance.MaxScore);

        // ランキングに登録および順位取得
        int rank = LocalRankingManager.Instance.AddRecord(resultRecord);

        // 順位表示
        view.SetRankValueText(rank);
    }

    private void RenderRankingList()
    {
        IList<RankingRecord> rankingList = LocalRankingManager.Instance.AllRecords;

        for (int i = 0; i < rankingList.Count; i++)
        {
            // 5位まで表示のため、それ以降は無視
            if (i >= 5) break;

            RankingRecord record = rankingList[i];

            // 1レコードのGameObject生成
            RankRecordView recordView = Instantiate(rankRecordPrefab, view.RankingRecordsParent);

            // 各種テキスト設定
            recordView.SetRankText(i + 1);
            recordView.SetScoreText(record.score);
        }
    }
}
