using UnityEngine;
using System.Collections.Generic;

namespace FactoryRush.Scripts.Map
{
    public class GridManager : MonoBehaviour
    {
        [Header("Manual Map Layout")]
        [Tooltip("Kéo thả các GridSlot trên scene vào đây, hoặc để trống nó sẽ tự tìm.")]
        public List<GridSlot> allSlots = new List<GridSlot>();

        void Start()
        {
          
            if (allSlots.Count == 0)
            {
                allSlots = new List<GridSlot>(GetComponentsInChildren<GridSlot>());
                Debug.Log($"[GridManager] Đã tự động tìm thấy {allSlots.Count} slots trên bản đồ.");
            }
        }
    }
}