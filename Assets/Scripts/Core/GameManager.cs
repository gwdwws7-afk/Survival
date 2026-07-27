using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ravensong.Data;

namespace Ravensong.Core
{
    /// <summary>
    /// 游戏主控（handover §7 P1 锁定）。
    /// 单例、scene 切换、save/load hook、全局游戏状态。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Save/Load")]
        [Tooltip("save 文件夹名（Application.persistentDataPath 下）")]
        public string saveFolder = "Saves";
        [Tooltip("save 文件扩展名")]
        public string saveExt = ".json";

        [Header("State")]
        public GameState state = GameState.Booting;

        // 全局游戏状态
        public float playTimeSeconds = 0f;
        public string currentSceneName;
        public int currentDayNumber = 1;
        public int currentSaveSlot = -1;

        public event Action OnGamePaused;
        public event Action OnGameResumed;
        public event Action OnGameSaved;
        public event Action OnGameLoaded;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (state == GameState.Playing)
            {
                playTimeSeconds += Time.deltaTime;
            }
        }

        // ============================================================
        // Pause / Resume
        // ============================================================

        public void Pause()
        {
            if (state != GameState.Playing) return;
            state = GameState.Paused;
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
        }

        public void Resume()
        {
            if (state != GameState.Paused) return;
            state = GameState.Playing;
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
        }

        public void TogglePause()
        {
            if (state == GameState.Playing) Pause();
            else if (state == GameState.Paused) Resume();
        }

        // ============================================================
        // Scene
        // ============================================================

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            currentSceneName = sceneName;
            SceneManager.LoadScene(sceneName, mode);
        }

        // ============================================================
        // Save/Load（data-config §C.1 锁定：仅玩家数据，非静态数据）
        // ============================================================

        public string GetSavePath(int slot)
        {
            return Path.Combine(Application.persistentDataPath, saveFolder, $"save_{slot}{saveExt}");
        }

        public bool SaveToSlot(int slot)
        {
            if (DataRegistry.Instance == null || DataRegistry.Instance.gameConfig == null)
            {
                Debug.LogError("[GameManager] DataRegistry or GameConfig not ready");
                return false;
            }
            int max = DataRegistry.Instance.gameConfig.maxSaveSlots;
            if (slot < 0 || slot >= max)
            {
                Debug.LogError($"[GameManager] Save slot {slot} out of range [0, {max})");
                return false;
            }

            try
            {
                var save = new SaveData
                {
                    playTimeSeconds = playTimeSeconds,
                    currentDayNumber = currentDayNumber,
                    currentSceneName = currentSceneName,
                    saveTimestamp = DateTime.UtcNow.ToString("o"),
                    schemaVersion = "1.0"
                };
                // TODO: 收集所有运行时状态（settlement / einherjars / inventory / oaths progress）
                // SettlementState, EinherjarState[], ItemStack[], OathProgress[]

                var path = GetSavePath(slot);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, JsonUtility.ToJson(save, true));
                currentSaveSlot = slot;
                OnGameSaved?.Invoke();
                Debug.Log($"[GameManager] Saved to slot {slot}: {path}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameManager] Save failed: {e.Message}");
                return false;
            }
        }

        public bool LoadFromSlot(int slot)
        {
            var path = GetSavePath(slot);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[GameManager] Save file not found: {path}");
                return false;
            }
            try
            {
                var json = File.ReadAllText(path);
                var save = JsonUtility.FromJson<SaveData>(json);
                if (save.schemaVersion != "1.0")
                {
                    Debug.LogError($"[GameManager] Save schema version mismatch: {save.schemaVersion} (expected 1.0)");
                    return false;
                }
                playTimeSeconds = save.playTimeSeconds;
                currentDayNumber = save.currentDayNumber;
                currentSaveSlot = slot;
                if (!string.IsNullOrEmpty(save.currentSceneName))
                    LoadScene(save.currentSceneName);
                OnGameLoaded?.Invoke();
                Debug.Log($"[GameManager] Loaded slot {slot}: day {currentDayNumber}, {playTimeSeconds:F0}s play time");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameManager] Load failed: {e.Message}");
                return false;
            }
        }

        public void DeleteSlot(int slot)
        {
            var path = GetSavePath(slot);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[GameManager] Deleted save slot {slot}");
            }
        }

        public bool SlotExists(int slot) => File.Exists(GetSavePath(slot));

        // ============================================================
        // Lifecycle
        // ============================================================

        public void StartNewGame()
        {
            playTimeSeconds = 0f;
            currentDayNumber = 1;
            currentSaveSlot = -1;
            state = GameState.Playing;
            Time.timeScale = 1f;
            LoadScene("GameScene");
        }
    }

    public enum GameState
    {
        Booting,
        Playing,
        Paused,
        InDialogue,    // 不可暂停时（重要对话/过场）
        InCutscene
    }

    /// <summary>玩家 save 数据（仅运行时，不含静态 SO）</summary>
    [Serializable]
    public class SaveData
    {
        public string schemaVersion;
        public string saveTimestamp;
        public float playTimeSeconds;
        public int currentDayNumber;
        public string currentSceneName;
        // TODO 阶段 1.4+ 扩展：
        // public SettlementState settlement;
        // public List<EinherjarState> einherjars;
        // public List<ItemStack> inventory;
        // public List<OathProgress> oaths;
        // public QuestProgress[] quests;
    }
}
