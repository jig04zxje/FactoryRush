using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Production
{
    public class ProductionManager : MonoBehaviour
    {
        public static ProductionManager Instance { get; private set; }

        [Header("Registry")]
        [SerializeField] private List<MachineController> activeMachines = new List<MachineController>();

        // These handles will be connected to the real Inventory system later
        // For now, they can be set by a MockInventory or the actual InventoryManager
        public MachineController.TakeItemsDelegate GlobalInputHandler;
        public MachineController.AddItemDelegate GlobalOutputHandler;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Automatically link InventoryManager delegates.
        private void Start()
        {
            if (InventoryManager.Instance != null)
            {
                SetupInventoryLinks(
                    InventoryManager.Instance.RemoveItems,
                    InventoryManager.Instance.AddItem
                );
            }
            else
            {
                Debug.LogError("ProductionManager: InventoryManager.Instance is null in Start()! " +
                               "Make sure InventoryManager is in the scene and initialized before ProductionManager.");
            }
        }

        public void RegisterMachine(MachineController machine)
        {
            if (!activeMachines.Contains(machine))
            {
                activeMachines.Add(machine);

                // Automatically link delegates
                machine.OnRequestInputs = GlobalInputHandler;
                machine.OnRequestAddItem = GlobalOutputHandler;

                // Try to start production if it was waiting
                machine.TryStartProduction();
            }
        }

        public void UnregisterMachine(MachineController machine)
        {
            if (activeMachines.Contains(machine))
            {
                activeMachines.Remove(machine);
            }
        }

        /// <summary>
        /// Example logic to check if all machines are currently working.
        /// Useful for UI indicators or Game State tracking.
        /// </summary>
        public bool AllMachinesRunning()
        {
            if (activeMachines.Count == 0) return false;

            foreach (var machine in activeMachines)
            {
                if (machine.GetState() != MachineState.Producing)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Used by Inventory system to provide the functional logic to all machines.
        /// </summary>
        public void SetupInventoryLinks(MachineController.TakeItemsDelegate inputHandler, MachineController.AddItemDelegate outputHandler)
        {
            GlobalInputHandler = inputHandler;
            GlobalOutputHandler = outputHandler;

            // Update all existing machines
            foreach (var machine in activeMachines)
            {
                machine.OnRequestInputs = GlobalInputHandler;
                machine.OnRequestAddItem = GlobalOutputHandler;
            }
        }

        public List<MachineController> GetActiveMachines() => activeMachines;
    }
}
