using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FactoryRush.Scripts.ScriptableObjects.Definitions;

namespace FactoryRush.Scripts.Production
{
    public enum MachineState
    {
        Idle,
        WaitingForInput,  // Configured inputs but inventory lacks materials
        Producing,        // Production Coroutine is running
        ReadyToHarvest,   // Finished, waiting for player click
        InventoryFull     // Output item inventory is full, production blocked
    }

    public class MachineController : MonoBehaviour
    {
        [Header("Data")]
        public MachineSO machineData;

        [Header("State (Read-only in Inspector)")]
        [SerializeField] private MachineState currentState = MachineState.Idle;
        [SerializeField] private float currentProgress = 0f;

        // ── Events used by MachineVisualController & UI ─────────────────────
        [Header("Events")]
        public UnityEvent OnProductionStarted = new UnityEvent();
        public UnityEvent<float> OnProductionProgress = new UnityEvent<float>(); // 0..1
        public UnityEvent OnProductionComplete = new UnityEvent(); // ReadyToHarvest
        public UnityEvent OnHarvested = new UnityEvent();
        public UnityEvent OnWaitingForInput = new UnityEvent(); // shows "?" warning
        public UnityEvent OnInventoryFull = new UnityEvent(); // shows "!" warning
        public UnityEvent OnProductionStopped = new UnityEvent(); // Game Over

        // ── Delegates linked to InventoryManager ───────────────────────────────
        public delegate bool TakeItemsDelegate(List<ItemSO> inputs);
        public delegate void AddItemDelegate(ItemSO item);
        public delegate bool IsItemFullDelegate(ItemSO item);

        public TakeItemsDelegate OnRequestInputs;
        public AddItemDelegate OnRequestAddItem;
        public IsItemFullDelegate OnCheckInventoryFull;

        // ── Coroutine references ──────────────────────────────────────────────
        private Coroutine _productionCoroutine;
        private Coroutine _tickerCoroutine;
        private bool _isGameOver = false;

        // ─────────────────────────────────────────────────────────────────────
        #region Unity Lifecycle

        private void Start()
        {
            if (machineData == null)
            {
                Debug.LogError($"[Machine] MachineData missing on {gameObject.name}!");
                return;
            }

            // Register GameOver listener
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.OnGameOver.AddListener(StopProduction);

            // Register with ProductionManager
            if (ProductionManager.Instance != null)
                ProductionManager.Instance.RegisterMachine(this);
            else
                Debug.LogWarning($"[Machine] ProductionManager not found for {gameObject.name}!");

            // Start auto-ticker
            _tickerCoroutine = StartCoroutine(AutoProductionTicker());
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.OnGameOver.RemoveListener(StopProduction);

            if (ProductionManager.Instance != null)
                ProductionManager.Instance.UnregisterMachine(this);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Core Production Loop

        /// <summary>Ticker checks every second if production can start.</summary>
        private IEnumerator AutoProductionTicker()
        {
            while (!_isGameOver)
            {
                if (currentState == MachineState.Idle || currentState == MachineState.WaitingForInput)
                    TryStartProduction();

                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>Try to start a new production cycle.</summary>
        public void TryStartProduction()
        {
            if (_isGameOver) return;
            if (currentState != MachineState.Idle && currentState != MachineState.WaitingForInput) return;

            // --- Check if output inventory is full ---
            if (OnCheckInventoryFull != null && machineData.outputItem != null)
            {
                if (OnCheckInventoryFull(machineData.outputItem))
                {
                    currentState = MachineState.InventoryFull;
                    Debug.LogWarning($"[Machine] {machineData.machineName}: Inventory full for {machineData.outputItem.itemName}! Production blocked.");
                    OnInventoryFull?.Invoke();
                    return;
                }
            }

            // --- Check & consume inputs ---
            if (machineData.requiredInputs != null && machineData.requiredInputs.Count > 0)
            {
                if (OnRequestInputs != null && OnRequestInputs(machineData.requiredInputs))
                {
                    // Enough materials → start production
                    _productionCoroutine = StartCoroutine(ProductionCycle());
                }
                else
                {
                    // Not enough materials
                    if (currentState != MachineState.WaitingForInput)
                    {
                        currentState = MachineState.WaitingForInput;
                        Debug.Log($"[Machine] {machineData.machineName}: Waiting for input materials.");
                        OnWaitingForInput?.Invoke();
                    }
                }
            }
            else
            {
                // Generator (Wheat Field, Chicken Coop) — no input required
                _productionCoroutine = StartCoroutine(ProductionCycle());
            }
        }

        /// <summary>Coroutine that counts time for 1 production cycle.</summary>
        private IEnumerator ProductionCycle()
        {
            currentState = MachineState.Producing;
            currentProgress = 0f;
            OnProductionStarted?.Invoke();
            Debug.Log($"[Machine] {machineData.machineName}: Production started ({machineData.productionTime}s).");

            while (currentProgress < machineData.productionTime)
            {
                if (_isGameOver) yield break;
                currentProgress += Time.deltaTime;
                OnProductionProgress?.Invoke(currentProgress / machineData.productionTime);
                yield return null;
            }

            currentProgress = machineData.productionTime;
            currentState = MachineState.ReadyToHarvest;
            Debug.Log($"[Machine] {machineData.machineName}: READY TO HARVEST!");
            OnProductionComplete?.Invoke();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Public API

        /// <summary>Player clicks machine → harvest output.</summary>
        public void Harvest()
        {
            if (currentState != MachineState.ReadyToHarvest) return;

            if (OnRequestAddItem == null)
            {
                Debug.LogWarning($"[Machine] {machineData.machineName}: OnRequestAddItem delegate missing!");
                return;
            }

            // Check again before adding
            if (OnCheckInventoryFull != null && OnCheckInventoryFull(machineData.outputItem))
            {
                currentState = MachineState.InventoryFull;
                Debug.LogWarning($"[Machine] {machineData.machineName}: Cannot harvest — inventory full!");
                OnInventoryFull?.Invoke();
                return;
            }

            OnRequestAddItem(machineData.outputItem);
            Debug.Log($"[Machine] {machineData.machineName}: Harvested {machineData.outputItem.itemName}!");

            currentState = MachineState.Idle;
            OnHarvested?.Invoke();

            // Try to start the next cycle immediately
            TryStartProduction();
        }

        /// <summary>
        /// Called when game ends — stops all coroutines,
        /// fires no further harvest or production events.
        /// </summary>
        public void StopProduction()
        {
            _isGameOver = true;

            if (_productionCoroutine != null)
            {
                StopCoroutine(_productionCoroutine);
                _productionCoroutine = null;
            }

            if (_tickerCoroutine != null)
            {
                StopCoroutine(_tickerCoroutine);
                _tickerCoroutine = null;
            }

            Debug.Log($"[Machine] {machineData.machineName}: Production stopped (Game Over).");
            OnProductionStopped?.Invoke();
        }

        // ── Getters ──────────────────────────────────────────────────────────

        /// <summary>Returns the current machine state.</summary>
        public MachineState GetState() => currentState;

        /// <summary>Returns production progress from 0.0 to 1.0.</summary>
        public float GetProgress() => machineData != null && machineData.productionTime > 0
            ? currentProgress / machineData.productionTime
            : 0f;

        /// <summary>Returns machine name from SO data.</summary>
        public string GetMachineName() => machineData != null ? machineData.machineName : gameObject.name;

        #endregion
    }
}
