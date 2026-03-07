using UnityEngine;

[CreateAssetMenu(menuName = "FactoryRush/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public int sellPrice;   
}