using System.Collections.Generic;
using LetConst.MasterData;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGimmickGenerationData", menuName = "マスターデータ/ギミック生成", order = 1)]
public sealed class MasterGimmickGeneration : MasterDataBase
{
    [SerializeField, Header(MasterDataModelConstants.Gimmick.GenerateOptionsLabel)]
    private GimmickGenerateEntry[] generateGimmickEntries;

    /// <summary>生成設定</summary>
    public IEnumerable<GimmickGenerateEntry> GenerateGimmickEntries => generateGimmickEntries;

    private void OnValidate()
    {
        // 各種配列の要素名の設定処理
        foreach (GimmickGenerateEntry entry in generateGimmickEntries)
        {
            entry.name = $"{entry.RangeMin}m ～ {entry.RangeMax}m";

            foreach (GimmickGenerateOptions option in entry.GenerateOptions)
            {
                option.name = option.Target?.name ?? "ギミック未指定";
            }
        }
    }
}
