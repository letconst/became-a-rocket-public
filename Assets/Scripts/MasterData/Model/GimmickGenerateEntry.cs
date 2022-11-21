using UnityEngine;

namespace LetConst.MasterData
{
    [System.Serializable]
    public class GimmickGenerateEntry
    {
        [HideInInspector]
        public string name;

        [SerializeField, Header("生成する最低高度")]
        private int rangeMin;

        /// <summary>生成する最低高度</summary>
        public int RangeMin => rangeMin;

        [SerializeField, Header("生成する最大高度")]
        private int rangeMax;

        /// <summary>生成する最大高度</summary>
        public int RangeMax => rangeMax;

        [SerializeField, Header("高度内で生成するギミック")]
        private GimmickGenerateOptions[] generateOptions;

        /// <summary>高度内で生成するギミック</summary>
        public GimmickGenerateOptions[] GenerateOptions => generateOptions;
    }
}
