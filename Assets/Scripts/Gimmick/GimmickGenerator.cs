using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LetConst.MasterData;
using UniRx;
using UnityEngine;

/// <summary>
/// ギミックの生成を制御するクラス
/// </summary>
public sealed class GimmickGenerator : MonoBehaviour
{
    #region Variables

    [SerializeField, Header("一度に生成する領域の高さ")]
    private int heightPerGeneration;

    [SerializeField, Header("ギミックの生成を開始する高さ (y)")]
    private int generationHeightOffset;

    private GimmickManager _gimmickManager;

    /// <summary>最後に生成判定が行われた高度</summary>
    private int _generatedHeight;

    /// <summary>現在高度での生成ギミックのプロファイル</summary>
    private GimmickGenerateEntry _generateTargetProfile;

    // TODO: キーをenumじゃなくする
    /// <summary>各種ギミックの最終生成高度</summary>
    private readonly Dictionary<GimmickType, int> _lastGeneratedHeights = new();

    #endregion

    private async UniTaskVoid Start()
    {
        _gimmickManager  = GimmickManager.Instance;
        _generatedHeight = generationHeightOffset;

        await _gimmickManager.WaitForReady();

        // 初期生成
        Generate(heightPerGeneration);

        EventReceiver();
    }

    /// <summary>
    /// 各種ギミックの生成を行う。
    /// </summary>
    /// <param name="generationHeight">生成を行う領域の高さ</param>
    private void Generate(int generationHeight)
    {
        for (int i = 0; i < generationHeight; i++)
        {
            // 生成予定高度での生成プロファイルを取得
            _generateTargetProfile = _gimmickManager.GetGenerateProfileByHeight(_generatedHeight + i);
            GimmickGenerateOptions[] generateOptions = _generateTargetProfile.GenerateOptions;

            foreach (GimmickGenerateOptions genOption in generateOptions)
            {
                MasterGimmick genTargetData = genOption.Target;

                if (!genTargetData)
                    continue;

                GimmickInitializeOptions initOption = genOption.InitializeOptions.GetRandom();

                float   x        = GimmickUtility.GetXPosition(initOption.XPosition);
                float   y        = _generatedHeight + i;
                Vector2 position = new(x, y);

                float lastGenPosDiff;

                // 最終生成高度があるなら、生成予定地からの差分を計算
                if (_lastGeneratedHeights.TryGetValue(genTargetData.Type, out int lastGenHeight))
                {
                    lastGenPosDiff = y - lastGenHeight;
                }
                // ないなら項目を追加し、最終生成高度を最低生成高度として初期化
                else
                {
                    _gimmickManager.Pool.RegisterPoolTarget(genTargetData.Type, genTargetData.Prefab);
                    _lastGeneratedHeights.Add(genTargetData.Type, 0);
                    lastGenPosDiff = genOption.GenerationMinInterval;
                }

                // このギミックが以前の生成位置から指定間隔生成されていなければ必ず生成
                if (genOption.GenerationMaxInterval > lastGenPosDiff)
                {
                    // 確率判定
                    if (!GimmickUtility.JudgeProbability(genOption.GenerationFrequency))
                        continue;

                    // このギミックの以前の生成位置が最小間隔以上のときのみ生成
                    if (lastGenPosDiff < genOption.GenerationMinInterval)
                        continue;
                }

                GameObject newGimmick = _gimmickManager.Pool.Rent(genTargetData.Type);
                newGimmick.transform.position = position;

                // 初期化処理があれば呼び出す
                newGimmick.GetComponent<IInitializableGimmick>()?.InitializeOnGenerate(initOption, position);

                // このギミックの生成位置を記憶
                _lastGeneratedHeights[genTargetData.Type] = (int) y;
            }
        }

        _generatedHeight += heightPerGeneration;
    }

    private void EventReceiver()
    {
        // プレイヤーの高さ変更イベントを受付
        // プレイヤーの高さが一定以上になったらギミックを生成する
        ScoreManager.Instance.CurrentScoreChanged.Subscribe(OnScoreChanged).AddTo(this);
    }

    private void OnScoreChanged(int score)
    {
        float halfHeight = heightPerGeneration / 2f;

        // 生成可能な高さを超えているときのみ処理
        if (_generatedHeight - halfHeight > score)
            return;

        Generate(heightPerGeneration);
    }
}
