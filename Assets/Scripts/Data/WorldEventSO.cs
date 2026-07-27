using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 世界事件（quest-event.md + data-config §C.2 类型 7）。
    /// 编织前乌鸦占卜、聚落争吵、新到奥丁诏令 等。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/WorldEvent", fileName = "WorldEvent")]
    public class WorldEventSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 event_<name>")]
        public string id;
        public string displayName;
        [TextArea] public string description;

        [Header("Trigger")]
        [Tooltip("触发条件（"第 N 天" / "编织 X 次后" / "随机 0.05/小时"）")]
        public string triggerCondition;
        [Range(0f, 1f)] public float triggerChance = 0.05f;

        [Header("Effects")]
        [Tooltip("事件触发的效果（字符串 ID 解释）")]
        public string[] effectIds;
        [Tooltip("事件持续时间（小时）")]
        [Range(0, 48)] public int durationHours = 1;

        [Header("Dialogue (optional)")]
        public string dialogueId;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("WorldEventSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^event_[a-z0-9_]+$"))
                errs.Add($"WorldEventSO.id '{id}' must match pattern event_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("WorldEventSO.displayName is required");
            return errs;
        }
    }
}
