using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerInputEventEmitter : MonoBehaviour, InputActions.IPlayerActions
{
    private InputActions      _input;
    private IMessagePublisher _broker;

    private bool _isHoldLeft;
    private bool _isHoldRight;

    private void Start()
    {
        _input  = new InputActions();
        _broker = GameManager.Instance.InputBroker;

        _input.Player.SetCallbacks(this);
        _input.BindEvent(this);
        _input.Enable();

        this.UpdateAsObservable()
            .Subscribe(_ => InputEventEmitter())
            .AddTo(this);
    }

    private void InputEventEmitter()
    {
        // 左回転入力
        if (GameManager.Instance.CurrentState.Value == GameState.InGame && _isHoldLeft)
        {
            _broker.Publish(GameEvent.Input.OnRotateRequest
                                     .Get(PlayerConstants.RotateDirection.Left,
                                          PlayerConstants.RotateSpeedByInput));
        }

        // 右回転入力
        if (GameManager.Instance.CurrentState.Value == GameState.InGame && _isHoldRight)
        {
            _broker.Publish(GameEvent.Input.OnRotateRequest
                                     .Get(PlayerConstants.RotateDirection.Right,
                                          -PlayerConstants.RotateSpeedByInput));
        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHoldLeft = true;
        }

        if (context.canceled)
        {
            _isHoldLeft = false;
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHoldRight = true;
        }

        if (context.canceled)
        {
            _isHoldRight = false;
        }
    }
}
