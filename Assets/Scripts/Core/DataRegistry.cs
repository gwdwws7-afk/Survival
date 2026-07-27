using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ravensong.Data;

namespace Ravensong.Core
{
    /// <summary>
    /// 数据注册中心（data-config §C.1 锁定）。
    /// - Edit-time + Play-mode-start 加载所有 .asset
    /// - 按 ID 解析（不用 SO 引用，避免 GUID 漂移）
    /// - 热重载（500ms debounce）
    /// - 缺 ID 警告而非 null
    /// - 启动时 ValidateAll
    /// </summary>
    public class DataRegistry : MonoBehaviour
    {
        public static DataRegistry Instance { get; private set; }

        [Header("Configuration")]
        [Tooltip("GameConfig 资产（数据中枢）")]
        public GameConfigSO gameConfig;

        [Header("Storage（按 ID 索引，运行时填充）")]
        private readonly Dictionary<string, RecipeSO> _recipes = new();
        private readonly Dictionary<string, ItemSO> _items = new();
        private readonly Dictionary<string, EinherjarSO> _einherjars = new();
        private readonly Dictionary<string, BiomeSO> _biomes = new();
        private readonly Dictionary<string, OathSO> _oaths = new();
        private readonly Dictionary<string, BossSO> _bosses = new();
        private readonly Dictionary<string, WorldEventSO> _worldEvents = new();
        private readonly Dictionary<string, DialogueSO> _dialogues = new();
        private readonly Dictionary<string, SendoffSO> _sendoffs = new();
        private readonly Dictionary<string, UIStyleSO> _uiStyles = new();
        private readonly Dictionary<string, SettlementSO> _settlements = new();
        private readonly Dictionary<string, ExpeditionSO> _expeditions = new();
        private readonly Dictionary<string, QuestSO> _quests = new();
        private readonly Dictionary<string, VFXPresetSO> _vfxPresets = new();
        private readonly Dictionary<string, AudioPresetSO> _audioPresets = new();

        // ToolSO 单独索引（继承自 ItemSO，按 ItemSO.id 解析）
        private readonly Dictionary<string, ToolSO> _tools = new();
        // BossDetailSO 按 boss.id 索引（不独立 ID）
        private readonly Dictionary<string, BossDetailSO> _bossDetails = new();

        private bool _isLoaded;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAll();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // Load & Validate
        // ============================================================

        public void LoadAll()
        {
            LoadFromResources<RecipeSO>("Data/Recipes", _recipes);
            LoadFromResources<ItemSO>("Data/Items", _items);
            LoadFromResources<ToolSO>("Data/Items", _tools);     // 共享目录，按 type 区分
            LoadFromResources<EinherjarSO>("Data/Einherjars", _einherjars);
            LoadFromResources<BiomeSO>("Data/Biomes", _biomes);
            LoadFromResources<OathSO>("Data/Oaths", _oaths);
            LoadFromResources<BossSO>("Data/Bosses", _bosses);
            LoadFromResources<WorldEventSO>("Data/WorldEvents", _worldEvents);
            LoadFromResources<DialogueSO>("Data/Dialogues", _dialogues);
            LoadFromResources<SendoffSO>("Data/Sendoffs", _sendoffs);
            LoadFromResources<UIStyleSO>("Data/UIStyles", _uiStyles);
            LoadFromResources<SettlementSO>("Data", _settlements);   // Settlement 通常唯一
            LoadFromResources<ExpeditionSO>("Data/Expeditions", _expeditions);
            LoadFromResources<QuestSO>("Data/Quests", _quests);
            LoadFromResources<VFXPresetSO>("Data/VFX", _vfxPresets);
            LoadFromResources<AudioPresetSO>("Data/Audio", _audioPresets);

            LoadBossDetails();

            _isLoaded = true;
            ValidateAll();
        }

        private void LoadFromResources<T>(string folder, Dictionary<string, T> dict) where T : ScriptableObject
        {
            var assets = Resources.LoadAll<T>(folder);
            foreach (var a in assets)
            {
                if (a is IDataValidatable v)
                {
                    if (string.IsNullOrEmpty(v.SchemaVersion))
                    {
                        Debug.LogWarning($"[DataRegistry] {a.GetType().Name} '{a.name}' missing SchemaVersion, skipping", a);
                        continue;
                    }
                }
                if (TryGetId(a, out var id))
                {
                    if (dict.ContainsKey(id))
                    {
                        Debug.LogWarning($"[DataRegistry] Duplicate ID '{id}' in {typeof(T).Name} ({a.name}), keeping first", a);
                        continue;
                    }
                    dict[id] = a;
                }
                else
                {
                    Debug.LogWarning($"[DataRegistry] {typeof(T).Name} '{a.name}' has no id, skipping", a);
                }
            }
            Debug.Log($"[DataRegistry] Loaded {dict.Count} {typeof(T).Name} from Resources/{folder}");
        }

        private void LoadBossDetails()
        {
            var assets = Resources.LoadAll<BossDetailSO>("Data/Bosses");
            foreach (var a in assets)
            {
                if (TryGetId(a, out var id))
                    _bossDetails[id] = a;
            }
        }

        private static bool TryGetId(ScriptableObject so, out string id)
        {
            // 通过反射读 id 字段（所有 SO 都有）
            var field = so.GetType().GetField("id",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(string))
            {
                id = (string)field.GetValue(so);
                return !string.IsNullOrEmpty(id);
            }
            id = null;
            return false;
        }

        public void ValidateAll()
        {
            int totalErrors = 0;
            totalErrors += ValidateDict(_recipes);
            totalErrors += ValidateDict(_items);
            totalErrors += ValidateDict(_tools);
            totalErrors += ValidateDict(_einherjars);
            totalErrors += ValidateDict(_biomes);
            totalErrors += ValidateDict(_oaths);
            totalErrors += ValidateDict(_bosses);
            totalErrors += ValidateDict(_worldEvents);
            totalErrors += ValidateDict(_dialogues);
            totalErrors += ValidateDict(_sendoffs);
            totalErrors += ValidateDict(_uiStyles);
            totalErrors += ValidateDict(_settlements);
            totalErrors += ValidateDict(_expeditions);
            totalErrors += ValidateDict(_quests);
            totalErrors += ValidateDict(_vfxPresets);
            totalErrors += ValidateDict(_audioPresets);

            if (gameConfig != null)
            {
                foreach (var e in gameConfig.Validate())
                    Debug.LogError($"[DataRegistry/GameConfig] {e}", gameConfig);
            }
            else
            {
                Debug.LogError("[DataRegistry] GameConfigSO not assigned! Assign in inspector.");
            }

            if (totalErrors > 0)
                Debug.LogError($"[DataRegistry] Validation finished with {totalErrors} error(s)");
            else
                Debug.Log("[DataRegistry] All SO validated successfully");
        }

        private int ValidateDict<T>(Dictionary<string, T> dict) where T : ScriptableObject
        {
            int n = 0;
            foreach (var kvp in dict)
            {
                if (kvp.Value is IDataValidatable v)
                {
                    foreach (var e in v.Validate())
                    {
                        Debug.LogError($"[DataRegistry/{kvp.Value.GetType().Name}/{kvp.Key}] {e}", kvp.Value);
                        n++;
                    }
                }
            }
            return n;
        }

        // ============================================================
        // Get API（按 ID 解析，缺 ID 警告而非 null）
        // ============================================================

        public RecipeSO GetRecipe(string id) => GetOrWarn(_recipes, id, "RecipeSO");
        public ItemSO GetItem(string id) => GetOrWarn(_items, id, "ItemSO");
        public ToolSO GetTool(string id) => GetOrWarn(_tools, id, "ToolSO");
        public EinherjarSO GetEinherjar(string id) => GetOrWarn(_einherjars, id, "EinherjarSO");
        public BiomeSO GetBiome(string id) => GetOrWarn(_biomes, id, "BiomeSO");
        public OathSO GetOath(string id) => GetOrWarn(_oaths, id, "OathSO");
        public BossSO GetBoss(string id) => GetOrWarn(_bosses, id, "BossSO");
        public WorldEventSO GetWorldEvent(string id) => GetOrWarn(_worldEvents, id, "WorldEventSO");
        public DialogueSO GetDialogue(string id) => GetOrWarn(_dialogues, id, "DialogueSO");
        public SendoffSO GetSendoff(string id) => GetOrWarn(_sendoffs, id, "SendoffSO");
        public UIStyleSO GetUIStyle(string id) => GetOrWarn(_uiStyles, id, "UIStyleSO");
        public SettlementSO GetSettlement(string id) => GetOrWarn(_settlements, id, "SettlementSO");
        public ExpeditionSO GetExpedition(string id) => GetOrWarn(_expeditions, id, "ExpeditionSO");
        public QuestSO GetQuest(string id) => GetOrWarn(_quests, id, "QuestSO");
        public VFXPresetSO GetVFX(string id) => GetOrWarn(_vfxPresets, id, "VFXPresetSO");
        public AudioPresetSO GetAudio(string id) => GetOrWarn(_audioPresets, id, "AudioPresetSO");
        public BossDetailSO GetBossDetail(string bossId) => GetOrWarn(_bossDetails, bossId, "BossDetailSO");

        private T GetOrWarn<T>(Dictionary<string, T> dict, string id, string typeName) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DataRegistry] Get<{typeName}> called with null/empty id");
                return null;
            }
            if (dict.TryGetValue(id, out var so)) return so;
            Debug.LogWarning($"[DataRegistry] {typeName} id '{id}' not found");
            return null;
        }

        // ============================================================
        // Hot Reload（500ms debounce — data-config §C.1 规则 5 锁定）
        // ============================================================

        private float _reloadDebounce;
        public void RequestReload() => _reloadDebounce = 0.5f;   // 500ms

        void Update()
        {
            if (_reloadDebounce > 0f)
            {
                _reloadDebounce -= Time.unscaledDeltaTime;
                if (_reloadDebounce <= 0f)
                {
                    Debug.Log("[DataRegistry] Hot reload triggered (500ms debounce)");
                    _recipes.Clear(); _items.Clear(); _tools.Clear();
                    _einherjars.Clear(); _biomes.Clear(); _oaths.Clear();
                    _bosses.Clear(); _worldEvents.Clear(); _dialogues.Clear();
                    _sendoffs.Clear(); _uiStyles.Clear(); _settlements.Clear();
                    _expeditions.Clear(); _quests.Clear(); _vfxPresets.Clear();
                    _audioPresets.Clear(); _bossDetails.Clear();
                    LoadAll();
                }
            }
        }

        public bool IsLoaded => _isLoaded;
    }
}
