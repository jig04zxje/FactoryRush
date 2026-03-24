using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FactoryRush.Scripts.Core;

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Handles the visual notification when an achievement is unlocked.
    /// Best placed on a UI Panel in the bottom-right corner.
    /// </summary>
    public class AchievementPopupUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup popupCanvasGroup; // Use CanvasGroup instead of SetActive
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image achievementIcon;
        
        [Header("Animation Settings")]
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float slideDuration = 0.5f;

        private RectTransform _rectTransform;
        private Vector2 _hiddenPos;
        private Vector2 _visiblePos;
        private bool _isShowing = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _visiblePos = _rectTransform.anchoredPosition;
            _hiddenPos = _visiblePos + new Vector2(500f, 0f); 
            _rectTransform.anchoredPosition = _hiddenPos;

            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 0f;
            }
        }

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void OnDisable()
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked.RemoveListener(QueueAchievement);
            }
        }

        private void TrySubscribe()
        {
            if (AchievementManager.Instance != null)
            {
                // Remove first to avoid double-subscription if both OnEnable and Start run
                AchievementManager.Instance.OnAchievementUnlocked.RemoveListener(QueueAchievement);
                AchievementManager.Instance.OnAchievementUnlocked.AddListener(QueueAchievement);
            }
        }

        private void QueueAchievement(AchievementSO ach)
        {
            if (_isShowing) return; 
            StartCoroutine(ShowNotification(ach));
        }

        private IEnumerator ShowNotification(AchievementSO ach)
        {
            _isShowing = true;
            
            // Set data
            if (titleText != null) titleText.text = ach.title;
            if (achievementIcon != null) achievementIcon.sprite = ach.icon;

            if (popupCanvasGroup != null) popupCanvasGroup.alpha = 1f;

            // Slide In
            yield return StartCoroutine(LerpPosition(_hiddenPos, _visiblePos, slideDuration));

            // Wait
            yield return new WaitForSeconds(displayDuration);

            // Slide Out
            yield return StartCoroutine(LerpPosition(_visiblePos, _hiddenPos, slideDuration));

            if (popupCanvasGroup != null) popupCanvasGroup.alpha = 0f;
            _isShowing = false;
        }

        private IEnumerator LerpPosition(Vector2 start, Vector2 end, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                _rectTransform.anchoredPosition = Vector2.Lerp(start, end, t);
                yield return null;
            }
            _rectTransform.anchoredPosition = end;
        }
    }
}
