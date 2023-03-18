using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LetConst.MasterData;
using UniRx;
using UnityEngine.Assertions;

/// <summary>
/// 生成されたギミックの管理を行うためのクラス
/// </summary>
public sealed class GimmickManager : SingletonMonoBehaviour<GimmickManager>
{
    private GimmickSoundHandler _soundHandler;

    private readonly Queue<GimmickGenerateEntry> _entriesPerHeightQueue = new();

    /// <summary>ギミック生成のマスターデータ</summary>
    public MasterGimmickGeneration GenerateData { get; private set; }

    /// <summary>各種ギミックのプーリング用インスタンス</summary>
    public GimmickPool Pool { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Initialize().Forget();
    }

    private async UniTaskVoid Initialize()
    {
        Pool         = new GimmickPool();
        GenerateData = await MasterDataManager.Instance.GetMasterDataAsync<MasterGimmickGeneration>("GimmickGeneration");

        Assert.IsNotNull(GenerateData, "GenerateData != null");

        // スコア (高度) ごとのギミック生成テーブルをキューとしてキャッシュ
        foreach (GimmickGenerateEntry entry in GenerateData.GenerateGimmickEntries)
        {
            _entriesPerHeightQueue.Enqueue(entry);
        }

        // 他の初期化処理を待機
        await UniTask.WhenAll(GameInitializer.WaitForInitialize(), SoundManager.Instance.WaitForReady());

        _soundHandler = new GimmickSoundHandler(GameManager.Instance, SoundManager.Instance);
        _soundHandler.AddTo(this);
    }

    /// <summary>
    /// 初期化が完了するまで待機する
    /// </summary>
    public async UniTask WaitForReady()
    {
        List<UniTask> tasks = new()
        {
            UniTask.Create(static async () => { await UniTask.WaitWhile(() => Instance.GenerateData == null); })
        };

        await UniTask.WhenAll(tasks);
    }

    /// <summary>
    /// 指定の高さに対応する生成プロファイルを取得する
    /// TODO: 過去の高さに対応するプロファイルは取得できないので、どうにかする
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public GimmickGenerateEntry GetGenerateProfileByHeight(int height)
    {
        GimmickGenerateEntry queue = _entriesPerHeightQueue.Peek();

        if (height < queue.RangeMax)
        {
            return queue;
        }

        _entriesPerHeightQueue.Dequeue();

        return _entriesPerHeightQueue.Peek();
    }
}
