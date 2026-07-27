using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 聚落状态（data-config §C.2 类型 12 + settlement.md）。
    /// 长屋容量 = 8（systems-index §3.5 锁定）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Settlement", fileName = "Settlement")]
    public class SettlementSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 settlement_<name>（通常仅一个 Main 聚落）")]
        public string id;
        public string displayName;

        [Header("Capacity（systems-index §3.5 锁定）")]
        [Tooltip("长屋 L1 = 4 / L2 = 8（v1.0）")]
        [Range(1, 16)] public int maxEinherjarCapacity = 4;
        [Range(1, 8)] public int currentLevel = 1;

        [Header("Buildings")]
        public SettlementBuilding[] buildings;

        [Header("Morale")]
        [Range(0f, 1f)] public float morale = 1f;
        [Tooltip("衰悼期 -20% 持续 24h（systems-index §3.2）")]
        [Range(0f, 1f)] public float mourningPenalty = 0.2f;
        [Range(0, 48)] public int mourningDurationHours = 24;

        [Header("Resources Pool（v1.0 无上限，v1.1 = 1000）")]
        [Tooltip("运行时初始值（player save 重置为 0）")]
        public int initialIron = 0;
        public int initialFood = 0;
        public int initialWood = 0;
        public int initialGrass = 0;
        [Range(0, 999)] public int initialGodEmber = 0;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("SettlementSO.id is required");
            if (maxEinherjarCapacity < 1) errs.Add("maxEinherjarCapacity must be >= 1");
            if (currentLevel < 1 || currentLevel > 5) errs.Add("currentLevel must be 1-5");
            return errs;
        }
    }

    [System.Serializable]
    public class SettlementBuilding
    {
        public string buildingId;        // "long_house" / "forge" / "shrine" / "wall" 等
        public string displayName;
        [Range(0, 5)] public int level = 1;
        public Vector2 gridPosition;     // 网格位置（场景放置用）
        public bool isActive = true;
    }
}
