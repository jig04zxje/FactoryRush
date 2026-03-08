using UnityEngine;
using FactoryRush.Scripts.Production;
using FactoryRush.Scripts.Core;

namespace FactoryRush.Scripts.Core
{
    /// <summary>
    /// Centralized input controller to handle all mouse clicks in the game.
    /// This prevents overlap issues between building and harvesting.
    /// </summary>
    public class PlayerInputController : MonoBehaviour
    {
        public static PlayerInputController Instance { get; private set; }

        [Header("Settings")]
        [Tooltip("Ensure your Machine prefabs are on this Layer")]
        public LayerMask machineLayer;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            // 1. Only allow interaction when GameState is Playing
            if (GameStateManager.Instance != null && GameStateManager.Instance.State != GameState.Playing) return;

            // 2. Listen for Left Mouse Click
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // --- Priority 1: MACHINE HARVESTING ---
            Collider2D machineHit = Physics2D.OverlapPoint(mousePos, machineLayer);
            if (machineHit != null)
            {
                MachineController machine = machineHit.GetComponent<MachineController>();
                if (machine != null)
                {
                    if (machine.GetState() == MachineState.ReadyToHarvest)
                    {
                        machine.Harvest();
                    }
                    else
                    {
                        Debug.Log($"[Input] {machine.machineData.machineName} is not ready yet.");
                    }

                    // Machine handled, RETURN IMMEDIATELY (Do not check grid below)
                    return;
                }
            }

            // --- Priority 2: PLACING BUILDINGS ON GRID SLOTS ---
            // Use OverlapPointAll to scan all objects at the click point (in case of clicking closely outside the machine)
            Collider2D[] allHits = Physics2D.OverlapPointAll(mousePos);
            foreach (Collider2D hit in allHits)
            {
                GridSlot slot = hit.GetComponent<GridSlot>();
                if (slot != null)
                {
                    if (!slot.isOccupied)
                    {
                        // Only build if the slot is empty
                        if (BuildingPlacer.Instance != null)
                        {
                            BuildingPlacer.Instance.PlaceBuilding(slot);
                        }
                    }
                    return; // Handled the slot, exit
                }
            }
        }
    }
}
