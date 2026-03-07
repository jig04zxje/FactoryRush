using UnityEngine;

public class MarketManager : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public void SellItem(ItemSO item, int amount)
    {
        if (!inventoryManager.Has(item, amount))
            return;

        inventoryManager.Remove(item, amount);

        int totalPrice = item.sellPrice * amount;

        ScoreManager.Instance.AddGold(totalPrice);
    }
}