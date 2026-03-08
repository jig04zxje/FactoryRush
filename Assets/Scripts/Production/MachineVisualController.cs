using UnityEngine;
using FactoryRush.Scripts.Production;

namespace FactoryRush.Scripts.Production
{
    /// <summary>
    /// Place this script on each Machine Prefab.
    /// Listens to events from MachineController and updates visuals accordingly.
    ///
    /// Unity Setup:
    ///   1. Add this component to the same GameObject as MachineController.
    ///   2. Assign child GameObjects (icons) to the corresponding slots in the Inspector.
    ///   3. No need to manually wire MachineController Events — this script auto-wires in Start().
    /// </summary>
    [RequireComponent(typeof(MachineController))]
    public class MachineVisualController : MonoBehaviour
    {
        [Header("Warning Icons (assign child GameObjects)")]
        [Tooltip("Icon '?' — displayed when waiting for input materials")]
        public GameObject waitingInputIcon;

        [Tooltip("Icon '!' — displayed when output inventory is full")]
        public GameObject inventoryFullIcon;

        [Tooltip("Icon '✓' or glow — displayed when ready to harvest")]
        public GameObject readyToHarvestIcon;

        [Header("Scale Pulse (while Producing)")]
        [Tooltip("Transform that will scale pulse. Usually the sprite child.")]
        public Transform pulseTarget;

        [Tooltip("Scale amount during pulse. E.g., 1.05")]
        public float pulseScale = 1.05f;

        [Tooltip("Pulse speed (cycles/second)")]
        public float pulseSpeed = 2f;

        // ── Private ──────────────────────────────────────────────────────────
        private MachineController _machine;
        private bool _isPulsing = false;
        private Vector3 _originalScale;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            _machine = GetComponent<MachineController>();

            if (pulseTarget != null)
                _originalScale = pulseTarget.localScale;
        }

        private void Start()
        {
            // Wire all events — no need to drag in the Inspector
            _machine.OnProductionStarted.AddListener(OnProductionStarted);
            _machine.OnProductionComplete.AddListener(OnReadyToHarvest);
            _machine.OnHarvested.AddListener(OnHarvested);
            _machine.OnWaitingForInput.AddListener(OnWaitingForInput);
            _machine.OnInventoryFull.AddListener(OnInventoryFull);
            _machine.OnProductionStopped.AddListener(OnProductionStopped);

            // Initialize initial visual state
            HideAllIcons();
        }

        private void OnDestroy()
        {
            if (_machine == null) return;
            _machine.OnProductionStarted.RemoveListener(OnProductionStarted);
            _machine.OnProductionComplete.RemoveListener(OnReadyToHarvest);
            _machine.OnHarvested.RemoveListener(OnHarvested);
            _machine.OnWaitingForInput.RemoveListener(OnWaitingForInput);
            _machine.OnInventoryFull.RemoveListener(OnInventoryFull);
            _machine.OnProductionStopped.RemoveListener(OnProductionStopped);
        }

        // ─────────────────────────────────────────────────────────────────────
        #region Update (Pulse Animation)

        private void Update()
        {
            if (!_isPulsing || pulseTarget == null) return;

            // Sinewave scale pulse
            float scaleMultiplier = 1f + (pulseScale - 1f) * Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
            pulseTarget.localScale = _originalScale * scaleMultiplier;
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Event Callbacks

        private void OnProductionStarted()
        {
            HideAllIcons();
            _isPulsing = true; // Start pulse animation
        }

        private void OnReadyToHarvest()
        {
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIcon(readyToHarvestIcon, true);
        }

        private void OnHarvested()
        {
            HideAllIcons();
            _isPulsing = false;
            ResetScale();
        }

        private void OnWaitingForInput()
        {
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIcon(waitingInputIcon, true);
        }

        private void OnInventoryFull()
        {
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIcon(inventoryFullIcon, true);
        }

        private void OnProductionStopped()
        {
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Helpers

        private void HideAllIcons()
        {
            SetIcon(waitingInputIcon, false);
            SetIcon(inventoryFullIcon, false);
            SetIcon(readyToHarvestIcon, false);
        }

        private void SetIcon(GameObject icon, bool active)
        {
            if (icon != null) icon.SetActive(active);
        }

        private void ResetScale()
        {
            if (pulseTarget != null)
                pulseTarget.localScale = _originalScale;
        }

        #endregion
    }
}
