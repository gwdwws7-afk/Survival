using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 誓言（game-concept §4 + oath-system.md + data-config §C.2 类型 5）。
    /// 5 誓言：锻冶/炉火/荒野/亡者/苍穹。
    /// 4 誓言完成 → 苍穹解锁 → 5 誓言完成 → 奥丁审判（终局）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Oath", fileName = "Oath")]
    public class OathSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 oath_<name>")]
        public string id;
        public OathType oathType = OathType.Forge;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Milestones (4-5 per oath)")]
        public OathMilestone[] milestones;
        [Tooltip("终极奖励（解锁什么内容）")]
        [TextArea] public string ultimateReward;

        [Header("Final (only for Canopy 苍穹)")]
        [Tooltip("苍穹之誓专属：true 触发奥丁审判（终局）")]
        public bool triggersOdinJudgment = false;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("OathSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^oath_[a-z0-9_]+$"))
                errs.Add($"OathSO.id '{id}' must match pattern oath_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("OathSO.displayName is required");
            if (milestones == null || milestones.Length < 4 || milestones.Length > 5)
                errs.Add($"OathSO.milestones must be 4-5, got {(milestones?.Length ?? 0)}");
            if (oathType == OathType.Canopy && !triggersOdinJudgment)
                errs.Add("Canopy oath must have triggersOdinJudgment=true");
            return errs;
        }
    }

    /// <summary>誓言 milestone</summary>
    [System.Serializable]
    public class OathMilestone
    {
        public string milestoneId;        // "weave_100" / "send_5_einherjars" 等
        public string displayName;
        [TextArea] public string description;
        [Tooltip("进度目标（按 milestoneId 解释）")]
        public int target = 1;
        [Tooltip("解锁后给的奖励（buff / 配方 / 建筑等）")]
        public string rewardId;
    }
}
