using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class TitleController : MonoBehaviour, InputActions.IUIActions
{
    private InputActions _input;

    private async UniTaskVoid Start()
    {
        _input = new InputActions();
        _input.UI.SetCallbacks(this);
        _input.BindEvent(this);
        _input.Enable();

        // SoundManager生成まで待機
        await UniTask.DelayFrame(5);
        await SoundManager.Instance.WaitForReady();

        SoundManager.Instance.StopAll();
        SoundManager.Instance.PlayMusic(MusicDef.Stage1, isLoop: true);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        // 押下時のみ処理
        if (!context.started)
            return;

        if (SceneManager.Instance.IsLoading)
            return;

        var dataPack = new ToMainGameSceneDataPack(GameScene.Title);

        SoundManager.Instance.PlayUISound(UISoundDef.Submit);
        SceneManager.Instance.LoadScene(GameScene.MainGame, sceneDataPack: dataPack);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
    }
}
