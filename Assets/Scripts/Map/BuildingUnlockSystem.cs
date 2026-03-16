using FactoryRush.Scripts.Core;
using UnityEngine;

namespace FactoryRush.Scripts.Map
{
    public class BuildingUnlockSystem : MonoBehaviour
    {
        public static BuildingUnlockSystem Instance { get; private set; }

        [Header("Placement State")]
        public bool isPlacementMode = false;
        private GameObject pendingBuildingPrefab;
        private int pendingBuildingCost;

        private GameObject ghostBuilding;
        [Header("Demolish State")]
        public bool isDemolishMode = false;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (isPlacementMode)
            {
                // 1.Make ghost building follow mouse
                if (ghostBuilding != null)
                {
                    //convert to real world position
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = 0; // Đảm bảo Z = 0 cho game 2D

                    // ghost building position
                    ghostBuilding.transform.position = mousePos;
                }

                // 2. Cancel with right mouse
                if (Input.GetMouseButtonDown(1))
                {
                    ExitPlacementMode();
                }
            }
            if (isDemolishMode && Input.GetMouseButtonDown(1))
            {
                ExitDemolishMode();
            }
        }

        public void StartPlacementMode(GameObject buildingPrefab, int cost)
        {
            pendingBuildingPrefab = buildingPrefab;
            pendingBuildingCost = cost;
            isPlacementMode = true;

            //Ghost preview
            // if old ghost still exist, delete it 
            if (ghostBuilding != null) Destroy(ghostBuilding);

            // copy of building
            ghostBuilding = Instantiate(buildingPrefab);

            //Turn off collider
            Collider2D[] colliders = ghostBuilding.GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders) col.enabled = false;

            // fade it 
            SpriteRenderer[] renderers = ghostBuilding.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;
                sr.sortingOrder = 100; 
            }

            Debug.Log($"[BuildingUnlockSystem] Đã vào chế độ đặt. Đang cầm {buildingPrefab.name} trên tay!");
        }

        public void PlaceBuildingOnSlot(GridSlot slot)
        {
            if (!isPlacementMode || pendingBuildingPrefab == null) return;

            if (slot.isOccupied)
            {
                Debug.Log("[BuildingUnlockSystem] Slot này đã có công trình, không cắm được!");
                return;
            }

            GameObject newBuilding = Instantiate(pendingBuildingPrefab, slot.transform.position, Quaternion.identity);
            newBuilding.transform.SetParent(slot.transform);
            slot.SetOccupied(true);

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddGold(-pendingBuildingCost);
            }
            Debug.Log($"[BuildingUnlockSystem] Đã đặt {pendingBuildingPrefab.name} và trừ {pendingBuildingCost} vàng.");

            int currentGold = ScoreManager.Instance != null ? ScoreManager.Instance.GetGold() : 0;

            if (currentGold < pendingBuildingCost)
            {
                Debug.Log("[BuildingUnlockSystem] Đã hết vàng! Tự động thoát chế độ xây dựng.");
                ExitPlacementMode();
            }
            else
            {
                Debug.Log("[BuildingUnlockSystem] Vẫn đủ vàng, tiếp tục cho phép cắm thêm!");
            }
        }

        private void ExitPlacementMode()
        {
            isPlacementMode = false;
            pendingBuildingPrefab = null;

            if (ghostBuilding != null) Destroy(ghostBuilding);

            Debug.Log("[BuildingUnlockSystem] Đã thoát chế độ đặt nhà.");
        }
        public void ToggleDemolishMode()
        {
            if (isPlacementMode) ExitPlacementMode();

            isDemolishMode = !isDemolishMode;
            Debug.Log($"[BuildingUnlockSystem] Chế độ phá dỡ: {isDemolishMode}");
        }

        private void ExitDemolishMode()
        {
            isDemolishMode = false;
            Debug.Log("[BuildingUnlockSystem] Đã thoát chế độ phá dỡ.");
        }

        public void DemolishBuildingOnSlot(GridSlot slot)
        {
            if (!isDemolishMode || !slot.isOccupied) return;

            //Find building in slot
            if (slot.transform.childCount > 0)
            {
                GameObject buildingToDestroy = slot.transform.GetChild(0).gameObject;
                Destroy(buildingToDestroy); 

                slot.SetOccupied(false); // return place

                // (optional) refund gold

                Debug.Log("[BuildingUnlockSystem] Đã dọn dẹp mặt bằng thành công!");
            }
        }
    }
}