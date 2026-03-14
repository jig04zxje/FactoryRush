using FactoryRush.Scripts.Core;
using TMPro;
using UnityEngine;

public class HighScorePanel : MonoBehaviour
{
    /// <summary>Text hiển thị toàn bộ top 5 scores.</summary>
    [SerializeField] private TextMeshProUGUI scoresText;

    /// <summary>
    /// Ẩn panel lúc đầu, đăng ký hiện lên khi game over.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(true);
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameOver.AddListener(Show);
            GameStateManager.Instance.OnGameStarted.AddListener(Hide); // ẩn khi Playing
        }
    }

    /// <summary>
    /// Hủy đăng ký sự kiện khi object bị destroy để tránh memory leak.
    /// </summary>
    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameOver.RemoveListener(Show);
            GameStateManager.Instance.OnGameStarted.RemoveListener(Hide);
        }
    }

    /// <summary>
    /// Hiển thị panel khi game over.
    /// </summary>
    private void Show() => gameObject.SetActive(true);
    
    /// <summary>
    /// Ẩn panel khi game bắt đầu.
    /// </summary>
    private void Hide() => gameObject.SetActive(false);

    /// <summary>
    /// Load và hiển thị scores mỗi lần panel hiện lên.
    /// </summary>
    private void OnEnable()
    {
        DisplayScores();
    }

    /// <summary>
    /// Load top 5 từ SaveSystem và hiển thị lên UI.
    /// </summary>
    private void DisplayScores()
    {
        if (scoresText == null) return;
        int[] scores = SaveSystem.LoadHighScores();
        if (scores.Length == 0)
        {
            scoresText.text = "Chưa có kết quả nào";
            return;
        }
        string result = "";
        for (int i = 0; i < scores.Length; i++)
            result += $"#{i + 1}: {scores[i]} gold\n";
        scoresText.text = result.TrimEnd();
    }

    /// <summary>Đóng panel high score.</summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}