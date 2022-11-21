using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ランキングの1レコード
/// </summary>
[System.Serializable]
public sealed class RankingRecord
{
    public int score;

    public RankingRecord(int score)
    {
        this.score = score;
    }
}

/// <summary>
/// ランキングレコードをリストとして保持するクラス
/// </summary>
[System.Serializable]
public sealed class RankingList
{
    [UnityEngine.SerializeField]
    private List<RankingRecord> records;

    public IList<RankingRecord> Records => records;

    public RankingList()
    {
        records ??= new List<RankingRecord>();
    }

    /// <summary>
    /// 指定のレコードを追加する
    /// </summary>
    /// <param name="record">追加するレコード</param>
    public void AddRecord(RankingRecord record)
    {
        records.Add(record);
    }

    /// <summary>
    /// 指定のレコードの順位を取得する
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public int GetRecordRank(RankingRecord record)
    {
        if (!records.Contains(record))
            return -1;

        return records.IndexOf(record) + 1;
    }

    /// <summary>
    /// 指定のレコードを削除する
    /// </summary>
    /// <param name="record">削除するレコード</param>
    public void RemoveRecord(RankingRecord record)
    {
        if (!records.Contains(record))
            return;

        records.Remove(record);
    }

    /// <summary>
    /// 登録されているレコードをランク順にソートする
    /// </summary>
    public void SortRecordsByDesc()
    {
        records = records.OrderByDescending(r => r.score).ToList();
    }
}
