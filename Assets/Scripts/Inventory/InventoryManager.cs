using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private Dictionary<ItemSO, int> inventory = new Dictionary<ItemSO, int>();

    public UnityEvent OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        OnInventoryChanged = new UnityEvent();
    }

    // Add item
    public void Add(ItemSO item, int amount = 1)
    {
        if (inventory.ContainsKey(item))
            inventory[item] += amount;
        else
            inventory[item] = amount;

        OnInventoryChanged.Invoke();
    }

    // Remove item
    public void Remove(ItemSO item, int amount = 1)
    {
        if (!inventory.ContainsKey(item)) return;

        inventory[item] -= amount;

        if (inventory[item] <= 0)
            inventory.Remove(item);

        OnInventoryChanged.Invoke();
    }

    // Check item
    public bool Has(ItemSO item, int amount = 1)
    {
        return inventory.ContainsKey(item) && inventory[item] >= amount;
    }

    public Dictionary<ItemSO, int> GetInventory()
    {
        return inventory;
    }
}