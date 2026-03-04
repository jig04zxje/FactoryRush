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
            // Simple Raycast for 2D interaction
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, machineLayer);

            if (hit.collider != null)
            {
                MachineController machine = hit.collider.GetComponent<MachineController>();
                if (machine != null)
                {
                    if (machine.GetState() == MachineState.ReadyToHarvest)
                    {
                        machine.Harvest();
                    }
                    else
                    {
                        Debug.Log($"Machine {machine.machineData.machineName} is not ready yet ({machine.GetState()})");
                    }
                }
            }
        }
    }
}
