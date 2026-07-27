using System;
using UnityEngine;
using Ravensong.Data;

namespace Ravensong.Core
{
    /// <summary>
    /// 日夜时钟（day-night-cycle.md + game-concept §3.2 Waxing Moon）。
    /// 由 DataRegistry 注入 GameConfigSO 的 dayLengthSeconds 和段比例。
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Header("Runtime")]
        [Tooltip("一天内已过秒数（0 ~ dayLengthSeconds）")]
        public float dayProgressSeconds = 0f;
        [Tooltip("Waxing Moon 月相（0-7，8 段）")]
        public int moonPhase = 0;
        [Tooltip("经过的总游戏天数（从 1 起）")]
        public int totalDays = 1;

        public event Action<DayPhase> OnPhaseChanged;
        public event Action<int> OnMoonPhaseChanged;
        public event Action<int> OnDayChanged;

        private DayPhase _currentPhase = DayPhase.Dawn;
        private GameConfigSO _config;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (DataRegistry.Instance != null)
                _config = DataRegistry.Instance.gameConfig;
        }

        void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.state != GameState.Playing) return;
            if (_config == null) return;

            float dayLength = _config.dayLengthSeconds;
            dayProgressSeconds += Time.deltaTime;

            // 跨天
            if (dayProgressSeconds >= dayLength)
            {
                dayProgressSeconds -= dayLength;
                totalDays++;
                OnDayChanged?.Invoke(totalDays);
                // 月相 8 段循环
                moonPhase = (moonPhase + 1) % 8;
                OnMoonPhaseChanged?.Invoke(moonPhase);
            }

            // 段切换
            var newPhase = ComputePhase(dayProgressSeconds / dayLength);
            if (newPhase != _currentPhase)
            {
                _currentPhase = newPhase;
                OnPhaseChanged?.Invoke(_currentPhase);
            }
        }

        /// <summary>根据 0-1 进度算 6 段之一（dawn/day/dusk/night/midnight/deepnight）</summary>
        public DayPhase ComputePhase(float t)
        {
            if (_config == null) return DayPhase.Day;
            float acc = 0f;
            acc += _config.dayDawnRatio;       if (t < acc) return DayPhase.Dawn;
            acc += _config.dayRatio;          if (t < acc) return DayPhase.Day;
            acc += _config.dayDuskRatio;      if (t < acc) return DayPhase.Dusk;
            acc += _config.nightRatio;        if (t < acc) return DayPhase.Night;
            acc += _config.midnightRatio;     if (t < acc) return DayPhase.Midnight;
            return DayPhase.DeepNight;
        }

        /// <summary>是否白天（用于 debuff 触发）</summary>
        public bool IsDaytime => _currentPhase == DayPhase.Day || _currentPhase == DayPhase.Dusk;

        /// <summary>是否夜晚（用于 buff + boss 战）</summary>
        public bool IsNighttime => _currentPhase == DayPhase.Night || _currentPhase == DayPhase.Midnight;

        /// <summary>当前段（运行时查询）</summary>
        public DayPhase CurrentPhase => _currentPhase;

        // 速度调节（debug + new player 锁定）
        public void SetTimeScale(float scale) => Time.timeScale = scale;

        public void AdvanceDays(int days)
        {
            totalDays += days;
            OnDayChanged?.Invoke(totalDays);
        }
    }

    public enum DayPhase
    {
        Dawn = 0,       // 黎明
        Day = 1,        // 日
        Dusk = 2,       // 暮
        Night = 3,      // 夜
        Midnight = 4,   // 午夜
        DeepNight = 5   // 深宵
    }
}
