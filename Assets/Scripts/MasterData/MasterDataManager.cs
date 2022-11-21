using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MasterDataManager
{
    #region Singleton

    private static MasterDataManager _instance;

    public static MasterDataManager Instance => _instance ??= new MasterDataManager();

    private MasterDataManager()
    {
    }

    #endregion

    private readonly List<AsyncOperationHandle> _dataHandles = new();

    /// <summary>
    /// 指定IDのマスターデータを非同期で取得する
    /// </summary>
    /// <param name="id">マスターデータID</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>マスターデータ</returns>
    public UniTask<T> GetMasterDataAsync<T>(string id) where T : MasterDataBase
    {
        // TODO: IDの存在確認
        return InternalGetMasterDataAsync<T>($"MasterData/{id}");
    }

    /// <summary>
    /// プレイヤーのマスターデータを非同期で取得する
    /// </summary>
    /// <returns>プレイヤーのマスターデータ</returns>
    public async UniTask<MasterPlayer> GetPlayerMasterDataAsync()
    {
        if (_dataHandles.Count > 0)
        {
            AsyncOperationHandle playerHandle = _dataHandles.Find(static handle => handle.Result is MasterPlayer);

            // プレイヤーのマスターデータがすでにロードいる場合はそれを返す
            if (playerHandle.IsValid())
            {
                return await playerHandle.Convert<MasterPlayer>().ToUniTask();
            }
        }

        return await InternalGetMasterDataAsync<MasterPlayer>("MasterData/Player");
    }

    private async UniTask<T> InternalGetMasterDataAsync<T>(string key) where T : MasterDataBase
    {
        AsyncOperationHandle<T> op = Addressables.LoadAssetAsync<T>(key);

        _dataHandles.Add(op);

        return await op.ToUniTask();
    }

    ~MasterDataManager()
    {
        // ハンドルの開放
        foreach (AsyncOperationHandle handle in _dataHandles)
        {
            Addressables.Release(handle);
        }

        _dataHandles.Clear();
    }
}
