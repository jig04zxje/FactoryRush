using UnityEngine;
using TMPro;
using FactoryRush.Scripts.Core;

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
            // Placeholder: Connect to Member Lương's TimerManager
            // float time = TimerManager.Instance.TimeRemaining;
            // timerText.text = FormatTime(time);

            // if (time < 30f) timerWarningEffect.SetActive(true);
        }

        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
