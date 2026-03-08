using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int maxStackSize = 20;

        private Dictionary<ItemSO, int> _items = new Dictionary<ItemSO, int>();

        // Event fired when inventory changes: ItemSO, currentAmount
        public UnityEvent<ItemSO, int> OnInventoryChanged = new UnityEvent<ItemSO, int>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Added AddItem(ItemSO) overload to match MachineController.AddItemDelegate signature
        /// <summary>
        /// Overload used by MachineController.AddItemDelegate — adds 1 unit of an item.
        /// </summary>
        public void AddItem(ItemSO item)
        {
            AddItem(item, 1);
        }

        public bool AddItem(ItemSO item, int amount)
        {
            if (item == null) return false;

            int currentAmount = GetCount(item);

            // Check capacity limit
            if (currentAmount >= maxStackSize)
            {
                Debug.LogWarning($"Inventory full for {item.itemName}!");
                return false;
            }

            int newAmount = Mathf.Min(currentAmount + amount, maxStackSize);
            _items[item] = newAmount;

            OnInventoryChanged?.Invoke(item, newAmount);
            return true;
        }

        public bool RemoveItem(ItemSO item, int amount)
        {
            if (item == null || !HasEnough(item, amount)) return false;

            _items[item] -= amount;

            if (_items[item] <= 0)
            {
                _items.Remove(item);
                OnInventoryChanged?.Invoke(item, 0);
            }
            else
            {
                OnInventoryChanged?.Invoke(item, _items[item]);
            }

            return true;
        }

        // Added RemoveItems(List<ItemSO>) to match MachineController.TakeItemsDelegate signature
        /// <summary>
        /// Atomically checks and removes a list of items. Used by MachineController.TakeItemsDelegate.
        /// Returns true only if ALL required items were present and successfully removed.
        /// </summary>
        public bool RemoveItems(List<ItemSO> items)
        {
            if (items == null || items.Count == 0) return true;

            // First pass: tally required amounts per item type
            var required = new Dictionary<ItemSO, int>();
            foreach (var item in items)
            {
                if (item == null) continue;
                if (!required.ContainsKey(item)) required[item] = 0;
                required[item]++;
            }

            // Check that we have enough of each before consuming anything
            foreach (var kvp in required)
            {
                if (!HasEnough(kvp.Key, kvp.Value))
                {
                    return false;
                }
            }

            // Second pass: remove all items
            foreach (var kvp in required)
            {
                RemoveItem(kvp.Key, kvp.Value);
            }

            return true;
        }

        public int GetCount(ItemSO item)
        {
            if (item == null) return 0;
            return _items.TryGetValue(item, out int count) ? count : 0;
        }

        public bool HasEnough(ItemSO item, int amount)
        {
            return GetCount(item) >= amount;
        }

        public bool IsFull(ItemSO item)
        {
            return GetCount(item) >= maxStackSize;
        }

        public Dictionary<ItemSO, int> GetAllItems()
        {
            return new Dictionary<ItemSO, int>(_items);
        }
    }
}
