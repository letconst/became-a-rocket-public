using System.Collections.Generic;
using UnityEngine;
#if UNITY_STANDALONE || UNITY_EDITOR
using UniRx;
using UniRx.Triggers;
#endif

public class LocalRankingManager : SingletonBase<LocalRankingManager>, System.IDisposable
{
    private RankingList _rankingList;

    /// <summary>
    /// ランキングの全レコード
    /// </summary>
    public IList<RankingRecord> AllRecords => _rankingList.Records;

    public LocalRankingManager()
    {
        _rankingList = PlayerPrefsUtility.GetObject<RankingList>(GameConstants.LocalRankingKey) ?? new RankingList();

#if UNITY_STANDALONE || UNITY_EDITOR
        // ゲーム終了時にDisposeするためのGameObject生成
        var objForDispose = new GameObject("LocalRankingManagerForDispose");
        Object.DontDestroyOnLoad(objForDispose);

        objForDispose.OnDestroyAsObservable()
                     .Take(1)
                     .Subscribe(_ => Dispose());
#endif
    }

    /// <summary>
    /// 指定のレコードをランキングに追加する
    /// </summary>
    /// <param name="record">追加するレコード</param>
    /// <returns></returns>
    public int AddRecord(RankingRecord record)
    {
        _rankingList.AddRecord(record);
        _rankingList.SortRecordsByDesc();

#if UNITY_WEBGL
        // WebGLではページのリロード等の際にDestroyイベントなどが呼ばれないことがあるため
        // 登録するごとに保存
        SaveRecords();
#endif

        return _rankingList.GetRecordRank(record);
    }

    /// <summary>
    /// 指定のレコードの順位を取得する
    /// </summary>
    /// <param name="record">順位を取得したいレコード</param>
    /// <returns></returns>
    public int GetRecordRank(RankingRecord record)
    {
        return _rankingList.GetRecordRank(record);
    }

    /// <summary>
    /// 指定のレコードをランキングから削除する
    /// </summary>
    /// <param name="record">削除するレコード</param>
    public void RemoveRecord(RankingRecord record)
    {
        _rankingList.RemoveRecord(record);
    }

    /// <summary>
    /// 登録されているレコードを保存する
    /// </summary>
    public void SaveRecords()
    {
        PlayerPrefsUtility.SetObject(GameConstants.LocalRankingKey, _rankingList);
        PlayerPrefs.Save();
    }

    public void Dispose()
    {
        SaveRecords();

        _rankingList = null;
    }
}
