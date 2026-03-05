using UnityEngine;

/// <summary>
/// Hệ thống lưu và tải điểm cao (High Scores).
/// Week 1: skeleton | Week 2: PlayerPrefs top 5 | Week 3: JSON + backward compatible.
/// </summary>
public static class SaveSystem
{
    /// <summary>
    /// Ghi nhận điểm số của người chơi vào bảng xếp hạng.
    /// </summary>
    /// <param name="gold">Số Gold đạt được trong lượt chơi.</param>
    public static void SubmitScore(int gold)
    {
        // TODO Week 2
    }

    /// <summary>
    /// Tải danh sách điểm cao đã lưu.
    /// </summary>
    /// <returns>Mảng điểm cao sắp xếp giảm dần (trả về mảng rỗng nếu chưa có dữ liệu).</returns>
    public static int[] LoadHighScores()
    {
        // TODO Week 2
        return new int[0];
    }
}

