using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Hiển thị điểm số cao nhất (HighScore) trên UI.
/// Lắng nghe sự kiện OnGoldChanged từ ScoreManager để cập nhật giá trị.
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    /// <summary>
    /// Text UI hiển thị điểm số.
    /// </summary>
    [SerializeField] private Text totalGoldText;

    /// <summary>
    /// Khởi tạo và đăng ký sự kiện tính điểm số cao nhất. 
    /// Cập nhật hiển thị ngay khi bắt đầu.
    /// </summary>
    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnGoldChanged.AddListener(UpdateDisplay);
            UpdateDisplay(ScoreManager.Instance.GetGold());
        }
    }

    /// <summary>
    /// Cập nhật hiển thị điểm số cao nhất trên UI.
    /// </summary>
    /// <param name="gold">Số Gold hiện tại.</param>
    public void UpdateDisplay(int gold)
    {
        totalGoldText.text = $"Total Gold: {gold}";
    }

}
