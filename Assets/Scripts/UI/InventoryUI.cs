using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotContainer;

        private Dictionary<ItemSO, InventorySlot> _spawnedSlots = new Dictionary<ItemSO, InventorySlot>();

        private void Start()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged.AddListener(OnInventoryChanged);
                RefreshUI();
            }
        }

        private void OnDestroy()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged.RemoveListener(OnInventoryChanged);
            }
        }

        private void OnInventoryChanged(ItemSO item, int count)
        {
            if (count <= 0)
            {
                if (_spawnedSlots.TryGetValue(item, out InventorySlot slot))
                {
                    Destroy(slot.gameObject);
                    _spawnedSlots.Remove(item);
                }
            }
            else
            {
                if (_spawnedSlots.TryGetValue(item, out InventorySlot slot))
                {
                    slot.UpdateSlot(item, count);
                }
                else
                {
                    CreateNewSlot(item, count);
                }
            }
        }

        private void CreateNewSlot(ItemSO item, int count)
        {
            GameObject newSlotObj = Instantiate(slotPrefab, slotContainer);
            InventorySlot slot = newSlotObj.GetComponent<InventorySlot>();
            slot.UpdateSlot(item, count);
            _spawnedSlots.Add(item, slot);
        }

        public void RefreshUI()
        {
            // Clear existing
            foreach (var slot in _spawnedSlots.Values)
            {
                Destroy(slot.gameObject);
            }
            _spawnedSlots.Clear();

            // Re-spawn all from manager
            if (InventoryManager.Instance != null)
            {
                foreach (var kvp in InventoryManager.Instance.GetAllItems())
                {
                    CreateNewSlot(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
