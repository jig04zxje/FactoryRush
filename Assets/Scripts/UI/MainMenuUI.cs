using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Handles the Main Menu logic.
    /// Provides scene transitions and displays persistent High Score.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Image fadeOverlay;

        [Header("Settings")]
        [SerializeField] private string gameSceneName = "GamePlayScene";
        [SerializeField] private float fadeDuration = 0.5f;

        private void Start()
        {
            // Set up buttons
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(OnPlayClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitClicked);
            }

            // Setup High Score
            if (highScoreText != null)
            {
                int bestScore = PlayerPrefs.GetInt("BestScore", 0);
                if (bestScore > 0)
                {
                    highScoreText.text = $"Best Score: {bestScore} Gold";
                }
                else
                {
                    highScoreText.text = ""; // Hide if no score yet
                }
            }

            // Ensure overlay is transparent and raycast target is off initially
            if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = 0f;
                fadeOverlay.color = c;
                fadeOverlay.raycastTarget = false;
            }
        }

        private void OnPlayClicked()
        {
            // Disable buttons to prevent multiple clicks
            if (playButton != null) playButton.interactable = false;
            if (quitButton != null) quitButton.interactable = false;

            StartCoroutine(TransitionToGameScene());
        }

        private void OnQuitClicked()
        {
            Debug.Log("[MainMenu] Quit Application");
            Application.Quit();
        }

        private IEnumerator TransitionToGameScene()
        {
            // Fade out
            if (fadeOverlay != null)
            {
                fadeOverlay.raycastTarget = true; // Block other clicks

                float elapsed = 0f;
                Color c = fadeOverlay.color;

                while (elapsed < fadeDuration)
                {
                    c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                    fadeOverlay.color = c;

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                c.a = 1f;
                fadeOverlay.color = c;
            }

            // Load the scene asynchronously
            SceneManager.LoadSceneAsync(gameSceneName);
        }
    }
}
