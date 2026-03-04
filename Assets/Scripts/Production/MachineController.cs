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
        Producing,
        ReadyToHarvest,
        WaitingForInput,
        WaitingForInventory
    }

    public class MachineController : MonoBehaviour
    {
        [Header("Settings")]
        public MachineSO machineData;

        [Header("State")]
        [SerializeField] private MachineState currentState = MachineState.Idle;
        [SerializeField] private float currentProgress = 0f;

        [Header("Events")]
        public UnityEvent OnProductionStarted;
        public UnityEvent<float> OnProductionProgress; // 0 to 1
        public UnityEvent OnProductionComplete;
        public UnityEvent OnHarvested;

        [Header("Warnings")]
        public UnityEvent OnWarningInputsMissing;
        public UnityEvent OnWarningInventoryFull;

        // Reference to external inventory logic (to be linked later)
        public delegate bool TakeItemsDelegate(List<ItemSO> items);
        public delegate void AddItemDelegate(ItemSO item);
        public delegate bool CheckCapacityDelegate(ItemSO item);

        public TakeItemsDelegate OnRequestInputs;
        public AddItemDelegate OnRequestAddItem;
        public CheckCapacityDelegate OnCheckCapacity;

        private void Start()
        {
            if (machineData == null)
            {
                Debug.LogError($"MachineData missing on {gameObject.name}");
                return;
            }

            // Register with global manager
            if (ProductionManager.Instance != null)
            {
                ProductionManager.Instance.RegisterMachine(this);
            }

            // Start production checking
            StartCoroutine(AutoProductionTicker());
        }

        private void OnDestroy()
        {
            if (ProductionManager.Instance != null)
            {
                ProductionManager.Instance.UnregisterMachine(this);
            }
        }

        private IEnumerator AutoProductionTicker()
        {
            while (true)
            {
                if (currentState == MachineState.Idle || currentState == MachineState.WaitingForInput || currentState == MachineState.WaitingForInventory)
                {
                    TryStartProduction();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void TryStartProduction()
        {
            if (currentState != MachineState.Idle && currentState != MachineState.WaitingForInput && currentState != MachineState.WaitingForInventory) return;

            // Check if inventory is full for the output item before even starting
            if (OnCheckCapacity != null && OnCheckCapacity(machineData.outputItem))
            {
                currentState = MachineState.WaitingForInventory;
                OnWarningInventoryFull?.Invoke();
                return;
            }

            // Check if we have delegates hooked up or if we need inputs
            if (machineData.requiredInputs.Count > 0)
            {
                if (OnRequestInputs != null && OnRequestInputs(machineData.requiredInputs))
                {
                    StartCoroutine(ProductionCycle());
                }
                else
                {
                    currentState = MachineState.WaitingForInput;
                    OnWarningInputsMissing?.Invoke();
                }
            }
            else
            {
                // Generators like Wheat Field don't need input
                StartCoroutine(ProductionCycle());
            }
        }

        private IEnumerator ProductionCycle()
        {
            currentState = MachineState.Producing;
            currentProgress = 0f;
            OnProductionStarted?.Invoke();

            while (currentProgress < machineData.productionTime)
            {
                currentProgress += Time.deltaTime;
                OnProductionProgress?.Invoke(currentProgress / machineData.productionTime);
                yield return null;
            }

            currentProgress = machineData.productionTime;
            currentState = MachineState.ReadyToHarvest;
            OnProductionComplete?.Invoke();
        }

        public void Harvest()
        {
            if (currentState != MachineState.ReadyToHarvest && currentState != MachineState.WaitingForInventory) return;

            // Final check on capacity before adding
            if (OnCheckCapacity != null && OnCheckCapacity(machineData.outputItem))
            {
                currentState = MachineState.WaitingForInventory;
                OnWarningInventoryFull?.Invoke();
                return;
            }

            if (OnRequestAddItem != null)
            {
                OnRequestAddItem(machineData.outputItem);
                currentState = MachineState.Idle;
                OnHarvested?.Invoke();

                // Try to start next cycle immediately
                TryStartProduction();
            }
            else
            {
                Debug.LogWarning("Cannot harvest: No Inventory delegate provider linked!");
            }
        }

        // For UI/Interaction
        public MachineState GetState() => currentState;
        public float GetProgress() => currentProgress / machineData.productionTime;
    }
}
