using UnityEngine;
using TMPro;
using FactoryRush.Scripts.Core;
using FactoryRush.Scripts.Timer;

namespace FactoryRush.Scripts.UI
{
    public class HUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject timerWarningEffect;

        private void Start()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnGoldChanged.AddListener(OnGoldChanged);
                UpdateGoldDisplay(ScoreManager.Instance.GetGold());
            }
        }

        private void OnDestroy()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnGoldChanged.RemoveListener(OnGoldChanged);
            }
        }

        private void OnGoldChanged(int totalGold)
        {
            UpdateGoldDisplay(totalGold);
        }

        private void UpdateGoldDisplay(int gold)
        {
            if (goldText != null)
                goldText.text = gold.ToString();
        }

        private void Update()
        {
            UpdateTimerUI();
        }

        private void UpdateTimerUI()
        {
            // Ensure timer text updates if uncommented.
            if (TimerManager.Instance == null) return;

            float time = TimerManager.Instance.TimeRemaining;
            if (timerText != null)
                timerText.text = FormatTime(time);

            // Show warning effect when < 30 seconds
            if (timerWarningEffect != null)
                timerWarningEffect.SetActive(time < 30f && TimerManager.Instance.IsRunning);
        }

        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
