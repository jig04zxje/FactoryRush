using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }

/// <summary>
/// Quản lý điểm số (Gold) trong game.
/// Sử dụng Singleton pattern để truy cập toàn cục.
/// Phát sự kiện OnGoldChanged mỗi khi số Gold thay đổi.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>Singleton instance duy nhất của ScoreManager.</summary>
    public static ScoreManager Instance { get; private set; }

    [Header("Config")]
    /// <summary>Tham chiếu đến ScriptableObject chứa dữ liệu Gold.</summary>
    [SerializeField] private GoldData goldData;

    /// <summary>Property public chỉ đọc để truy cập tổng Gold từ bên ngoài.</summary>
    public int TotalGold => goldData.totalGold;

    [Header("Events")]
    /// <summary>Sự kiện được gọi mỗi khi Gold thay đổi, truyền kèm giá trị Gold mới.</summary>
    public IntUnityEvent OnGoldChanged;

    /// <summary>
    /// Khởi tạo Singleton. Nếu đã tồn tại instance khác thì hủy object này.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Khởi tạo Gold từ config và phát sự kiện cho UI.
    /// </summary>
    private void Start()
    {
        goldData.ResetToDefault();
        OnGoldChanged?.Invoke(goldData.totalGold);
    }

    /// <summary>
    /// Cộng thêm Gold cho người chơi và phát sự kiện thông báo thay đổi.
    /// </summary>
    /// <param name="amount">Số Gold cần cộng thêm (bỏ qua nếu &lt;= 0).</param>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        goldData.totalGold += amount;
        OnGoldChanged?.Invoke(goldData.totalGold);
    }

    /// <summary>
    /// Lấy số Gold hiện tại.
    /// </summary>
    public int GetGold() => goldData.totalGold;

    /// <summary>
    /// Đặt lại Gold về 0 và phát sự kiện thông báo thay đổi.
    /// </summary>
    public void ResetGold()
    {
        goldData.totalGold = 0;
        OnGoldChanged?.Invoke(goldData.totalGold);
    }
}
