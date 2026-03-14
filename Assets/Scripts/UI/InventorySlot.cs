using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.UI
{
    public class InventorySlot : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private GameObject warningIcon; // Show if full or problem

        public ItemSO CurrentItem { get; private set; }

        public void UpdateSlot(ItemSO item, int count)
        {
            CurrentItem = item;

            if (item != null)
            {
                iconImage.sprite = item.icon;
                iconImage.enabled = true;
                countText.text = count.ToString();

                // Example: Show warning if count is 0 (should usually be removed) or reaching limit
                if (warningIcon != null)
                    warningIcon.SetActive(count >= 20);
            }
            else
            {
                ClearSlot();
            }
        }

        public void ClearSlot()
        {
            CurrentItem = null;
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "";
            if (warningIcon != null) warningIcon.SetActive(false);
        }
    }
}
