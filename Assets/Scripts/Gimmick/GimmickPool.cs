using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public sealed class GimmickPool
{
    private readonly Transform _poolParent;

    private GameObject _poolTargetPrefab;
    private Transform  _poolTargetParent;

    private readonly Dictionary<int, (GameObject prefab, ObjectPool<GameObject> pool, Transform parent)> _gimmickPoolDict = new();

    private System.IDisposable _contactedEventDisposable;

    public GimmickPool()
    {
        _poolParent = new GameObject("Gimmicks").transform;

        EventReceiver();
    }

    ~GimmickPool()
    {
        _contactedEventDisposable?.Dispose();
    }

    private void EventReceiver()
    {
        // ギミックの返却要求イベントを受付
        _contactedEventDisposable = GameManager.Instance.PlayerBroker
                                               .Receive<GameEvent.Player.OnReturnGimmickRequest>()
                                               .Subscribe(OnGimmickReturnRequest);
    }

    private GameObject OnInstantiate()
    {
        GameObject newObj = Object.Instantiate(_poolTargetPrefab, Vector3.zero, Quaternion.identity, _poolTargetParent);

        return newObj;
    }

    private void OnRent(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReturn(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnClear(GameObject obj)
    {
        Object.Destroy(obj);
    }

    private void OnGimmickReturnRequest(GameEvent.Player.OnReturnGimmickRequest data)
    {
        // 触れたギミックをプールに戻す
        Return(data.Type, data.ObjectToReturn);
    }

    /// <summary>
    /// ギミックを登録し、プールを作成する
    /// </summary>
    /// <param name="targetType">対象のギミック</param>
    public void RegisterPoolTarget(GimmickType targetType, GameObject targetPrefab)
    {
        int typeIndex = (int) targetType;

        // すでに指定ギミックのプールが作成されているなら終了
        if (_gimmickPoolDict.ContainsKey(typeIndex))
            return;

        // プールおよび親オブジェクト作成
        ObjectPool<GameObject> newPool   = new(OnInstantiate, OnRent, OnReturn, OnClear);
        Transform              newParent = new GameObject(targetPrefab.name).transform;
        newParent.parent = _poolParent;

        _gimmickPoolDict.Add(typeIndex, (targetPrefab, newPool, newParent));
    }

    /// <summary>
    /// プールから指定のギミックのGameObjectを取り出す
    /// </summary>
    /// <param name="targetType">取り出すギミックの種類</param>
    /// <returns>取り出したギミックのGameObject</returns>
    public GameObject Rent(GimmickType targetType)
    {
        int typeIndex = (int) targetType;

        // 指定ギミックのプレハブ・親オブジェクト取得
        _poolTargetPrefab = _gimmickPoolDict[typeIndex].prefab;
        _poolTargetParent = _gimmickPoolDict[typeIndex].parent;

        return _gimmickPoolDict[typeIndex].pool.Get();
    }

    /// <summary>
    /// 指定のギミックGameObjectをプールに返却する
    /// </summary>
    /// <param name="targetType">返却するギミックの種類</param>
    /// <param name="targetGameObject">返却するGameObject</param>
    public void Return(GimmickType targetType, GameObject targetGameObject)
    {
        _gimmickPoolDict[(int) targetType].pool.Release(targetGameObject);
    }
}
