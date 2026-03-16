using System.Collections;
using UnityEngine;
using TMPro;
using FactoryRush.Scripts.Core;
using UnityEngine.SceneManagement;

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Handles the Game Over Result Screen.
    /// Animates the final score calculation and indicates high scores.
    /// </summary>
    public class ResultScreenUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private CanvasGroup panelCanvasGroup;

        [Header("Score Texts")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI deltaScoreText;
        [SerializeField] private GameObject newRecordPopup;

        [Header("Animation Settings")]
        [SerializeField] private float countUpDuration = 1.5f;
        [SerializeField] private float fadeInDuration = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color positiveDeltaColor = Color.green;
        [SerializeField] private Color negativeDeltaColor = Color.red;

        private int _finalGold;
        private int _bestGold;
        private bool _isCountingUp;
        private Coroutine _animationCoroutine;

        private void Start()
        {
            if (resultPanel != null) resultPanel.SetActive(false);
            if (newRecordPopup != null) newRecordPopup.SetActive(false);
            if (deltaScoreText != null) deltaScoreText.text = "";

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameOver.AddListener(ShowResultScreen);
            }
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameOver.RemoveListener(ShowResultScreen);
            }
        }

        // We can allow clicking to skip the animation
        private void Update()
        {
            if (_isCountingUp && Input.GetMouseButtonDown(0))
            {
                // Skip animation
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                }
                FinishAnimation();
            }
        }

        /// <summary>
        /// Triggered when the Game Over event fires.
        /// </summary>
        public void ShowResultScreen()
        {
            if (resultPanel == null) return;

            // 1. Data Retrieval
            _finalGold = ScoreManager.Instance != null ? ScoreManager.Instance.GetGold() : 0;
            _bestGold = PlayerPrefs.GetInt("BestScore", 0); // Placeholder for Lượng's Save System

            // 2. State Prep
            finalScoreText.text = "0";
            deltaScoreText.text = "";
            newRecordPopup.SetActive(false);

            resultPanel.SetActive(true);

            // 3. Start Animations
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 0f;
            }

            _isCountingUp = true;
            _animationCoroutine = StartCoroutine(AnimateScreen());
        }

        private IEnumerator AnimateScreen()
        {
            // --- Fade In Panel ---
            if (panelCanvasGroup != null)
            {
                float elapsedFade = 0f;
                while (elapsedFade < fadeInDuration)
                {
                    panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedFade / fadeInDuration);
                    elapsedFade += Time.deltaTime;
                    yield return null;
                }
                panelCanvasGroup.alpha = 1f;
            }

            // --- Count Up Animation ---
            float elapsedCount = 0f;
            while (elapsedCount < countUpDuration)
            {
                float progress = elapsedCount / countUpDuration;
                // Easing out curve
                float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

                int displayedScore = Mathf.RoundToInt(_finalGold * easedProgress);
                finalScoreText.text = displayedScore.ToString();

                elapsedCount += Time.deltaTime;
                yield return null;
            }

            FinishAnimation();
        }

        private void FinishAnimation()
        {
            _isCountingUp = false;
            finalScoreText.text = _finalGold.ToString();

            int delta = _finalGold - _bestGold;

            if (delta > 0)
            {
                deltaScoreText.text = $"+{delta}";
                deltaScoreText.color = positiveDeltaColor;

                // Save new local best
                PlayerPrefs.SetInt("BestScore", _finalGold);
                PlayerPrefs.Save();

                if (newRecordPopup != null) newRecordPopup.SetActive(true);
            }
            else
            {
                // Unchanged or lower
                deltaScoreText.text = $"{delta}"; // delta is 0 or negative
                deltaScoreText.color = delta == 0 ? Color.white : negativeDeltaColor;
            }
        }

        // --- Buttons exposed for UI Interaction ---

        public void OnRestartClicked()
        {
            // Simple reload of current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnMainMenuClicked()
        {
            // Load MainMenu scene. Assumes index 0 or name "MainMenu" exists.
            SceneManager.LoadScene("MainMenu");
        }
    }
}
