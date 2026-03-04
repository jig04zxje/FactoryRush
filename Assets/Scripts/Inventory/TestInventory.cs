using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Testing
{
    public class TestInventory : MonoBehaviour
    {
        [SerializeField] private ItemSO testItem;
        [SerializeField] private int addAmount = 5;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                bool success = InventoryManager.Instance.AddItem(testItem, addAmount);
                Debug.Log($"Add {addAmount} {testItem.itemName}: {success}. Current: {InventoryManager.Instance.GetCount(testItem)}");
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                MarketManager.Instance.SellItem(testItem, 1);

            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                MarketManager.Instance.SellAllOfItem(testItem);
            }
        }
    }
}
