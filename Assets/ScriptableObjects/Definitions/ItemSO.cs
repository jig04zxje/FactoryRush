using UnityEngine;

namespace FactoryRush.Scripts.ScriptableObjects.Definitions
{
    public enum ItemType
    {
        RawMaterial,
        Product
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Factory Rush/Item")]
    public class ItemSO : ScriptableObject
    {
        [Header("Item Info")]
        public string itemName;
        public Sprite icon;
        public int price;
        public int gem_gain; // Requirement cô Chi
        public ItemType type;

        [TextArea(3, 10)]
        public string description;
    }
}
