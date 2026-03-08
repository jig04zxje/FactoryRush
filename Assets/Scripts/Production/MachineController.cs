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
        WaitingForInput,
        Producing,
        ReadyToHarvest
    }

    public class MachineController : MonoBehaviour
    {
        [Header("Data")]
        public MachineSO machineData;

        [Header("State")]
        [SerializeField] private MachineState currentState = MachineState.Idle;
        [SerializeField] private float currentProgress = 0f;

        [Header("Events")]
        public UnityEvent OnProductionStarted = new UnityEvent();
        public UnityEvent<float> OnProductionProgress = new UnityEvent<float>(); // 0 to 1
        public UnityEvent OnProductionComplete = new UnityEvent();
        public UnityEvent OnHarvested = new UnityEvent();

        // Reference to external inventory logic (to be linked later)
        public delegate bool TakeItemsDelegate(List<ItemSO> inputs);
        public delegate void AddItemDelegate(ItemSO item);

        public TakeItemsDelegate OnRequestInputs;
        public AddItemDelegate OnRequestAddItem;

        private void Start()
        {
            if (machineData == null)
            {
                Debug.LogError($"[Machine] MachineData missing on {gameObject.name}!");
                return;
            }

            // Register with global manager
            if (ProductionManager.Instance != null)
            {
                ProductionManager.Instance.RegisterMachine(this);
            }
            else
            {
                Debug.LogWarning($"[Machine] ProductionManager Instance not found for {gameObject.name}!");
            }

            // Start production checking
            StartCoroutine(AutoProductionTicker());
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
                Debug.Log($"[Machine] {machineData.machineName} starting automatic production.");
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
            Debug.Log($"[Machine] {machineData.machineName} is READY TO HARVEST!");
            OnProductionComplete?.Invoke();
        }

        public void Harvest()
        {
            if (currentState != MachineState.ReadyToHarvest) return;

            if (OnRequestAddItem != null)
            {
                OnRequestAddItem(machineData.outputItem);
                Debug.Log($"[Machine] {machineData.machineName} Harvested: {machineData.outputItem.itemName}!");
                currentState = MachineState.Idle;
                OnHarvested?.Invoke();

                // Restart production if possible
                TryStartProduction();
            }
            else
            {
                Debug.LogWarning($"[Machine] {machineData.machineName} cannot harvest because OnRequestAddItem delegate is missing!");
            }
        }

        public MachineState GetState() => currentState;
        public float GetProgress() => currentProgress / machineData.productionTime;
    }
}
