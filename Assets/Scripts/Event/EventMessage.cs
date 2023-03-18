public abstract class EventMessage
{
}

/// <summary>
/// <see cref="UniRx.MessageBroker"/>で使用する通知用クラスの基底クラス
/// </summary>
public abstract class EventMessage<E>
    where E : EventMessage<E>, new()
{
    protected static readonly E Cache = new();

    public static E Get() => Cache;
}

/// <summary>
/// <see cref="UniRx.MessageBroker"/>で使用する通知用クラスの基底クラス
/// </summary>
public abstract class EventMessage<E, P1>
    where E : EventMessage<E, P1>, new()
{
    protected static readonly E Cache = new();

    protected P1 param1;

    public static E Get(P1 param1)
    {
        Cache.param1 = param1;

        return Cache;
    }
}

/// <summary>
/// <see cref="UniRx.MessageBroker"/>で使用する通知用クラスの基底クラス
/// </summary>
public abstract class EventMessage<E, P1, P2>
    where E : EventMessage<E, P1, P2>, new()
{
    protected static readonly E Cache = new();

    protected P1 param1;
    protected P2 param2;

    public static E Get(P1 param1, P2 param2)
    {
        Cache.param1 = param1;
        Cache.param2 = param2;

        return Cache;
    }
}

/// <summary>
/// <see cref="UniRx.MessageBroker"/>で使用する通知用クラスの基底クラス
/// </summary>
public abstract class EventMessage<E, P1, P2, P3>
    where E : EventMessage<E, P1, P2, P3>, new()
{
    protected static readonly E Cache = new();

    protected P1 param1;
    protected P2 param2;
    protected P3 param3;

    public static E Get(P1 param1, P2 param2, P3 param3)
    {
        Cache.param1 = param1;
        Cache.param2 = param2;
        Cache.param3 = param3;

        return Cache;
    }
}
