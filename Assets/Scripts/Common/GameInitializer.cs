using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Application.targetFrameRate = 60;

        GenerateDontDestroyObjects();
    }

    /// <summary>
    /// 初期生成対象のプレハブをDontDestroyとして生成する
    /// </summary>
    private static async void GenerateDontDestroyObjects()
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
}
