/// <summary>
/// シングルトンクラスの基底クラス
/// </summary>
/// <typeparam name="T">インスタンスとするクラスの型</typeparam>
public abstract class SingletonBase<T> where T : class
{
    private static readonly System.Lazy<T> _instance = new(CreateInstanceOfT);

    public static T Instance => _instance.Value;

    private static T CreateInstanceOfT()
    {
        return System.Activator.CreateInstance<T>();
    }
}
