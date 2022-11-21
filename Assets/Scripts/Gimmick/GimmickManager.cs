using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LetConst.MasterData;
using UniRx;
using UnityEngine.Assertions;

public sealed class GimmickManager : SingletonMonoBehaviour<GimmickManager>
{
    private GimmickSoundHandler _soundHandler;

    /// <summary>ギミック生成のマスターデータ</summary>
    public MasterGimmickGeneration GenerateData { get; private set; }

    /// <summary>各種ギミックのプーリング用インスタンス</summary>
    public GimmickPool Pool { get; private set; }

    protected override async void Awake()
    {
        base.Awake();

        Pool = new GimmickPool();
        GenerateData =
            await MasterDataManager.Instance.GetMasterDataAsync<MasterGimmickGeneration>("GimmickGeneration");

        await UniTask.DelayFrame(3);
        await SoundManager.Instance.WaitForReady();

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
    /// 現在スコアでのギミック生成プロファイルを取得する
    /// </summary>
    /// <returns></returns>
    public GimmickGenerateEntry GetCurrentScoreProfile()
    {
        Assert.IsNotNull(GenerateData, "GenerateData != null");

        GimmickGenerateEntry currentProfileToReturn = null;

        int currentScore = ScoreManager.Instance.CurrentScore;

        foreach (GimmickGenerateEntry entry in GenerateData.GenerateGimmickEntries)
        {
            if (entry.RangeMin <= currentScore && currentScore <= entry.RangeMax)
            {
                currentProfileToReturn = entry;

                break;
            }
        }

        return currentProfileToReturn;
    }

    /// <summary>
    /// 現在スコアでの指定ギミックの設定データを取得する
    /// </summary>
    /// <param name="type">取得するデータのギミック種類</param>
    /// <returns></returns>
    public GimmickGenerateOptions GetCurrentGenerateOptions(GimmickType type)
    {
        GimmickGenerateOptions currentOptionToReturn = null;
        GimmickGenerateEntry   currentEntry          = GetCurrentScoreProfile();

        foreach (GimmickGenerateOptions options in currentEntry.GenerateOptions)
        {
            if (options.Target.Type != type)
                continue;

            currentOptionToReturn = options;

            break;
        }

        return currentOptionToReturn;
    }
}
