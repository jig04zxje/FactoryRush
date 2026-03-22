using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactoryRush.Scripts.ScriptableObjects.Definitions;
using FactoryRush.Scripts.Inventory;

namespace FactoryRush.Scripts.UI
{
    public class MarketItemRow : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button sellOneButton;
        [SerializeField] private Button sellAllButton;

        private ItemSO _boundItem;

        public void Setup(ItemSO item)
        {
            _boundItem = item;
            itemIcon.sprite = item.icon;
            itemNameText.text = item.itemName;
            priceText.text = $"{item.price}v";

            sellOneButton.onClick.AddListener(OnSellOneClicked);
            sellAllButton.onClick.AddListener(OnSellAllClicked);
        }

        private void OnSellOneClicked()
        {
            MarketManager.Instance.SellItem(_boundItem, 1);
            AudioManager.Instance?.PlaySFX("sell_ding");

        }

        private void OnSellAllClicked()
        {
            MarketManager.Instance.SellAllOfItem(_boundItem);
            AudioManager.Instance?.PlaySFX("sell_ding");
        }
    }
}
