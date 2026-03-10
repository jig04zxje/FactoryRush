using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.ScriptableObjects.Definitions;
using FactoryRush.Scripts.Core;
// using FactoryRush.Scripts.Map; // Assuming BuildingUnlockSystem is here

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Manages the Building Shop panel.
    /// Lists available machines and handles purchase requests.
    /// </summary>
    public class BuildingShopUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private BuildingShopItemRow itemRowPrefab;

        [Header("Data")]
        [Tooltip("If empty, it will auto-load all machines from Resources/ScriptableObjects/Machines")]
        [SerializeField] private List<MachineSO> availableMachines;

        private List<BuildingShopItemRow> _instantiatedRows = new List<BuildingShopItemRow>();

        private void Start()
        {
            // Auto-load if list is empty
            if (availableMachines == null || availableMachines.Count == 0)
            {
                availableMachines = new List<MachineSO>();
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:MachineSO", new[] { "Assets/ScriptableObjects/Machines" });

                foreach (string guid in guids)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    MachineSO machine = UnityEditor.AssetDatabase.LoadAssetAtPath<MachineSO>(path);
                    if (machine.purchasePrice > 0)
                    {
                        availableMachines.Add(machine);
                    }
                }

                if (availableMachines.Count == 0)
                {
                    Debug.LogWarning("[BuildingShopUI] No MachineSO found in Assets/ScriptableObjects/Machines!");
                }
                else
                {
                    Debug.Log($"[BuildingShopUI] Auto-loaded {availableMachines.Count} machines from Assets.");
                }
            }

            PopulateShop();

            // Subscribe to gold changes to update row colors dynamically
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnGoldChanged.AddListener(UpdateAllRows);
            }

            // Close the shop initially
            if (shopPanel != null)
                shopPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnGoldChanged.RemoveListener(UpdateAllRows);
            }
        }

        /// <summary>
        /// Instantiates a UI row for each available machine.
        /// </summary>
        private void PopulateShop()
        {
            if (itemRowPrefab == null || contentContainer == null) return;

            // Clear existing
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }
            _instantiatedRows.Clear();

            foreach (MachineSO machine in availableMachines)
            {
                BuildingShopItemRow newRow = Instantiate(itemRowPrefab, contentContainer);
                newRow.Setup(machine, this);
                _instantiatedRows.Add(newRow);
            }
        }

        /// <summary>
        /// Called when ScoreManager announces a change in gold.
        /// Updates affordability colors on all rows.
        /// </summary>
        private void UpdateAllRows(int currentGold)
        {
            foreach (var row in _instantiatedRows)
            {
                row.UpdateAffordabilityUI(currentGold);
            }
        }

        /// <summary>
        /// Toggles the visibility of the shop panel.
        /// </summary>
        public void ToggleShop()
        {
            if (shopPanel == null) return;
            shopPanel.SetActive(!shopPanel.activeSelf);

            if (shopPanel.activeSelf)
            {
                // Force an update when opened to ensure colors are correct
                UpdateAllRows(ScoreManager.Instance.GetGold());
            }
        }

        public void CloseShop()
        {
            if (shopPanel != null) shopPanel.SetActive(false);
        }

        /// <summary>
        /// Called by a BuildingShopItemRow when its Buy button is clicked.
        /// </summary>
        public void RequestPurchase(MachineSO machineConfig)
        {
            int currentGold = ScoreManager.Instance.GetGold();

            if (currentGold >= machineConfig.purchasePrice)
            {
                Debug.Log($"[BuildingShopUI] Purchasing {machineConfig.machineName} for {machineConfig.purchasePrice}G.");

                // Call placement system here (To be implemented by Placement system)
                // PlacementSystem.Instance.StartPlacement(machineConfig);

                Debug.Log($"[BuildingShopUI] -> Triggering Placement Mode for {machineConfig.machineName}");
                CloseShop();
            }
            else
            {
                Debug.Log($"[BuildingShopUI] Not enough gold to buy {machineConfig.machineName}. Cost: {machineConfig.purchasePrice}, Current: {currentGold}");
                // UI FIX: We can trigger an error sound or shake animation here.
                StartCoroutine(ShakeShopPanel());
            }
        }

        /// <summary>
        /// Simple coroutine to shake the panel when a purchase is rejected.
        /// </summary>
        private IEnumerator ShakeShopPanel()
        {
            if (shopPanel == null) yield break;

            Vector3 originalPos = shopPanel.transform.localPosition;
            float duration = 0.2f;
            float magnitude = 10f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
                shopPanel.transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            shopPanel.transform.localPosition = originalPos;
        }
    }
}
