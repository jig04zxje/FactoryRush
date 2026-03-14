using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;
using FactoryRush.Scripts.Core;

namespace FactoryRush.Scripts.Inventory
{
    public class MarketManager : MonoBehaviour
    {
        public static MarketManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Added DontDestroyOnLoad so MarketManager persists on scene reload.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SellItem(ItemSO item, int quantity)
        {
            if (item == null || quantity <= 0) return;

            if (InventoryManager.Instance == null)
            {
                Debug.LogError("InventoryManager Instance not found! Make sure it's in the scene.");
                return;
            }

            if (ScoreManager.Instance == null)
            {
                Debug.LogError("ScoreManager Instance not found! Make sure it's in the scene.");
                return;
            }

            if (InventoryManager.Instance.HasEnough(item, quantity))
            {
                if (InventoryManager.Instance.RemoveItem(item, quantity))
                {
                    int totalGoldEarned = item.price * quantity;
                    ScoreManager.Instance.AddGold(totalGoldEarned);
                    Debug.Log($"Sold {quantity} {item.itemName} for {totalGoldEarned} gold.");
                }
            }
            else
            {
                Debug.LogWarning("Not enough items to sell!");
            }
        }

        public void SellAllOfItem(ItemSO item)
        {
            int count = InventoryManager.Instance.GetCount(item);
            if (count > 0)
            {
                SellItem(item, count);
            }
        }
    }
}
