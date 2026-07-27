using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 世界 Boss 基础（data-config §C.2 类型 6）。
    /// 4-5 个 boss，分布在 4-5 群系。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Boss", fileName = "Boss")]
    public class BossSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 boss_<name>")]
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite portrait;

        [Header("Location")]
        public string biomeId;        // 出现的群系
        public BiomeId biomeHint = BiomeId.BirchGrove;

        [Header("Stats")]
        [Range(100, 10000)] public int maxHP = 1000;
        [Range(10f, 200f)] public float baseAttackDamage = 50f;
        [Range(0f, 100f)] public float baseDefense = 20f;
        [Range(0f, 1f)] public float critChance = 0.1f;

        [Header("Loot")]
        [Tooltip("战利品 ItemSO ID（Tier 4 装备）")]
        public string guaranteedLootId;
        [Tooltip("战利品 ItemSO ID 池（随机 1 个）")]
        public string[] possibleLootIds;
        [Range(0, 999)] public int godEmberReward = 50;

        [Header("Cooldown (避免刷)")]
        [Tooltip("击败后冷却小时数")]
        [Range(0, 168)] public int respawnCooldownHours = 24;

        [Header("Phases (详细)")]
        public BossDetailSO detail;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("BossSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^boss_[a-z0-9_]+$"))
                errs.Add($"BossSO.id '{id}' must match pattern boss_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("BossSO.displayName is required");
            if (maxHP < 100) errs.Add("BossSO.maxHP must be >= 100");
            return errs;
        }
    }
}
