using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Production
{
    public class ProductionManager : MonoBehaviour
    {
        public static ProductionManager Instance { get; private set; }

        [Header("Registry")]
        [SerializeField] private List<MachineController> activeMachines = new List<MachineController>();

        // These handles will be connected to the real Inventory system later
        public MachineController.TakeItemsDelegate GlobalInputHandler;
        public MachineController.AddItemDelegate GlobalOutputHandler;
        public MachineController.CheckCapacityDelegate GlobalCapacityHandler;

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

        public void RegisterMachine(MachineController machine)
        {
            if (!activeMachines.Contains(machine))
            {
                activeMachines.Add(machine);

                // Automatically link delegates
                machine.OnRequestInputs = GlobalInputHandler;
                machine.OnRequestAddItem = GlobalOutputHandler;
                machine.OnCheckCapacity = GlobalCapacityHandler;

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
        public void SetupInventoryLinks(MachineController.TakeItemsDelegate inputHandler, MachineController.AddItemDelegate outputHandler, MachineController.CheckCapacityDelegate capacityHandler)
        {
            GlobalInputHandler = inputHandler;
            GlobalOutputHandler = outputHandler;
            GlobalCapacityHandler = capacityHandler;

            // Update all existing machines
            foreach (var machine in activeMachines)
            {
                machine.OnRequestInputs = GlobalInputHandler;
                machine.OnRequestAddItem = GlobalOutputHandler;
                machine.OnCheckCapacity = GlobalCapacityHandler;
            }
        }

        public List<MachineController> GetActiveMachines() => activeMachines;
    }
}
