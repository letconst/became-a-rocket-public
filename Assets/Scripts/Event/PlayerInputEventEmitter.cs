using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// 入力に応じたイベントを発行するクラス
/// </summary>
public sealed class PlayerInputEventEmitter : MonoBehaviour, InputActions.IPlayerActions
{
    private InputActions      _input;
    private IMessagePublisher _broker;

    private bool _isHoldLeft;
    private bool _isHoldRight;

#if UNITY_WEBGL
    private TouchInfo _readyInput = new();

    private readonly TouchInfo _leftTouchInfo  = new();
    private readonly TouchInfo _rightTouchInfo = new();
#endif

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
        GameState gameState = GameManager.Instance.CurrentState.Value;

        if (gameState == GameState.InGame && _isHoldLeft)
        {
            _broker.Publish(GameEvent.Input.OnRotateRequest
                                     .Get(PlayerConstants.RotateDirection.Left,
                                          PlayerConstants.RotateSpeedByInput));
        }

        // 右回転入力
        if (gameState == GameState.InGame && _isHoldRight)
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

    public void OnTouch(InputAction.CallbackContext context)
    {
#if UNITY_WEBGL
        // TouchControlか確認
        if (context.control is not TouchControl control)
            return;

        GameState  gameState  = GameManager.Instance.CurrentState.Value;
        TouchPhase touchPhase = control.phase.ReadValue();

        if (gameState == GameState.Ready)
        {
            if (touchPhase == TouchPhase.Began)
            {
                if (SceneManager.Instance.IsLoading)
                    return;

                _readyInput = new TouchInfo { touchId = control.touchId.ReadValue() };

                GameManager.Instance.GameBroker.Publish(GameEvent.OnStateChangeRequest.Get(GameState.InGame, null, null));
                SoundManager.Instance.PlayUISound(UISoundDef.Submit);
            }
        }
        else if (gameState == GameState.InGame)
        {
            float screenHalfWidth   = Screen.width / 2f;                                // 画面サイズの横半分
            bool  isLeftSideTouched = control.position.ReadValue().x < screenHalfWidth; // 画面左側をタッチしたか
            int   touchId           = control.touchId.ReadValue();                      // タッチID

            // タッチ開始でホールドフラグ有効
            // Readyステート時に入力されたタッチIDでは受け付けない
            if (touchPhase == TouchPhase.Began && _readyInput?.touchId != touchId)
            {
                if (isLeftSideTouched && _leftTouchInfo.touchId == null)
                {
                    _isHoldLeft = true;
                    _leftTouchInfo.SetValue(touchId, TouchInfo.TouchSide.Left);
                }

                if (!isLeftSideTouched && _rightTouchInfo.touchId == null)
                {
                    _isHoldRight = true;
                    _rightTouchInfo.SetValue(touchId, TouchInfo.TouchSide.Right);
                }
            }

            // タッチ終了でホールドフラグ無効
            if (touchPhase is TouchPhase.Ended or TouchPhase.Canceled)
            {
                if (_readyInput != null)
                {
                    _readyInput = null;
                }

                if (_leftTouchInfo.touchId == touchId && _leftTouchInfo.touchSide == TouchInfo.TouchSide.Left)
                {
                    _isHoldLeft = false;
                    _leftTouchInfo.SetValue(null, TouchInfo.TouchSide.None);
                }

                if (_rightTouchInfo.touchId == touchId && _rightTouchInfo.touchSide == TouchInfo.TouchSide.Right)
                {
                    _isHoldRight = false;
                    _rightTouchInfo.SetValue(null, TouchInfo.TouchSide.None);
                }
            }
        }
#endif
    }

#if UNITY_WEBGL
    /// <summary>
    /// タッチした情報を保持しておくためのクラス
    /// </summary>
    private sealed class TouchInfo
    {
        public enum TouchSide
        {
            None,
            Left,
            Right,
        }

        public int?      touchId;
        public TouchSide touchSide;

        /// <summary>
        /// 各種値を設定する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="side"></param>
        public void SetValue(int? id, TouchSide side)
        {
            touchId   = id;
            touchSide = side;
        }
    }
#endif
}
