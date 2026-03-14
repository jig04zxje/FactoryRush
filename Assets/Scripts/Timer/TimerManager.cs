using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FactoryRush.Scripts.Timer
{
    /// <summary>
    /// Quản lý bộ đếm thời gian của game.
    /// Sử dụng Singleton pattern để có thể truy cập toàn cục.
    /// </summary>
    public class TimerManager : MonoBehaviour
    {
        /// <summary>
        /// Instance duy nhất của TimerManager (Singleton).
        /// </summary>
        public static TimerManager Instance { get; private set; }

        [Header("Config")]

        /// <summary>
        /// Thời gian mặc định của countdown timer (tính bằng giây).
        /// </summary>
        [SerializeField] private float defaultDurationSeconds = 300f;

        /// <summary>
        /// Ngưỡng cảnh báo khi thời gian còn lại thấp (giây).
        /// </summary>
        [SerializeField] private float warningThresholdSeconds = 30f;

        [Header("State")]

        /// <summary>
        /// Thời gian còn lại hiện tại của timer (giây).
        /// </summary>
        [SerializeField] private float timeRemaining;

        /// <summary>
        /// Property public để đọc thời gian còn lại.
        /// </summary>
        public float TimeRemaining => timeRemaining;

        /// <summary>
        /// Trạng thái đang chạy của timer.
        /// </summary>
        public bool IsRunning { get; private set; }

        [Header("Events")]

        /// <summary>
        /// Event được gọi một lần khi thời gian còn lại nhỏ hơn ngưỡng cảnh báo.
        /// </summary>
        public UnityEvent OnTimerWarning = new UnityEvent();

        /// <summary>
        /// Event được gọi khi thời gian kết thúc (hết giờ).
        /// </summary>
        public UnityEvent OnTimerEnd = new UnityEvent();

        /// <summary>
        /// Tham chiếu tới coroutine đang chạy để có thể dừng khi cần.
        /// </summary>
        private Coroutine countdownRoutine;

        /// <summary>
        /// Đánh dấu xem cảnh báo đã được kích hoạt hay chưa.
        /// </summary>
        private bool warned;

        /// <summary>
        /// Khởi tạo Singleton.
        /// Nếu đã tồn tại instance thì destroy object trùng.
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
        /// Lắng nghe event khi game bắt đầu để tự động start timer.
        /// Đồng thời đăng ký EndGame khi timer kết thúc.
        /// </summary>
        private void Start()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameStarted.AddListener(StartTimerDefault);

                // Tự động kết thúc game khi hết thời gian
                OnTimerEnd.AddListener(GameStateManager.Instance.EndGame);
            }
        }

        /// <summary>
        /// Bắt đầu timer với thời gian mặc định đã cấu hình.
        /// </summary>
        public void StartTimerDefault()
        {
            StartTimer(defaultDurationSeconds);
        }

        /// <summary>
        /// Bắt đầu countdown timer với số giây chỉ định.
        /// Nếu timer đang chạy thì sẽ dừng timer cũ trước.
        /// </summary>
        /// <param name="seconds">Số giây của countdown timer.</param>
        public void StartTimer(float seconds)
        {
            StopTimer();

            timeRemaining = Mathf.Max(0f, seconds);
            warned = false;
            IsRunning = true;

            countdownRoutine = StartCoroutine(CountdownCoroutine());
        }

        /// <summary>
        /// Dừng countdown timer và hủy coroutine đang chạy.
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
        /// Cộng thêm thời gian vào timer hiện tại.
        /// </summary>
        public void AddTime(float secondsToAdd)
        {
            if (secondsToAdd <= 0f) return;
            if (!IsRunning) return;

            timeRemaining += secondsToAdd;

            // Cho phép trigger lại cảnh báo nếu thời gian tăng lên trên threshold
            if (timeRemaining > warningThresholdSeconds)
                warned = false;
        }

        /// <summary>
        /// Coroutine thực hiện countdown mỗi frame.
        /// Khi thời gian thấp sẽ kích hoạt cảnh báo.
        /// Khi hết thời gian sẽ kích hoạt event kết thúc.
        /// </summary>
        private IEnumerator CountdownCoroutine()
        {
            while (IsRunning)
            {
                // Giảm thời gian theo thời gian thực mỗi frame
                timeRemaining -= Time.deltaTime;

                // Kiểm tra và kích hoạt cảnh báo khi xuống dưới threshold
                if (!warned && timeRemaining <= warningThresholdSeconds && timeRemaining > 0f)
                {
                    warned = true;
                    OnTimerWarning?.Invoke();
                }

                // Hết thời gian
                if (timeRemaining <= 0f)
                {
                    timeRemaining = 0f;
                    IsRunning = false;
                    OnTimerEnd?.Invoke();
                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Trả về chuỗi thời gian dạng "mm:ss" để hiển thị trên UI.
        /// </summary>
        public string GetTimeText()
        {
            int t = Mathf.CeilToInt(timeRemaining);
            return $"{t / 60:00}:{t % 60:00}";
        }
    }
}