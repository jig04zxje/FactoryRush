using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactoryRush.Scripts.Core
{
    /// <summary>
    /// Singleton manager handling achievement unlocks and persistence.
    /// Listens to Gem updates from ScoreManager.
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [Header("Achievements Config")]
        [SerializeField] private List<AchievementSO> achievements;

        /// <summary>Event fired when a new achievement is unlocked.</summary>
        public UnityEvent<AchievementSO> OnAchievementUnlocked = new UnityEvent<AchievementSO>();

        private HashSet<string> _unlockedIds = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAchievements();
        }

        private void Start()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnGemsChanged.AddListener(CheckAchievements);

                Invoke(nameof(ForceInitialCheck), 0.5f);
            }
            else
            {
                Debug.LogWarning("[AchievementManager] ScoreManager not found in Start!");
            }
        }

        private void ForceInitialCheck()
        {
            if (ScoreManager.Instance == null) return;
            CheckAchievements(ScoreManager.Instance.GetGems());
        }

        private void OnDestroy()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnGemsChanged.RemoveListener(CheckAchievements);
            }
        }

        /// <summary>
        /// Compares current gems against the thresholds of all locked achievements.
        /// </summary>
        public void CheckAchievements(int currentGems)
        {
            foreach (var ach in achievements)
            {
                if (!IsUnlocked(ach.id) && currentGems >= ach.gemThreshold)
                {
                    UnlockAchievement(ach);
                }
            }
        }

        public bool IsUnlocked(string id)
        {
            return _unlockedIds.Contains(id);
        }

        private void UnlockAchievement(AchievementSO ach)
        {
            if (_unlockedIds.Contains(ach.id)) return;

            _unlockedIds.Add(ach.id);
            SaveAchievement(ach.id);

            OnAchievementUnlocked?.Invoke(ach);
        }

        #region Persistence (PlayerPrefs)

        private void SaveAchievement(string id)
        {
            PlayerPrefs.SetInt("Achievement_" + id, 1);
            PlayerPrefs.Save();
        }

        private void LoadAchievements()
        {
            _unlockedIds.Clear();
            foreach (var ach in achievements)
            {
                if (PlayerPrefs.GetInt("Achievement_" + ach.id, 0) == 1)
                {
                    _unlockedIds.Add(ach.id);
                }
            }
        }

        [ContextMenu("Reset Achievements")]
        public void ResetAllAchievements()
        {
            foreach (var ach in achievements)
            {
                PlayerPrefs.DeleteKey("Achievement_" + ach.id);
            }
            PlayerPrefs.Save();
            _unlockedIds.Clear();
        }

        #endregion

        /// <summary>
        /// Returns all achievements for UI display.
        /// </summary>
        public List<AchievementSO> GetAllAchievements() => achievements;
    }
}
