using System.Collections.Generic;
using UnityEngine;

namespace FactoryRush.Scripts.ScriptableObjects.Definitions
{
    [CreateAssetMenu(fileName = "New Machine", menuName = "Factory Rush/Machine")]
    public class MachineSO : ScriptableObject
    {
        [Header("Machine Info")]
        public string machineName;
        public int purchasePrice;
        public GameObject visualPrefab;

        [Header("Production Config")]
        public List<ItemSO> requiredInputs;
        public ItemSO outputItem;
        public float productionTime;

        [TextArea(3, 10)]
        public string description;
    }
}
