
using UnityEngine;
// using FactoryRush.Scripts.Systems; // Mở comment dòng này nếu GameStateManager nằm trong namespace Systems của D

namespace FactoryRush.Scripts.Map
{
    public class GridSlot : MonoBehaviour
    {
        public bool isOccupied = false;
        private SpriteRenderer spriteRenderer;

        [Header("Visual Colors")]
        public Color emptyColor = Color.green;
        public Color occupiedColor = Color.gray;
        public Color hoverColor = Color.yellow; 

        [Header("Placement Mode Colors (Task 11-14)")]
        public Color validPlacementColor = new Color(0, 1, 0, 0.8f); 
        public Color invalidPlacementColor = new Color(1, 0, 0, 0.8f); 

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateVisual();
        }

        private void OnMouseEnter()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.State != GameState.Playing) return;

            if (spriteRenderer == null) return;

            bool isPlacing = BuildingUnlockSystem.Instance != null && BuildingUnlockSystem.Instance.isPlacementMode;

            if (isPlacing)
            {

                spriteRenderer.color = isOccupied ? invalidPlacementColor : hoverColor;
            }
            else
            {
                if (!isOccupied)
                {
                    spriteRenderer.color = hoverColor;
                }
            }
            if (SlotTooltipManager.Instance != null)
            {
                if (isPlacing)
                {
                    SlotTooltipManager.Instance.ShowTooltip(isOccupied ? "Alreadytaken area" : "Placeable");
                }
                else
                {
                    SlotTooltipManager.Instance.ShowTooltip(isOccupied ? "Alreadytaken area" : "Empty place");
                }
            }
        }

        private void OnMouseExit()
        {
            UpdateVisual();
            if (SlotTooltipManager.Instance != null)
            {
                SlotTooltipManager.Instance.HideTooltip();
            }
        }

        private void OnMouseDown()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.State != GameState.Playing) return;

            if (BuildingUnlockSystem.Instance != null && BuildingUnlockSystem.Instance.isPlacementMode)
            {
                BuildingUnlockSystem.Instance.PlaceBuildingOnSlot(this);
            }
            else if (!isOccupied)
            {
                Debug.Log($"Click bình thường vào ô trống: {gameObject.name}");
            }
        }

        public void SetOccupied(bool status)
        {
            isOccupied = status;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isOccupied ? occupiedColor : emptyColor;
            }
        }
    }
}