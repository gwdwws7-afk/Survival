using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 远征任务（data-config §C.2 类型 13 + world-exploration.md）。
    /// 5 分钟循环的夜间微任务（game-concept §4）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Expedition", fileName = "Expedition")]
    public class ExpeditionSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 expedition_<name>")]
        public string id;
        public string displayName;
        [TextArea] public string description;

        [Header("Type")]
        [Tooltip("patrol / hunt / gather / sanctuary（game-concept §4 5分钟循环）")]
        public string expeditionType;
        [Tooltip("目标群系")]
        public string biomeId;

        [Header("Risk/Reward")]
        [Range(0f, 1f)] public float baseRisk = 0.3f;
        [Tooltip("奖励（物品 ID + 期望数量）")]
        public ItemStack[] expectedLoot;
        [Tooltip("奖励：英灵招募机会（0 = 无）")]
        [Range(0f, 1f)] public float einherjarRecruitChance = 0f;

        [Header("Duration")]
        [Tooltip("游戏内时长（小时）")]
        [Range(0.5f, 12f)] public float durationHours = 2f;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("ExpeditionSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^expedition_[a-z0-9_]+$"))
                errs.Add($"ExpeditionSO.id '{id}' must match pattern expedition_<name>");

            if (string.IsNullOrEmpty(expeditionType)) errs.Add("expeditionType is required");
            return errs;
        }
    }
}
