using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để load scene

namespace FactoryRush.Scripts.Core
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("UI References")]
        [Tooltip("Kéo Canvas Group của màn hình đen vào đây")]
        public CanvasGroup fadeCanvasGroup;

        [Header("Settings")]
        public float fadeDuration = 0.5f; // Thời gian fade (nửa giây)

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }

        private IEnumerator TransitionCoroutine(string sceneName)
        {
            if (fadeCanvasGroup == null) yield break;

            // 1. Fade Out
            fadeCanvasGroup.blocksRaycasts = true;
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;

            // 2.LoadScene
            yield return SceneManager.LoadSceneAsync(sceneName);

            // 3.Fade In)
            time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false; 
        }
    }
}