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
        WaitingForInput
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

        // Reference to external inventory logic (to be linked later)
        // We use a delegate/event approach to stay decoupled
        public delegate bool TakeItemsDelegate(List<ItemSO> items);
        public delegate void AddItemDelegate(ItemSO item);

        public TakeItemsDelegate OnRequestInputs;
        public AddItemDelegate OnRequestAddItem;

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
                if (currentState == MachineState.Idle || currentState == MachineState.WaitingForInput)
                {
                    TryStartProduction();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void TryStartProduction()
        {
            if (currentState != MachineState.Idle && currentState != MachineState.WaitingForInput) return;

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
            if (currentState != MachineState.ReadyToHarvest) return;

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
