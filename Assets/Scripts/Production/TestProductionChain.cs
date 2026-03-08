using UnityEngine;
using System.Collections.Generic;
using FactoryRush.Scripts.Production;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Testing
{
    /// <summary>
    /// Test script for the production chain — only for Development/Testing.
    /// Place on an Empty GameObject in the scene when testing.
    ///
    /// KEYBINDS:
    ///   P  — Print the status of ALL machines in the Console
    ///   W  — Add 6 Wheat to inventory (enough for Bakery + Mill)
    ///   E  — Add 2 Egg to inventory (test Kitchen chain)
    ///   F  — Add 1 Flour + 1 Egg (test CakeBakery 2-step)
    ///   X  — Fill Wheat to max (20 units) to test InventoryFull blocking
    ///   G  — Simulate Game Over (stop all production)
    /// </summary>
    public class TestProductionChain : MonoBehaviour
    {
        [Header("Item References (assign SO assets in Inspector)")]
        public ItemSO wheat;
        public ItemSO egg;
        public ItemSO flour;
        public ItemSO bread;
        public ItemSO friedEgg;
        public ItemSO cake;

        private void Update()
        {
            // P — Print machine states
            if (Input.GetKeyDown(KeyCode.P))
                PrintAllMachineStates();

            // W — Add 6 Wheat
            if (Input.GetKeyDown(KeyCode.W))
                AddItem(wheat, 6, "Wheat");

            // E — Add 2 Egg
            if (Input.GetKeyDown(KeyCode.E))
                AddItem(egg, 2, "Egg");

            // F — Add 1 Flour + 1 Egg (test CakeBakery 2-step chain)
            if (Input.GetKeyDown(KeyCode.F))
            {
                AddItem(flour, 1, "Flour");
                AddItem(egg, 1, "Egg");
                Debug.Log("[Test] Added Flour + Egg → CakeBakery should start if placed.");
            }

            // X — Fill Wheat to 20 (test InventoryFull)
            if (Input.GetKeyDown(KeyCode.X))
            {
                AddItem(wheat, 20, "Wheat (FILL to test InventoryFull)");
                Debug.Log("[Test] Wheat filled to max. WheatField should show InventoryFull warning.");
            }

            // G — Trigger Game Over
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.EndGame();
                    Debug.Log("[Test] Game Over triggered. All machines should stop.");
                }
                else
                {
                    Debug.LogWarning("[Test] GameStateManager not found!");
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────

        private void PrintAllMachineStates()
        {
            if (ProductionManager.Instance == null)
            {
                Debug.LogWarning("[Test] ProductionManager not found!");
                return;
            }

            var machines = ProductionManager.Instance.GetActiveMachines();
            Debug.Log($"[Test] ===== Machine States ({machines.Count} total) =====");

            foreach (var m in machines)
            {
                string progressStr = m.GetState() == MachineState.Producing
                    ? $" | Progress: {m.GetProgress() * 100f:F0}%"
                    : "";

                Debug.Log($"[Test]  {m.GetMachineName(),-20} → {m.GetState()}{progressStr}");
            }

            // Print inventory
            if (InventoryManager.Instance != null)
            {
                Debug.Log("[Test] --- Inventory ---");
                var items = InventoryManager.Instance.GetAllItems();
                if (items.Count == 0)
                    Debug.Log("[Test]  (empty)");
                else
                    foreach (var kvp in items)
                        Debug.Log($"[Test]  {kvp.Key.itemName}: {kvp.Value}");
            }

            // Print gold
            if (FactoryRush.Scripts.Core.ScoreManager.Instance != null)
                Debug.Log($"[Test] --- Gold: {FactoryRush.Scripts.Core.ScoreManager.Instance.GetGold()} ---");

            Debug.Log("[Test] =================================");
        }

        private void AddItem(ItemSO item, int amount, string label)
        {
            if (item == null)
            {
                Debug.LogWarning($"[Test] {label} ItemSO not assigned in Inspector!");
                return;
            }

            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("[Test] InventoryManager not found!");
                return;
            }

            bool success = InventoryManager.Instance.AddItem(item, amount);
            int current = InventoryManager.Instance.GetCount(item);
            Debug.Log($"[Test] Add {amount}x {label}: {(success ? "OK" : "FAILED (full?)")} — Current: {current}");
        }
    }
}
