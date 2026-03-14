using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;
using FactoryRush.Scripts.Core;

namespace FactoryRush.Scripts.UI
{
    public class MarketUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private Transform rowContainer;
        [SerializeField] private List<ItemSO> sellableItems; // Items that can be sold

        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Auto-add if missing
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (GameStateManager.Instance != null)
            {
                UpdateInteractability(GameStateManager.Instance.State);
                GameStateManager.Instance.OnGameStarted.AddListener(() => UpdateInteractability(GameState.Playing));
                GameStateManager.Instance.OnGameOver.AddListener(() => UpdateInteractability(GameState.GameOver));
            }

            InitializeMarket();
        }

        private void UpdateInteractability(GameState state)
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = (state == GameState.Playing);
            }
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
