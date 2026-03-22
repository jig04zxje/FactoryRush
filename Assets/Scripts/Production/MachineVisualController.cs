using UnityEngine;
using FactoryRush.Scripts.Production;

namespace FactoryRush.Scripts.Production
{
    [RequireComponent(typeof(MachineController))]
    public class MachineVisualController : MonoBehaviour
    {
        [Header("Animator")]
        [Tooltip("Leave empty — script auto-finds Animator in children")]
        public Animator animatorTarget;

        [Header("Warning Icons (assign child GameObjects)")]
        [Tooltip("Icon '?' — displayed when waiting for input materials")]
        public GameObject waitingInputIcon;

        [Tooltip("Icon '!' — displayed when output inventory is full")]
        public GameObject inventoryFullIcon;

        [Tooltip("Icon check or glow — displayed when ready to harvest")]
        public GameObject readyToHarvestIcon;

        [Header("Scale Pulse (while Producing)")]
        [Tooltip("Transform to scale pulse. Usually the sprite child.")]
        public Transform pulseTarget;

        [Tooltip("Scale multiplier during pulse. E.g., 1.05 = 5% bigger")]
        public float pulseScale = 1.05f;

        [Tooltip("Pulse speed in cycles per second")]
        public float pulseSpeed = 2f;

        // ── Private fields ────────────────────────────────────────
        private MachineController _machine;
        private bool _isPulsing = false;
        private Vector3 _originalScale;

        // Animator parameter names — must match exactly in the Animator window
        private const string PARAM_RUNNING = "IsRunning";
        private const string PARAM_READY = "IsReady";

        // ─────────────────────────────────────────────────────────
        private void Awake()
        {
            _machine = GetComponent<MachineController>();

            // Auto-find Animator if not manually assigned
            if (animatorTarget == null)
                animatorTarget = GetComponentInChildren<Animator>();

            if (pulseTarget != null)
                _originalScale = pulseTarget.localScale;
        }

        private void Start()
        {
            // Subscribe to MachineController events — no manual wiring needed
            _machine.OnProductionStarted.AddListener(OnProductionStarted);
            _machine.OnProductionComplete.AddListener(OnReadyToHarvest);
            _machine.OnHarvested.AddListener(OnHarvested);
            _machine.OnWaitingForInput.AddListener(OnWaitingForInput);
            _machine.OnInventoryFull.AddListener(OnInventoryFull);
            _machine.OnProductionStopped.AddListener(OnProductionStopped);

            // Initialize to idle state on start
            HideAllIcons();
            SetIdle();
        }

        private void OnDestroy()
        {
            if (_machine == null) return;

            // Unsubscribe to prevent memory leaks
            _machine.OnProductionStarted.RemoveListener(OnProductionStarted);
            _machine.OnProductionComplete.RemoveListener(OnReadyToHarvest);
            _machine.OnHarvested.RemoveListener(OnHarvested);
            _machine.OnWaitingForInput.RemoveListener(OnWaitingForInput);
            _machine.OnInventoryFull.RemoveListener(OnInventoryFull);
            _machine.OnProductionStopped.RemoveListener(OnProductionStopped);
        }

        // ─────────────────────────────────────────────────────────
        #region Update — Pulse Animation

        private void Update()
        {
            if (!_isPulsing || pulseTarget == null) return;

            // Sine wave scale pulse while producing
            float s = 1f + (pulseScale - 1f) * Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
            pulseTarget.localScale = _originalScale * s;
        }

        #endregion

        // ─────────────────────────────────────────────────────────
        #region Event Callbacks

        private void OnProductionStarted()
        {
            HideAllIcons();
            _isPulsing = true;
            SetRunning();
        }

        private void OnReadyToHarvest()
        {
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIcon(readyToHarvestIcon, true);
            SetReady();
        }

        private void OnHarvested()
        {
            HideAllIcons();
            _isPulsing = false;
            ResetScale();
            SetIdle();
        }

        private void OnWaitingForInput()
        {
            // Not enough materials — return to idle visually
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIcon(waitingInputIcon, true);
            SetIdle();
        }

        private void OnInventoryFull()
        {
            // Output inventory full — block production visually
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIcon(inventoryFullIcon, true);
            SetIdle();
        }

        private void OnProductionStopped()
        {
            // Game over — stop all visuals
            _isPulsing = false;
            ResetScale();
            HideAllIcons();
            SetIdle();
        }

        #endregion

        // ─────────────────────────────────────────────────────────
        #region Animator State Helpers

        // IsRunning=false, IsReady=false
        private void SetIdle()
        {
            if (animatorTarget == null) return;
            animatorTarget.SetBool(PARAM_RUNNING, false);
            animatorTarget.SetBool(PARAM_READY, false);
        }

        // IsRunning=true, IsReady=false
        private void SetRunning()
        {
            if (animatorTarget == null) return;
            animatorTarget.SetBool(PARAM_RUNNING, true);
            animatorTarget.SetBool(PARAM_READY, false);
        }

        // IsRunning=false, IsReady=true
        private void SetReady()
        {
            if (animatorTarget == null) return;
            animatorTarget.SetBool(PARAM_RUNNING, false);
            animatorTarget.SetBool(PARAM_READY, true);
        }

        #endregion

        // ─────────────────────────────────────────────────────────
        #region Icon and Scale Helpers

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