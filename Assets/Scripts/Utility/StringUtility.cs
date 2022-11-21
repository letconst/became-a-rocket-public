using JetBrains.Annotations;

public static class StringUtility
{
    [StringFormatMethod("format")]
    public static string Format(this string source, object arg0)
    {
        return string.Format(source, arg0);
    }

    [StringFormatMethod("format")]
    public static string Format(this string source, object arg0, object arg1)
    {
        return string.Format(source, arg0, arg1);
    }

    [StringFormatMethod("format")]
    public static string Format(this string source, object arg0, object arg1, object arg2)
    {
        return string.Format(source, arg0, arg1, arg2);
    }

    [StringFormatMethod("format")]
    public static string Format(this string source, params object[] args)
    {
        return string.Format(source, args);
    }

    /// <summary>
    /// 指定のランクに対応するラベルテキストを取得する
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public static string GetRankingRecordLabel(int rank)
    {
        return rank switch
        {
            1    => "1st",
            2    => "2nd",
            3    => "3rd",
            >= 4 => $"{rank}th",
            _    => rank.ToString()
        };
    }
}
