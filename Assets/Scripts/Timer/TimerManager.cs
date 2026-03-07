using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Quản lý bộ đếm ngược trong game.
/// Sử dụng Singleton pattern để truy cập toàn cục.
/// </summary>
public class TimerManager : MonoBehaviour
{
    /// <summary>Singleton instance duy nhất của TimerManager.</summary>
    public static TimerManager Instance { get; private set; }

    [Header("Config")]
    /// <summary>Thời gian mặc định của bộ đếm ngược (giây).</summary>
    [SerializeField] private float defaultDurationSeconds = 300f;

    /// <summary>Ngưỡng cảnh báo khi thời gian còn lại ít (giây).</summary>
    [SerializeField] private float warningThresholdSeconds = 30f;

    [Header("State")]
    /// <summary>Thời gian còn lại hiện tại (giây).</summary>
    [SerializeField] private float timeRemaining;

    /// <summary>Property public để đọc thời gian còn lại từ bên ngoài.</summary>
    public float TimeRemaining => timeRemaining;

    /// <summary>Trạng thái đang chạy của bộ đếm ngược.</summary>
    public bool IsRunning { get; private set; }

    [Header("Events")]
    /// <summary>Sự kiện được gọi một lần khi thời gian còn lại dưới ngưỡng cảnh báo.</summary>
    public UnityEvent OnTimerWarning;

    /// <summary>Sự kiện được gọi khi bộ đếm ngược kết thúc (hết giờ).</summary>
    public UnityEvent OnTimerEnd;

    /// <summary>Tham chiếu đến coroutine đang chạy để có thể dừng khi cần.</summary>
    private Coroutine countdownRoutine;

    /// <summary>Cờ đánh dấu đã phát cảnh báo hay chưa, tránh gọi lặp lại.</summary>
    private bool warned;

    /// <summary>
    /// Khởi tạo Singleton. Nếu đã tồn tại instance khác thì hủy object này.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Lắng nghe event khi game bắt đầu → tự khởi động timer.
    /// Đăng ký gọi EndGame khi hết giờ.
    /// </summary>
    private void Start()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStarted.AddListener(StartTimerDefault);

            // Khi hết giờ → tự động kết thúc game
            OnTimerEnd.AddListener(GameStateManager.Instance.EndGame);
        }
    }

    /// <summary>
    /// Bắt đầu bộ đếm ngược với thời gian mặc định đã cấu hình.
    /// </summary>
    public void StartTimerDefault()
    {
        StartTimer(defaultDurationSeconds);
    }

    /// <summary>
    /// Bắt đầu bộ đếm ngược với số giây chỉ định.
    /// Tự động dừng timer cũ nếu đang chạy.
    /// </summary>
    /// <param name="seconds">Số giây cho bộ đếm ngược (tối thiểu 0).</param>
    public void StartTimer(float seconds)
    {
        StopTimer();

        timeRemaining = Mathf.Max(0f, seconds);
        warned = false;
        IsRunning = true;

        countdownRoutine = StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// Dừng bộ đếm ngược và hủy coroutine đang chạy.
    /// </summary>
    public void StopTimer()
    {
        IsRunning = false;

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
    }

    /// <summary>
    /// Coroutine thực hiện đếm ngược mỗi frame.
    /// Phát sự kiện cảnh báo khi gần hết giờ và sự kiện kết thúc khi hết giờ.
    /// </summary>
    private IEnumerator CountdownCoroutine()
    {
        while (IsRunning)
        {
            // Giảm thời gian theo thời gian thực mỗi frame
            timeRemaining -= Time.deltaTime;

            // Kiểm tra và phát cảnh báo một lần duy nhất khi dưới ngưỡng
            if (!warned && timeRemaining <= warningThresholdSeconds && timeRemaining > 0f)
            {
                warned = true;
                OnTimerWarning?.Invoke();
            }

            // Hết giờ → dừng timer và phát sự kiện kết thúc
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                IsRunning = false;
                OnTimerEnd?.Invoke();
                yield break;
            }

            // Chờ đến frame tiếp theo
            yield return null;
        }
    }

    /// <summary>
    /// Trả về thời gian còn lại dưới dạng chuỗi "mm:ss" để hiển thị lên UI.
    /// </summary>
    public string GetTimeText()
    {
        int t = Mathf.CeilToInt(timeRemaining);
        return $"{t / 60:00}:{t % 60:00}";
    }
}