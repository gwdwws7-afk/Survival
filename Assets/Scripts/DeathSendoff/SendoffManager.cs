using System;
using System.Collections.Generic;
using UnityEngine;
using Ravensong.Data;
using Ravensong.Core;
using Ravensong.Einherjar;

namespace Ravensong.DeathSendoff
{
    /// <summary>
    /// 5 签名系统 #5：Death & Send-off（death-sendoff.md + game-concept §3.3 Wyrd）。
    /// 玩家在英灵死亡时选择：送 Valhalla（buff 换）/ 强留（3-5 天腐化）。
    /// </summary>
    public class SendoffManager : MonoBehaviour
    {
        public static SendoffManager Instance { get; private set; }

        [Header("Runtime")]
        public List<ValhallaBuffEntry> permanentBuffs = new();   // 送走的英灵留下的 buff 累加

        public event Action<EinherjarState, SendoffSO> OnSendoffPerformed;
        public event Action<EinherjarState> OnCorruptionTriggered;

        private DataRegistry _registry;
        private GameConfigSO _config;
        private EinherjarManager _einherjar;
        private SettlementManager _settlement;
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
            _einherjar = EinherjarManager.Instance;
            _settlement = SettlementManager.Instance;
            _time = TimeManager.Instance;
            if (_einherjar != null) _einherjar.OnEinherjarDied += OnEinherjarDied;
            if (_time != null) _time.OnDayChanged += OnDayChanged;
        }

        void OnDestroy()
        {
            if (_einherjar != null) _einherjar.OnEinherjarDied -= OnEinherjarDied;
            if (_time != null) _time.OnDayChanged -= OnDayChanged;
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // 英灵死亡事件
        // ============================================================

        private void OnEinherjarDied(EinherjarState dead)
        {
            // 这里通常会弹 UI 让玩家选 Valhalla 还是 Keep
            // 简化处理：默认送 Valhalla（玩家可在 UI 调用 PerformSendoff 改选）
            Debug.Log($"[Sendoff] {dead.displayName} died. Choose: Valhalla or Keep Body? (call PerformSendoff)");
            // 默认 1 帧后自动 Valhalla（避免 demo 阻塞）
            // 实际由 UI 层接管
        }

        // ============================================================
        // 玩家决策
        // ============================================================

        public void PerformSendoff(EinherjarState dead, string sendoffId)
        {
            var sendoff = _registry.GetSendoff(sendoffId);
            if (sendoff == null)
            {
                Debug.LogError($"[Sendoff] SendoffSO '{sendoffId}' not found");
                return;
            }
            if (sendoff.sendoffType == SendoffType.Valhalla)
            {
                PerformValhallaSendoff(dead, sendoff);
            }
            else
            {
                PerformKeepBody(dead, sendoff);
            }
            OnSendoffPerformed?.Invoke(dead, sendoff);
        }

        private void PerformValhallaSendoff(EinherjarState dead, SendoffSO sendoff)
        {
            // 记录永久 buff
            permanentBuffs.Add(new ValhallaBuffEntry
            {
                einherjarId = dead.einherjarId,
                buffId = sendoff.buffId,
                buffStats = sendoff.buffStats
            });
            Debug.Log($"[Sendoff] 🕊 {dead.displayName} sent to Valhalla. Permanent buff: {sendoff.buffId}");

            // 触发衰悼期（聚落 -20% 士气 24h）
            if (_settlement != null && _config != null)
            {
                _settlement.ApplyMourningPenalty(_config.mourningMoralePenalty, _config.mourningHours);
            }
        }

        private void PerformKeepBody(EinherjarState dead, SendoffSO sendoff)
        {
            // 启动强留倒计时
            if (_einherjar != null) _einherjar.StartKeepBody(dead);
            Debug.Log($"[Sendoff] 🪦 {dead.displayName} kept body. Corruption incoming in {_config.keepCorruptionDaysMin}-{_config.keepCorruptionDaysMax} days");
        }

        // ============================================================
        // 日推进：强留腐化检查
        // ============================================================

        private void OnDayChanged(int newDay)
        {
            if (_einherjar == null) return;
            for (int i = _einherjar.activeEinherjars.Count - 1; i >= 0; i--)
            {
                var s = _einherjar.activeEinherjars[i];
                if (!s.isAlive || s.keepBodyDaysRemaining < 0) continue;
                s.keepBodyDaysRemaining--;
                if (s.keepBodyDaysRemaining <= 0)
                {
                    // 尸鬼化
                    s.isAlive = false;
                    s.dayDied = newDay;
                    s.keepBodyDaysRemaining = -1;
                    OnCorruptionTriggered?.Invoke(s);
                    Debug.LogWarning($"[Sendoff] 💀 {s.displayName} 尸鬼化！失去工人 + 反噬");
                }
            }
        }

        // ============================================================
        // 永久 buff 应用（计算叠加值）
        // ============================================================

        public StatBlock AggregatePermanentBuffs()
        {
            var total = StatBlock.Zero;
            foreach (var b in permanentBuffs)
            {
                if (b.buffStats != null) total.Add(b.buffStats);
            }
            return total;
        }
    }

    [Serializable]
    public class ValhallaBuffEntry
    {
        public string einherjarId;
        public string buffId;
        public StatBlock buffStats;
    }
}
