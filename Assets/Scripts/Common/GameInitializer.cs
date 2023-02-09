using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameInitializer
{
    private static bool _isInitialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void Initialize()
    {
        Application.targetFrameRate = 60;

        await GenerateDontDestroyObjects();

        _isInitialized = true;
    }

    /// <summary>
    /// 初期生成対象のプレハブをDontDestroyとして生成する
    /// </summary>
    private static async Task GenerateDontDestroyObjects()
    {
        // InstantiateOnLoadラベルのアセットをロード
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>("InstantiateOnLoad", null);

        IList<GameObject> prefabs = await handle.Task;

        // ロードしたプレハブを生成
        foreach (GameObject prefab in prefabs)
        {
            GameObject newObj = Object.Instantiate(prefab);
            newObj.name = newObj.name.Replace("(Clone)", "");
            Object.DontDestroyOnLoad(newObj);
        }

        Addressables.Release(handle);

        // 初期化完了通知
        UniRx.MessageBroker.Default.Publish(GameEvent.OnGameInitialized.Get());
    }

    /// <summary>
    /// 初期化完了を待機する
    /// </summary>
    public static async UniTask WaitForInitialize()
    {
        await UniTask.WaitUntil(() => _isInitialized);
    }
}
