using UnityEngine;

[CreateAssetMenu(fileName = "NewGimmickData", menuName = "マスターデータ/ギミック", order = 1)]
public sealed class MasterGimmick : MasterDataBaseWithId
{
    [SerializeField, Header(MasterDataModelConstants.Gimmick.TypeLabel)]
    private GimmickType type;

    /// <summary>ギミックの種類</summary>
    public GimmickType Type => type;

    [SerializeField, Header(MasterDataModelConstants.Gimmick.PrefabLabel)]
    private GameObject prefab;

    /// <summary>プレハブ</summary>
    public GameObject Prefab => prefab;
}
