using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// ゲーム初期化用シーンを制御するクラス
/// </summary>
public sealed class InitializeHandler : MonoBehaviour
{
    private void Start()
    {
        // 初期化完了通知が来たらタイトルへ遷移
        MessageBroker.Default.Receive<GameEvent.OnGameInitialized>()
                     .Subscribe(_ => OnGameInitialized().Forget())
                     .AddTo(this);
    }

    private async UniTaskVoid OnGameInitialized()
    {
        await SoundManager.Instance.WaitForReady();

        SceneManager.Instance.LoadScene(GameScene.Title);
    }
}
