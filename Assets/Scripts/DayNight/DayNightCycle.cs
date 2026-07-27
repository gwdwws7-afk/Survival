using UnityEngine;
using Ravensong.Data;
using Ravensong.Core;

namespace Ravensong.DayNight
{
    /// <summary>
    /// 5 签名系统 #1：Day-Night Cycle / Waxing Moon（day-night-cycle.md）。
    /// 监听 TimeManager 的段变化，把 debuff/buff 应用到玩家。
    /// </summary>
    public class DayNightCycle : MonoBehaviour
    {
        public static DayNightCycle Instance { get; private set; }

        [Header("Player 状态（按 debuff/buff 应用）")]
        public float currentMoveSpeedMultiplier = 1f;
        public float currentVisionMultiplier = 1f;
        public float currentWeaveTimeMultiplier = 1f;  // > 1 = 更慢

        public event System.Action<DayPhase> OnBuffsChanged;

        private TimeManager _time;
        private GameConfigSO _config;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            _time = TimeManager.Instance;
            if (_time != null) _time.OnPhaseChanged += OnPhaseChanged;
            if (DataRegistry.Instance != null) _config = DataRegistry.Instance.gameConfig;
            Recompute();
        }

        void OnDestroy()
        {
            if (_time != null) _time.OnPhaseChanged -= OnPhaseChanged;
            if (Instance == this) Instance = null;
        }

        private void OnPhaseChanged(DayPhase p) => Recompute();

        /// <summary>根据当前段重算 buff/debuff（game-concept §3.2 锁定数值）</summary>
        public void Recompute()
        {
            if (_config == null || _time == null) return;
            bool isDay = _time.IsDaytime;
            bool isNight = _time.IsNighttime;

            // 基础 = 1.0
            currentMoveSpeedMultiplier = 1f;
            currentVisionMultiplier = 1f;
            currentWeaveTimeMultiplier = 1f;

            if (isDay)
            {
                // 白天 debuff
                currentMoveSpeedMultiplier -= _config.dayMoveSpeedPenalty;
                currentVisionMultiplier -= _config.dayVisionPenalty;
                currentWeaveTimeMultiplier += _config.dayWeavePenalty;
            }
            else if (isNight)
            {
                // 夜晚 buff
                currentMoveSpeedMultiplier += _config.nightMoveSpeedBonus;
                currentWeaveTimeMultiplier -= _config.nightWeaveTimeReduction;
            }

            OnBuffsChanged?.Invoke(_time.CurrentPhase);
            Debug.Log($"[DayNight] Recompute: phase={_time.CurrentPhase}, moveMult={currentMoveSpeedMultiplier:F2}, visionMult={currentVisionMultiplier:F2}, weaveMult={currentWeaveTimeMultiplier:F2}");
        }

        // ============================================================
        // 状态查询
        // ============================================================

        public bool IsDaytime() => _time != null && _time.IsDaytime;
        public bool IsNighttime() => _time != null && _time.IsNighttime;
        public DayPhase CurrentPhase() => _time != null ? _time.CurrentPhase : DayPhase.Day;
        public int MoonPhase() => _time != null ? _time.moonPhase : 0;

        /// <summary>Waxing Moon 月相加成（占位；具体加成按 8 段表）</summary>
        public string GetMoonPhaseName()
        {
            switch (MoonPhase())
            {
                case 0: return "新月";
                case 1: return "蛾眉月";
                case 2: return "上弦月";
                case 3: return "盈凸月";
                case 4: return "满月";
                case 5: return "亏凸月";
                case 6: return "下弦月";
                case 7: return "残月";
                default: return "未知";
            }
        }
    }
}
