using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移制御を行うクラス
/// </summary>
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
        Initialize();
    }

    private void Initialize()
    {
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
    /// <param name="fadeOutTimeInSec">フェードアウトのフェード時間 (秒)</param>
    /// <param name="fadeInTimeInSec">フェードインのフェード時間 (秒)</param>
    /// <param name="sceneDataPack">1つ前のシーン情報として持たせるデータ</param>
    public void LoadScene(GameScene nextScene, float fadeOutTimeInSec = 1f, float fadeInTimeInSec = 1f,
                          SceneDataPack sceneDataPack = null)
    {
        // 他にシーンを読み込み中だったら行わない
        if (IsLoading) return;

        PrevSceneData = sceneDataPack;

        InternalLoadScene(nextScene.ToString(), fadeOutTimeInSec, fadeInTimeInSec).Forget();
    }

    /// <summary>
    /// シーンを読み込む内部処理
    /// </summary>
    /// <param name="sceneName">読み込む対象のシーン名</param>
    /// <param name="fadeOutTimeInSec">フェードアウトのフェード時間 (秒)</param>
    /// <param name="fadeInTimeInSec">フェードインのフェード時間 (秒)</param>
    private async UniTask InternalLoadScene(string sceneName, float fadeOutTimeInSec, float fadeInTimeInSec)
    {
        IsLoading = true;

        Assert.IsNotNull(fadeCanvasGroup, "fadeCanvasGroup != null");

        fadeCanvasGroup.blocksRaycasts = true;

        await FadeTransition.FadeOut(fadeCanvasGroup, fadeOutTimeInSec);
        await LoadSceneAsync(sceneName);
        await FadeTransition.FadeIn(fadeCanvasGroup, fadeInTimeInSec);

        fadeCanvasGroup.blocksRaycasts = false;

        IsLoading = false;
    }

    /// <summary>
    /// シーンを非同期で読み込む
    /// </summary>
    /// <param name="sceneName">読み込む対象のシーン名</param>
    private async UniTask LoadSceneAsync(string sceneName)
    {
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName);

        await handle.ToUniTask();

        CurrentScene = handle.Result.Scene;
    }
}
