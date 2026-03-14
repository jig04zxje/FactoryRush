using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Panel cho phép người chơi mua thêm thời gian.
/// Hiển thị số lần gia hạn còn lại và 3 gói gia hạn.
/// </summary>
public class TimeExtensionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI extensionCountText;
    [SerializeField] private Button extend1MinButton;
    [SerializeField] private Button extend3MinButton;
    [SerializeField] private Button extend5MinButton;

    /// <summary>
    /// Đăng ký sự kiện cho các button khi panel được khởi tạo.
    /// </summary>
    private void Awake()
    {
        extend1MinButton.onClick.AddListener(OnExtend1Min);
        extend3MinButton.onClick.AddListener(OnExtend3Min);
        extend5MinButton.onClick.AddListener(OnExtend5Min);
    }

    /// <summary>
    /// Cập nhật display mỗi lần panel hiện lên.
    /// </summary>
    private void OnEnable()
    {
        UpdateDisplay();
    }

    /// <summary>
    /// Cập nhật text hiển thị số lần gia hạn còn lại.
    /// </summary>
    private void UpdateDisplay()
    {
        if (TimeExtensionSystem.Instance == null) return;
        int remaining = 2 - TimeExtensionSystem.Instance.ExtensionCount;
        extensionCountText.text = $"Times Extended:{remaining}";
    }

    /// <summary>Mua thêm 1 phút, tốn 50 vàng.</summary>
    public void OnExtend1Min()
    {
        if (TimeExtensionSystem.Instance == null) return;
        TimeExtensionSystem.Instance.TryExtendOneMinute();
        UpdateDisplay();
    }

    /// <summary>Mua thêm 3 phút, tốn 120 vàng.</summary>
    public void OnExtend3Min()
    {
        if (TimeExtensionSystem.Instance == null) return;
        TimeExtensionSystem.Instance.TryExtendThreeMinutes();
        UpdateDisplay();
    }

    /// <summary>Mua thêm 5 phút, tốn 180 vàng.</summary>
    public void OnExtend5Min()
    {
        if (TimeExtensionSystem.Instance == null) return;
        TimeExtensionSystem.Instance.TryExtendFiveMinutes();
        UpdateDisplay();
    }

    /// <summary>Đóng panel gia hạn.</summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}