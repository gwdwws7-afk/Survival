using System;
using System.Collections.Generic;
using UnityEngine;
using Ravensong.Data;
using Ravensong.Core;

namespace Ravensong.Settlement
{
    /// <summary>
    /// 聚落管理（settlement.md + systems-index §3.5）。
    /// 资源池（4 种 + god-ember）、长屋容量、士气、衰悼期、仓储。
    /// </summary>
    public class SettlementManager : MonoBehaviour
    {
        public static SettlementManager Instance { get; private set; }

        [Header("Resources (systems-index §3.2 锁定)")]
        public int iron = 0;
        public int food = 0;
        public int wood = 0;
        public int grass = 0;
        public int godEmber = 0;

        [Header("Warehouse")]
        public int warehouseSlotsUsed = 0;
        public int warehouseLevel = 1;

        [Header("Morale")]
        [Range(0f, 1f)] public float morale = 1f;
        [Tooltip("衰悼期剩余小时数")]
        public float mourningRemainingHours = 0f;

        [Header("Buildings")]
        public List<SettlementBuilding> buildings = new();

        public event Action<ResourceType, int> OnResourceChanged;
        public event Action<float> OnMoraleChanged;

        private DataRegistry _registry;
        private GameConfigSO _config;
        private TimeManager _time;
        private EinherjarManager _einherjar;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            _registry = DataRegistry.Instance;
            if (_registry != null) _config = _registry.gameConfig;
            _time = TimeManager.Instance;
            _einherjar = EinherjarManager.Instance;
            if (_time != null) _time.OnDayChanged += OnDayChanged;
        }

        void OnDestroy()
        {
            if (_time != null) _time.OnDayChanged -= OnDayChanged;
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // 资源池
        // ============================================================

        public int GetResource(ResourceType t) => t switch
        {
            ResourceType.Iron => iron,
            ResourceType.Food => food,
            ResourceType.Wood => wood,
            ResourceType.Grass => grass,
            ResourceType.GodEmber => godEmber,
            _ => 0
        };

        public void AddResource(ResourceType t, int amount)
        {
            int newVal = GetResource(t) + amount;
            if (t == ResourceType.GodEmber && _config != null)
                newVal = Mathf.Clamp(newVal, 0, _config.godEmberMax);
            SetResource(t, newVal);
        }

        public bool TryConsume(ResourceType t, int amount)
        {
            if (GetResource(t) < amount) return false;
            SetResource(t, GetResource(t) - amount);
            return true;
        }

        private void SetResource(ResourceType t, int v)
        {
            switch (t)
            {
                case ResourceType.Iron: iron = v; break;
                case ResourceType.Food: food = v; break;
                case ResourceType.Wood: wood = v; break;
                case ResourceType.Grass: grass = v; break;
                case ResourceType.GodEmber: godEmber = v; break;
            }
            OnResourceChanged?.Invoke(t, v);
        }

        // ============================================================
        // 衰悼期
        // ============================================================

        public void ApplyMourningPenalty(float penaltyFraction, int hours)
        {
            morale = Mathf.Max(0f, morale - penaltyFraction);
            mourningRemainingHours += hours;
            OnMoraleChanged?.Invoke(morale);
            Debug.Log($"[Settlement] Mourning: -{(penaltyFraction * 100):F0}% morale for {hours}h, current morale={morale:F2}");
        }

        // ============================================================
        // 日推进：士气自然恢复 + 衰悼期倒计时
        // ============================================================

        private void OnDayChanged(int newDay)
        {
            if (_config == null) return;
            // 衰悼期倒计时
            if (mourningRemainingHours > 0f)
            {
                mourningRemainingHours = Mathf.Max(0f, mourningRemainingHours - 24f);
                if (mourningRemainingHours == 0f)
                    Debug.Log("[Settlement] Mourning period ended");
            }
            // 士气自然恢复
            morale = Mathf.Min(_config.moraleMax, morale + _config.moraleRegenPerHour * 24f);
            OnMoraleChanged?.Invoke(morale);
        }

        // ============================================================
        // 仓储
        // ============================================================

        public int GetWarehouseCapacity()
        {
            if (_config == null) return 24;
            return warehouseLevel >= 2 ? _config.warehouseL2Slots : _config.warehouseL1Slots;
        }

        public bool HasWarehouseSpace() => warehouseSlotsUsed < GetWarehouseCapacity();

        // ============================================================
        // 容量（长屋）
        // ============================================================

        public int GetLongHouseCapacity()
        {
            if (_config == null) return 4;
            // 简化：L1=4 / L2=8 由 warehouseLevel 推断
            return warehouseLevel >= 2 ? _config.longHouseL2Capacity : _config.longHouseL1Capacity;
        }
    }
}
