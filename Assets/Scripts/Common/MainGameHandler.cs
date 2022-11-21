using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public sealed class MainGameHandler : MonoBehaviour, InputActions.IUIActions
{
    private bool _isSubmittedOnReady;

    private InputActions _input;

    private GameObject _lastSelectedUiObject;

    private GameSoundHandler _soundHandler;

    private async UniTaskVoid Start()
    {
        _input = new InputActions();
        _input.UI.SetCallbacks(this);
        _input.BindEvent(this);
        _input.Enable();

        // ゲームステートがReady中にSubmit入力受付
        this.UpdateAsObservable()
            .Where(static _ => GameManager.Instance.CurrentState.Value == GameState.Ready)
            .Where(_ => _isSubmittedOnReady)
            .Take(1)
            .Subscribe(static _ => OnSubmittedOnReady())
            .AddTo(this);

        await UniTask.DelayFrame(3);

        await SoundManager.Instance.WaitForReady();

        _soundHandler = new GameSoundHandler(GameManager.Instance, SoundManager.Instance, ScoreManager.Instance);
        _soundHandler.AddTo(this);

        // タイトルからの遷移だったら、BGMは変わらないので終了
        if (SceneManager.Instance.PrevSceneData?.PrevGameGameScene == GameScene.Title)
            return;

        SoundManager.Instance.StopAll();
        SoundManager.Instance.PlayMusic(MusicDef.Stage1, isLoop: true);
    }

    private static void OnSubmittedOnReady()
    {
        GameManager.Instance.GameBroker.Publish(GameEvent.OnStateChangeRequest.Get(GameState.InGame, null, null));
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        // 押下時のみ処理
        if (!context.started)
            return;

        if (SceneManager.Instance.IsLoading)
            return;

        if (GameManager.Instance.CurrentState.Value == GameState.Ready)
        {
            _isSubmittedOnReady = true;

            SoundManager.Instance.PlayUISound(UISoundDef.Submit);
        }

        if (EventSystem.current.currentSelectedGameObject)
        {
            SoundManager.Instance.PlayUISound(UISoundDef.Submit);
        }
    }

    public async void OnNavigate(InputAction.CallbackContext context)
    {
        // 押下時のみ処理
        if (!context.action.WasPressedThisFrame())
            return;

        // 選択中のUI GameObject切り替わりまでラグがあるので待機
        await UniTask.DelayFrame(3);

        GameObject selectedUiObject = EventSystem.current.currentSelectedGameObject;

        // 選択してるUIが変化していなければ終了
        if (selectedUiObject == _lastSelectedUiObject)
            return;

        _lastSelectedUiObject = selectedUiObject;

        SoundManager.Instance.PlayUISound(UISoundDef.Select);
    }
}
