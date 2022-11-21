using UniRx;

public sealed class GameManager : SingletonMonoBehaviour<GameManager>
{
    private readonly MessageBroker _gameBroker   = new();
    private readonly MessageBroker _inputBroker  = new();
    private readonly MessageBroker _playerBroker = new();

    private readonly ReactiveProperty<GameState> _currentState = new();

    public IMessageBroker GameBroker   => _gameBroker;
    public IMessageBroker InputBroker  => _inputBroker;
    public IMessageBroker PlayerBroker => _playerBroker;

    /// <summary>現在のゲームステート</summary>
    public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;

    protected override void Awake()
    {
        base.Awake();

        _currentState.Value = GameState.Ready;
    }

    private void Start()
    {
        _gameBroker.AddTo(this);
        _inputBroker.AddTo(this);
        _playerBroker.AddTo(this);

        EventReceiver();
    }

    private void EventReceiver()
    {
        // ステート変更要求イベントの受付
        _gameBroker.Receive<GameEvent.OnStateChangeRequest>().Subscribe(OnStateChangeRequest).AddTo(this);

        // 燃料切れイベントの受付
        _gameBroker.Receive<GameEvent.OnFuelEmptied>().Subscribe(_ => OnFuelEmptied()).AddTo(this);

        // プレイヤーが場外に出たイベントの受付
        _playerBroker.Receive<GameEvent.Player.OnOutOfField>().Subscribe(_ => OnPlayerOutOfField()).AddTo(this);
    }

    private void OnStateChangeRequest(GameEvent.OnStateChangeRequest data)
    {
        // ゲームが終了している際はステート変更を受け付けない
        if (_currentState.Value == GameState.Finish)
        {
            data.OnRejected?.Invoke();

            return;
        }

        _currentState.Value = data.ToChangeState;
        data.OnAccepted?.Invoke();
    }

    private void OnFuelEmptied()
    {
        _currentState.Value = GameState.Finish;
    }

    private void OnPlayerOutOfField()
    {
        _currentState.Value = GameState.Finish;
    }
}
