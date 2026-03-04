using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.UI
{
    public class MarketUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private Transform rowContainer;
        [SerializeField] private List<ItemSO> sellableItems; // Items that can be sold

        private void Start()
        {
            InitializeMarket();
        }

        public void InitializeMarket()
        {
            // Clear existing rows
            foreach (Transform child in rowContainer)
            {
                Destroy(child.gameObject);
            }

            // Create rows for each sellable item
            foreach (var item in sellableItems)
            {
                GameObject newRow = Instantiate(rowPrefab, rowContainer);
                MarketItemRow rowScript = newRow.GetComponent<MarketItemRow>();
                rowScript.Setup(item);
            }
        }
    }
}
