using UnityEngine;

namespace FactoryRush.Scripts.Production
{
    /// <summary>
    /// Handles player interaction with machines on the map (Click to Harvest).
    /// </summary>
    public class HarvestHandler : MonoBehaviour
    {
        [Header("Settings")]
        public LayerMask machineLayer;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 1. Check precisely on Machine Layer
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos, machineLayer);

            if (hitCollider != null)
            {
                MachineController machine = hitCollider.GetComponent<MachineController>();
                if (machine != null)
                {
                    if (machine.GetState() == MachineState.ReadyToHarvest)
                    {
                        machine.Harvest();
                    }
                    else
                    {
                        Debug.Log($"[Harvest] {machine.machineData.machineName} is not ready yet ({machine.GetState()})");
                    }
                }
            }
            else
            {
                // 2. DIAGNOSTIC: Check what was actually hit regardless of layer
                Collider2D anyHit = Physics2D.OverlapPoint(mousePos);

                if (anyHit != null)
                {
                    string layerName = LayerMask.LayerToName(anyHit.gameObject.layer);
                    Debug.LogWarning($"[Harvest] Clicked '{anyHit.gameObject.name}' on Layer '{layerName}'. " +
                                     $"Expected Layer Mask: {machineLayer.value}.");

                    if (anyHit.gameObject.name.Contains("Slot"))
                    {
                        Debug.LogWarning("[Harvest] HINT: You clicked a GridSlot. Ensure the machine has a Collider2D and correct Layer.");
                    }
                }
                else
                {
                    Debug.Log($"[Harvest] Clicked empty space at {mousePos}.");
                }
            }
        }
    }
}
