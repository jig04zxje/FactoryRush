using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager.OnInventoryChanged.AddListener(RefreshUI);
    }

    void RefreshUI()
    {
        Debug.Log("Inventory Updated");

        foreach (var item in inventoryManager.GetInventory())
        {
            Debug.Log(item.Key.name + " : " + item.Value);
        }
    }
}