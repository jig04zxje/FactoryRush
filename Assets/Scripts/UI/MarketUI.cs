using UnityEngine;

public class MarketUI : MonoBehaviour
{
    public MarketManager marketManager;
    public InventoryManager inventoryManager;

    public ItemSO item;

    public void SellOne()
    {
        marketManager.SellItem(item, 1);
    }

    public void SellAll()
    {
        var inventory = inventoryManager.GetInventory();

        if (!inventory.ContainsKey(item))
            return;

        int amount = inventory[item];

        marketManager.SellItem(item, amount);
    }
}