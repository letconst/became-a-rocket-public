using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public sealed class InitializeHandler : MonoBehaviour
{
    private void Start()
    {
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
