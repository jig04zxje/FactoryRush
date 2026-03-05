using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Enum định nghĩa các trạng thái chính của game.
/// </summary>
public enum GameState
{
    /// <summary>Màn hình menu chính.</summary>
    MainMenu,
    /// <summary>Đang trong lượt chơi.</summary>
    Playing,
    /// <summary>Kết thúc lượt chơi.</summary>
    GameOver
}

/// <summary>
/// Quản lý trạng thái game (MainMenu, Playing, GameOver).
/// Sử dụng Singleton pattern để truy cập toàn cục.
/// Phát sự kiện khi trạng thái thay đổi để các hệ thống khác phản ứng.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>Singleton instance duy nhất của GameStateManager.</summary>
    public static GameStateManager Instance { get; private set; }

    /// <summary>Trạng thái hiện tại của game, mặc định là MainMenu.</summary>
    [SerializeField] private GameState state = GameState.MainMenu;

    /// <summary>Property public chỉ đọc để truy cập trạng thái game từ bên ngoài.</summary>
    public GameState State => state;

    /// <summary>Sự kiện được gọi khi game bắt đầu (chuyển sang Playing).</summary>
    public UnityEvent OnGameStarted;

    /// <summary>Sự kiện được gọi khi game kết thúc (chuyển sang GameOver).</summary>
    public UnityEvent OnGameOver;

    /// <summary>
    /// Khởi tạo Singleton. Nếu đã tồn tại instance khác thì hủy object này.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Bắt đầu game. Chuyển trạng thái sang Playing và phát sự kiện OnGameStarted.
    /// Bỏ qua nếu game đã đang chạy.
    /// </summary>
    public void StartGame()
    {
        if (state == GameState.Playing) return;
        state = GameState.Playing;
        OnGameStarted?.Invoke();
    }

    /// <summary>
    /// Kết thúc game. Chuyển trạng thái sang GameOver và phát sự kiện OnGameOver.
    /// Chỉ hoạt động khi đang ở trạng thái Playing.
    /// </summary>
    public void EndGame()
    {
        if (state != GameState.Playing) return;
        state = GameState.GameOver;
        OnGameOver?.Invoke();
    }
    
    /// <summary>
    /// Tải lại scene hiện tại (hard reset toàn bộ).
    /// </summary>
    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}