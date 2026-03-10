using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactoryRush.Scripts.ScriptableObjects.Definitions;
using FactoryRush.Scripts.Core;
// using FactoryRush.Scripts.Map; // Assuming BuildingUnlockSystem is or will be here

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Represents a single machine row in the Building Shop UI.
    /// Handles updating its own visual state based on player gold.
    /// </summary>
    public class BuildingShopItemRow : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;

        [Header("Colors for Price")]
        [SerializeField] private Color affordableColor = Color.white;
        [SerializeField] private Color unaffordableColor = Color.red;

        private MachineSO _machineData;
        private BuildingShopUI _parentShop;

        public void Setup(MachineSO machine, BuildingShopUI parentShop)
        {
            _machineData = machine;
            _parentShop = parentShop;

            if (iconImage != null && machine.visualPrefab != null)
            {
                // Simple workaround: Since MachineSO doesn't have an icon field currently,
                // we try to extract the SpriteRenderer sprite from its prefab.
                var spriteRenderer = machine.visualPrefab.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    iconImage.sprite = spriteRenderer.sprite;
                }
            }

            if (nameText != null) nameText.text = machine.machineName;

            // Format price: Free vs Cost
            if (priceText != null)
            {
                priceText.text = machine.purchasePrice <= 0 ? "Free" : $"{machine.purchasePrice}G";
            }

            // Hook up button
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyClicked);
            }

            // Do initial UI update
            UpdateAffordabilityUI(ScoreManager.Instance.GetGold());
        }

        /// <summary>
        /// Called by the parent shop when gold changes to update colors and button states.
        /// </summary>
        public void UpdateAffordabilityUI(int currentGold)
        {
            bool canAfford = currentGold >= _machineData.purchasePrice;

            if (priceText != null)
            {
                priceText.color = canAfford ? affordableColor : unaffordableColor;
            }

            // We keep the button interactable to show error shakes, or we can disable it entirely.
            // Based on the plan, let's keep it interactable to allow for UI rejection feedback.
        }

        private void OnBuyClicked()
        {
            _parentShop.RequestPurchase(_machineData);
        }
    }
}
