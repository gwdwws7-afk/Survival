using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// Boss 阶段详细设计（data-config §C.2 类型 17 + boss-design.md）。
    /// P0 修复：4 Boss 详细设计（handover §5）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/BossDetail", fileName = "BossDetail")]
    public class BossDetailSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 bossdetail_<name>")]
        public string id;
        public string displayName;

        [Header("Phases (3-4 phase)")]
        public BossPhase[] phases;
        [Tooltip("phase 切换 HP 阈值百分比（升序）")]
        public float[] phaseThresholds = { 0.66f, 0.33f };

        [Header("Mechanics")]
        [Tooltip("boss 特殊机制（"召唤" / "狂暴" / "分阶段攻击范围扩大" 等）")]
        [TextArea] public string mechanicDescription;

        [Header("Lore (死亡时)")]
        [TextArea(3, 6)] public string deathQuote;
        public string valhallaConnectionEinherjarId;   // 这个 boss 死后会变成英灵（可选）

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("BossDetailSO.id is required");
            if (phases == null || phases.Length < 1) errs.Add("BossDetailSO.phases must be >= 1");
            return errs;
        }
    }

    [System.Serializable]
    public class BossPhase
    {
        public string phaseName;
        [TextArea] public string description;
        [Tooltip("本阶段攻击模式 ID（喂给 BT）")]
        public string[] attackPatternIds;
        [Range(0f, 2f)] public float damageMultiplier = 1f;
        [Range(0f, 2f)] public float speedMultiplier = 1f;
    }
}
