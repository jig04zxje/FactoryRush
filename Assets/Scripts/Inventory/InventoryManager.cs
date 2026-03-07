using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<ItemSO, int> inventory = new Dictionary<ItemSO, int>();

    public UnityEvent OnInventoryChanged = new UnityEvent();

    public void Add(ItemSO item, int amount = 1)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += amount;
        }
        else
        {
            inventory.Add(item, amount);
        }

        OnInventoryChanged.Invoke();
    }

    public void Remove(ItemSO item, int amount = 1)
    {
        if (!inventory.ContainsKey(item))
            return;

        inventory[item] -= amount;

        if (inventory[item] <= 0)
        {
            inventory.Remove(item);
        }

        OnInventoryChanged.Invoke();
    }

    public bool Has(ItemSO item, int amount = 1)
    {
        if (!inventory.ContainsKey(item))
            return false;

        return inventory[item] >= amount;
    }

    public Dictionary<ItemSO, int> GetInventory()
    {
        return inventory;
    }
}