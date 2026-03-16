using UnityEngine;

/// <summary>
/// ScriptableObject chứa dữ liệu Gold, dùng để chia sẻ state giữa các scene.
/// Tạo asset: Right-click → Create → Game → GoldData
/// </summary>
[CreateAssetMenu(fileName = "GoldData", menuName = "Game/GoldData")]
public class GoldData : ScriptableObject
{
    [Header("Config")]
    /// <summary>Số Gold ban đầu khi bắt đầu game.</summary>
    public int startingGold;

    [Header("State")]
    /// <summary>Tổng Gold hiện tại (thay đổi runtime).</summary>
    public int totalGold;

    /// <summary>
    /// Reset Gold về giá trị ban đầu.
    /// </summary>
    public void ResetToDefault()
    {
        totalGold = startingGold;
    }
}
