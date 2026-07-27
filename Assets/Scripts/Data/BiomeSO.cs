using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 生物群系（world-exploration.md + data-config §C.2 类型 4）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Biome", fileName = "Biome")]
    public class BiomeSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 biome_<name>")]
        public string id;
        public BiomeId biomeId = BiomeId.BirchGrove;
        public string displayName;
        [TextArea] public string description;

        [Header("Art Anchors")]
        [Tooltip("主场景图（style-bible 锁定）")]
        public Sprite mainScene;
        public Sprite[] layeredParallax; // 视差层（back/mid/front）

        [Header("Resources")]
        [Tooltip("群系可产资源 ID + 单位/小时")]
        public ResourceYield[] resourceYields;
        [Tooltip("群系难度系数（0.5=易, 1.0=中, 1.5=难）")]
        [Range(0.5f, 2f)] public float difficultyFactor = 1f;

        [Header("Hazard")]
        [Tooltip("群系环境伤害（HP/分钟，永冻崖寒冷等）")]
        [Range(0f, 5f)] public float hazardDPS = 0f;
        public string hazardDescription;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("BiomeSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^biome_[a-z0-9_]+$"))
                errs.Add($"BiomeSO.id '{id}' must match pattern biome_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("BiomeSO.displayName is required");
            return errs;
        }
    }

    /// <summary>群系资源产量条目</summary>
    [System.Serializable]
    public class ResourceYield
    {
        public string itemId;        // ItemSO.id
        [Range(0f, 10f)] public float unitsPerHour = 1f;
        [Range(0f, 1f)] public float chanceToSpawn = 1f;
    }
}
