using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// タイトルシーンを制御するクラス
/// </summary>
public sealed class TitleController : MonoBehaviour, InputActions.IUIActions
{
    private InputActions _input;

    private void Start()
    {
        Initialize().Forget();
    }

    private async UniTaskVoid Initialize()
    {
        _input = new InputActions();
        _input.UI.SetCallbacks(this);
        _input.BindEvent(this);
        _input.Enable();

        // SoundManager生成まで待機
        await GameInitializer.WaitForInitialize();
        await SoundManager.Instance.WaitForReady();

        SoundManager.Instance.StopAll();
        SoundManager.Instance.PlayMusic(MusicDef.Stage1, isLoop: true);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        // 押下時のみ処理
        if (!context.started)
            return;

        LoadMainGame();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // ButtonControlか確認
        if (context.control is not ButtonControl control)
            return;

        if (control.wasPressedThisFrame)
        {
            LoadMainGame();
        }
    }

    /// <summary>
    /// メインゲームシーンを読み込む
    /// </summary>
    private void LoadMainGame()
    {
        if (SceneManager.Instance.IsLoading)
            return;

        var dataPack = new ToMainGameSceneDataPack(GameScene.Title);

        SoundManager.Instance.PlayUISound(UISoundDef.Submit);
        SceneManager.Instance.LoadScene(GameScene.MainGame, sceneDataPack: dataPack);
    }
}
