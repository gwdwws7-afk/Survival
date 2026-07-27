using System;
using System.Collections.Generic;
using UnityEngine;
using Ravensong.Data;
using Ravensong.Core;

namespace Ravensong.Einherjar
{
    /// <summary>
    /// 5 签名系统 #3：Einherjar 居民（einherjar.md + game-concept §3.3）。
    /// 招募、衰老、工作、死亡触发（送别由 SendoffManager 处理）。
    /// </summary>
    public class EinherjarManager : MonoBehaviour
    {
        public static EinherjarManager Instance { get; private set; }

        [Header("Runtime（运行时招募的英灵）")]
        public List<EinherjarState> activeEinherjars = new();
        public event Action<EinherjarState> OnEinherjarRecruited;
        public event Action<EinherjarState> OnEinherjarDied;   // 给 SendoffManager 接管

        private DataRegistry _registry;
        private GameConfigSO _config;
        private TimeManager _time;

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
            if (_time != null) _time.OnDayChanged += OnDayChanged;
        }

        void OnDestroy()
        {
            if (_time != null) _time.OnDayChanged -= OnDayChanged;
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // 招募
        // ============================================================

        public bool TryRecruit(EinherjarSO so)
        {
            if (so == null) return false;
            // 容量检查
            int cap = GetCapacity();
            if (activeEinherjars.Count >= cap)
            {
                Debug.Log($"[Einherjar] Capacity full ({activeEinherjars.Count}/{cap}). Send one off first.");
                return false;
            }
            var state = new EinherjarState
            {
                einherjarId = so.id,
                displayName = so.displayName,
                profession = so.profession,
                workType = so.workType,
                efficiency = so.workEfficiency,
                willDie = so.willDie,
                daysToDeath = so.willDie ? UnityEngine.Random.Range(so.daysToDeath, so.daysToDeath + 1) : -1,
                dayRecruited = _time != null ? _time.totalDays : 0,
                isAlive = true
            };
            activeEinherjars.Add(state);
            OnEinherjarRecruited?.Invoke(state);
            Debug.Log($"[Einherjar] + {so.displayName} ({so.profession}), daysToDeath={state.daysToDeath}, eff={state.efficiency:F2}");
            return true;
        }

        // ============================================================
        // 日推进（每游戏日调用）
        // ============================================================

        private void OnDayChanged(int newDay)
        {
            for (int i = activeEinherjars.Count - 1; i >= 0; i--)
            {
                var s = activeEinherjars[i];
                if (!s.isAlive) continue;
                s.daysAlive++;

                // 工作产出（1-3 单位/小时 → 按 dayLength 折算）
                ProduceResources(s);

                // 死亡检查
                if (s.willDie && s.daysAlive >= s.daysToDeath)
                {
                    s.isAlive = false;
                    s.dayDied = newDay;
                    Debug.Log($"[Einherjar] ☠ {s.displayName} died on day {newDay} (was scheduled for {s.daysToDeath} days)");
                    OnEinherjarDied?.Invoke(s);
                }
            }
        }

        private void ProduceResources(EinherjarState s)
        {
            if (_config == null) return;
            // 简化：每游戏日按 workEfficiency × 单位/小时区间 产出
            float min = _config.einherjarWorkMinUnitsPerHour;
            float max = _config.einherjarWorkMaxUnitsPerHour;
            int produced = Mathf.RoundToInt(s.efficiency * UnityEngine.Random.Range(min, max + 1) * 24f);
            // TODO: 通过 SettlementManager.AddResource(workType, produced)
            Debug.Log($"[Einherjar] {s.displayName} produced {produced} {s.workType}");
        }

        // ============================================================
        // 强留倒计时（SendoffManager 通知时启动）
        // ============================================================

        public void StartKeepBody(EinherjarState s)
        {
            if (_config == null) return;
            s.keepBodyDaysRemaining = UnityEngine.Random.Range(
                _config.keepCorruptionDaysMin, _config.keepCorruptionDaysMax + 1);
            Debug.Log($"[Einherjar] {s.displayName} kept body, corruption in {s.keepBodyDaysRemaining} days");
        }

        // ============================================================
        // 容量查询
        // ============================================================

        public int GetCapacity()
        {
            if (_config == null) return 4;
            // 简化：L1 默认；后续可读 SettlementSO.currentLevel
            return _config.longHouseL1Capacity;
        }

        public int GetAliveCount() => activeEinherjars.FindAll(e => e.isAlive).Count;

        public EinherjarState GetById(string id) => activeEinherjars.Find(e => e.einherjarId == id);
    }

    /// <summary>运行时英灵状态（不存为 SO）</summary>
    [Serializable]
    public class EinherjarState
    {
        public string einherjarId;        // EinherjarSO.id
        public string displayName;
        public Profession profession;
        public ResourceType workType;
        public float efficiency;
        public bool willDie;
        public int daysToDeath;           // 招募时算
        public int dayRecruited;
        public int daysAlive;
        public bool isAlive;
        public int dayDied;
        public int keepBodyDaysRemaining = -1;   // 强留倒计时，-1 = 未强留
    }
}
