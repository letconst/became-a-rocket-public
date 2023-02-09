using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ランキングの1レコード
/// </summary>
[System.Serializable]
public sealed class RankingRecord : System.IEquatable<RankingRecord>
{
    public int score;

    public RankingRecord(int score)
    {
        this.score = score;
    }

    public bool Equals(RankingRecord other)
    {
        return score == other?.score;
    }

    public override int GetHashCode()
    {
        return score.GetHashCode();
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

        // 念の為、重複解消しておく
        DistinctRecords();
    }

    /// <summary>
    /// 指定のレコードを追加する
    /// </summary>
    /// <param name="record">追加するレコード</param>
    public void AddRecord(RankingRecord record)
    {
        int duplicateCount = 0;

        // 重複チェック
        foreach (RankingRecord rankingRecord in records)
        {
            if (rankingRecord.Equals(record))
            {
                duplicateCount++;
            }
        }

        // 重複があれば回避
        if (0 < duplicateCount)
        {
            // 2件以上重複してたら解消
            if (1 < duplicateCount)
            {
                DistinctRecords();
            }

            return;
        }

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

    /// <summary>
    /// 登録されているレコードから重複項目を消去する
    /// </summary>
    private void DistinctRecords()
    {
        HashSet<RankingRecord> hashRecords = new(records);
        records = hashRecords.ToList();
    }
}
