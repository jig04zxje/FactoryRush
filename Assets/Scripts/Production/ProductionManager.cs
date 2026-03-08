using System.Collections.Generic;
using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Production
{
    /// <summary>
    /// Manages all active MachineControllers in the scene.
    /// - Registers / unregisters machines
    /// - Links inventory delegates automatically
    /// - Stops all production on Game Over
    /// </summary>
    public class ProductionManager : MonoBehaviour
    {
        public static ProductionManager Instance { get; private set; }

        [Header("Registry (Read-only)")]
        [SerializeField] private List<MachineController> activeMachines = new List<MachineController>();

        // Global delegates — set once and distributed to every registered machine
        public MachineController.TakeItemsDelegate GlobalInputHandler;
        public MachineController.AddItemDelegate GlobalOutputHandler;
        public MachineController.IsItemFullDelegate GlobalFullChecker;

        // ─────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

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

        private void Start()
        {
            // Wire InventoryManager delegates
            if (InventoryManager.Instance != null)
            {
                SetupInventoryLinks(
                    InventoryManager.Instance.RemoveItems,
                    InventoryManager.Instance.AddItem,
                    InventoryManager.Instance.IsFull
                );
            }
            else
            {
                Debug.LogError("[ProductionManager] InventoryManager.Instance is null! " +
                               "Make sure InventoryManager is in the scene before ProductionManager.");
            }

            // Wire GameOver — stop all production when game ends
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.OnGameOver.AddListener(StopAllProduction);
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.OnGameOver.RemoveListener(StopAllProduction);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Machine Registry

        /// <summary>Machine self-registers in Start(). Automatically wires delegates.</summary>
        public void RegisterMachine(MachineController machine)
        {
            if (machine == null || activeMachines.Contains(machine)) return;

            activeMachines.Add(machine);
            WireDelegatesToMachine(machine);
            machine.TryStartProduction();

            Debug.Log($"[ProductionManager] Registered: {machine.GetMachineName()} (total: {activeMachines.Count})");
        }

        /// <summary>Machine self-unregisters in OnDestroy().</summary>
        public void UnregisterMachine(MachineController machine)
        {
            if (activeMachines.Remove(machine))
                Debug.Log($"[ProductionManager] Unregistered: {machine.GetMachineName()}");
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Delegate Setup

        /// <summary>
        /// Updates global delegates and redistributes them to all current machines.
        /// Called automatically in Start(). Can also be called manually if InventoryManager initializes late.
        /// </summary>
        public void SetupInventoryLinks(
            MachineController.TakeItemsDelegate inputHandler,
            MachineController.AddItemDelegate outputHandler,
            MachineController.IsItemFullDelegate fullChecker)
        {
            GlobalInputHandler = inputHandler;
            GlobalOutputHandler = outputHandler;
            GlobalFullChecker = fullChecker;

            foreach (var machine in activeMachines)
                WireDelegatesToMachine(machine);

            Debug.Log("[ProductionManager] Inventory delegates wired to all machines.");
        }

        private void WireDelegatesToMachine(MachineController machine)
        {
            machine.OnRequestInputs = GlobalInputHandler;
            machine.OnRequestAddItem = GlobalOutputHandler;
            machine.OnCheckInventoryFull = GlobalFullChecker;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Game Over

        /// <summary>Stops all production when game ends.</summary>
        public void StopAllProduction()
        {
            Debug.Log("[ProductionManager] Game Over — stopping all machines.");
            foreach (var machine in activeMachines)
                machine.StopProduction();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Utility

        /// <summary>True if ALL machines are in the Producing state.</summary>
        public bool AllMachinesRunning()
        {
            if (activeMachines.Count == 0) return false;
            foreach (var m in activeMachines)
                if (m.GetState() != MachineState.Producing) return false;
            return true;
        }

        /// <summary>Returns a copy of the list of active machines.</summary>
        public List<MachineController> GetActiveMachines() => new List<MachineController>(activeMachines);

        #endregion
    }
}
