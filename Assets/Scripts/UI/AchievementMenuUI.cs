using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.Core;

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Manages the achievement list in the Main Menu.
    /// Uses SlidePanel logic to open/close.
    /// </summary>
    public class AchievementMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform contentContainer;
        [SerializeField] private AchievementItemRow itemRowPrefab;
        [SerializeField] private SlidePanel slidePanel; 

        private void Start()
        {
            // Initial state is handled by SlidePanel (Awake/Start)
        }

        public void OpenMenu()
        {
            PopulateList();
            if (slidePanel != null) slidePanel.Open();
        }

        public void CloseMenu()
        {
            if (slidePanel != null) slidePanel.Close();
        }

        private void PopulateList()
        {
            if (contentContainer == null || itemRowPrefab == null) 
            {
                Debug.LogError("[AchievementMenuUI] Missing Container or Prefab assignment!");
                return;
            }

            // Clear existing
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }

            if (AchievementManager.Instance == null) 
            {
                Debug.LogError("[AchievementMenuUI] AchievementManager.Instance is NULL!");
                return;
            }

            var allAchievements = AchievementManager.Instance.GetAllAchievements();
            Debug.Log($"[AchievementMenuUI] Found {allAchievements.Count} achievements in Manager.");

            foreach (var ach in allAchievements)
            {
                bool isUnlocked = AchievementManager.Instance.IsUnlocked(ach.id);
                AchievementItemRow newRow = Instantiate(itemRowPrefab, contentContainer);
                newRow.Setup(ach, isUnlocked);
            }
        }
    }
}
