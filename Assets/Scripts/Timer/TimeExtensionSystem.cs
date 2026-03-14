using FactoryRush.Scripts.Core;
using UnityEngine;

/// <summary>
/// Xử lý logic mua thêm thời gian:
/// - kiểm tra giới hạn số lần mua
/// - trừ vàng
/// - cộng thêm thời gian vào timer
/// </summary>
public class TimeExtensionSystem : MonoBehaviour
{
    /// <summary>
    /// Instance singleton của TimeExtensionSystem.
    /// </summary>
    public static TimeExtensionSystem Instance { get; private set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>Số lần tối đa có thể mua thêm thời gian trong 1 lượt chơi.</summary>
    [Header("Config")]
    [SerializeField] private int maxExtensions = 2;

    /// <summary>Số lần đã sử dụng mua thêm thời gian trong lượt chơi hiện tại.</summary>
    [Header("State")]
    [SerializeField] private int extensionCount;

    /// <summary>Trả về true nếu người chơi vẫn còn có thể mua thêm thời gian.</summary>
    public bool CanExtend => extensionCount < maxExtensions;

    /// <summary>Số lần đã sử dụng chức năng mua thêm thời gian trong lượt chơi hiện tại.</summary>
    public int ExtensionCount => extensionCount;

    /// <summary>
    /// Đăng ký lắng nghe event OnGameStarted để reset số lần mua khi game bắt đầu.
    /// </summary>
    private void Start()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStarted.AddListener(ResetExtensions);
        else
            Debug.LogWarning("TimeExtensionSystem: GameStateManager.Instance is null in Start()");
    }

    /// <summary>
    /// Hủy đăng ký event khi object bị destroy để tránh memory leak.
    /// </summary>
    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStarted.RemoveListener(ResetExtensions);
    }

    /// <summary>
    /// Thử mua thêm thời gian.
    /// Quy trình:
    /// 1. Kiểm tra còn được phép mua thêm không (CanExtend)
    /// 2. Kiểm tra dữ liệu đầu vào hợp lệ
    /// 3. Kiểm tra các hệ thống cần thiết tồn tại
    /// 4. Kiểm tra trạng thái game đang Playing
    /// 5. Trừ vàng của người chơi
    /// 6. Cộng thêm thời gian vào Timer
    /// 7. Tăng số lần đã sử dụng
    /// </summary>
    /// <param name="goldCost">Số vàng cần trả để mua thêm thời gian.</param>
    /// <param name="secondsToAdd">Số giây được cộng thêm vào timer.</param>
    /// <returns>True nếu mua thành công, false nếu thất bại.</returns>
    public bool TryExtend(int goldCost, float secondsToAdd)
    {
        if (!CanExtend)
        {
            Debug.Log("TryExtend FAILED: extension limit reached");
            return false;
        }
        if (goldCost <= 0 || secondsToAdd <= 0f)
        {
            Debug.Log("TryExtend FAILED: invalid input");
            return false;
        }
        if (ScoreManager.Instance == null)
        {
            Debug.Log("TryExtend FAILED: ScoreManager is null");
            return false;
        }
        if (TimerManager.Instance == null)
        {
            Debug.Log("TryExtend FAILED: TimerManager is null");
            return false;
        }
        if (GameStateManager.Instance != null && GameStateManager.Instance.State != GameState.Playing)
        {
            Debug.Log($"TryExtend FAILED: state is {GameStateManager.Instance.State}, not Playing");
            return false;
        }
        if (!ScoreManager.Instance.SpendGold(goldCost))
        {
            Debug.Log($"TryExtend FAILED: not enough gold (need {goldCost}, have {ScoreManager.Instance.GetGold()})");
            return false;
        }

        TimerManager.Instance.AddTime(secondsToAdd);
        extensionCount++;
        Debug.Log($"TryExtend SUCCESS: -{goldCost} gold, +{secondsToAdd}s, extensionCount={extensionCount}");
        return true;
    }

    /// <summary>
    /// Reset lại số lần mua thêm thời gian khi game bắt đầu.
    /// Được gọi tự động qua OnGameStarted event.
    /// </summary>
    public void ResetExtensions()
    {
        extensionCount = 0;
        Debug.Log("TimeExtensionSystem: extensions reset");
    }

    /// <summary>Mua thêm 1 phút, tốn 50 vàng. Dùng để bind Inspector.</summary>
    public void TryExtendOneMinute() => TryExtend(50, 60f);

    /// <summary>Mua thêm 3 phút, tốn 120 vàng. Dùng để bind Inspector.</summary>
    public void TryExtendThreeMinutes() => TryExtend(120, 180f);

    /// <summary>Mua thêm 5 phút, tốn 180 vàng. Dùng để bind Inspector.</summary>
    public void TryExtendFiveMinutes() => TryExtend(180, 300f);
}