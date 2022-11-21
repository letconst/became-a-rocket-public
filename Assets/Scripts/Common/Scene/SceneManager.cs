using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public sealed class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    [SerializeField]
    private CanvasGroup fadeCanvasGroup;

    /// <summary>シーンを読み込んでいる最中か</summary>
    public bool IsLoading { get; private set; }

    /// <summary>現在読み込まれているシーン</summary>
    public Scene CurrentScene { get; private set; }

    /// <summary>1つ前のシーン情報</summary>
    public SceneDataPack PrevSceneData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        Assert.IsNotNull(fadeCanvasGroup, "fadeCanvasGroup != null");

        fadeCanvasGroup.alpha          = 0f;
        fadeCanvasGroup.interactable   = false;
        fadeCanvasGroup.blocksRaycasts = false;

        CurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }

    /// <summary>
    /// シーンを読み込む
    /// </summary>
    /// <param name="nextScene">読み込む対象のシーン</param>
    /// <param name="fadeOutTime">フェードアウトのフェード時間 (秒)</param>
    /// <param name="fadeInTime">フェードインのフェード時間 (秒)</param>
    /// <param name="sceneDataPack">1つ前のシーン情報として持たせるデータ</param>
    public void LoadScene(GameScene nextScene, float fadeOutTime = 1f, float fadeInTime = 1f, SceneDataPack sceneDataPack = null)
    {
        // 他にシーンを読み込み中だったら行わない
        if (IsLoading) return;

        PrevSceneData = sceneDataPack;

        InternalLoadScene(nextScene.ToString(), fadeOutTime, fadeInTime).Forget();
    }

    private async UniTask InternalLoadScene(string sceneName, float fadeOutTime, float fadeInTime)
    {
        IsLoading = true;

        Assert.IsNotNull(fadeCanvasGroup, "fadeCanvasGroup != null");

        fadeCanvasGroup.blocksRaycasts = true;

        await FadeTransition.FadeOut(fadeCanvasGroup, fadeOutTime);
        await LoadSceneAsync(sceneName);
        await FadeTransition.FadeIn(fadeCanvasGroup, fadeInTime);

        fadeCanvasGroup.blocksRaycasts = false;

        IsLoading = false;
    }

    private async UniTask LoadSceneAsync(string sceneName)
    {
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName);

        await handle.ToUniTask();

        CurrentScene = handle.Result.Scene;
    }
}
