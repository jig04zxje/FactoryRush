using UnityEngine;
using System.Collections;

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Slide panel vào/ra từ góc màn hình bằng animation mượt.
    /// Gắn lên GameObject của panel cần slide.
    /// </summary>
    public class SlidePanel : MonoBehaviour
    {
        [Header("Config")]
        /// <summary>
        /// Khoảng cách trượt ra ngoài màn hình (pixel).
        /// Nên lớn hơn width của panel một chút để ẩn hoàn toàn.
        /// </summary>
        [SerializeField] private float slideDistance = -800f;

        /// <summary>
        /// Thời gian hoàn thành animation slide (giây).
        /// Càng nhỏ càng nhanh.
        /// </summary>
        [SerializeField] private float slideDuration = 0.3f;

        /// <summary>RectTransform của panel, dùng để di chuyển vị trí UI.</summary>
        private RectTransform rectTransform;

        /// <summary>Vị trí khi panel đang mở (vị trí ban đầu trong Inspector).</summary>
        private Vector2 openPosition;

        /// <summary>Vị trí khi panel đang đóng (ngoài màn hình bên phải).</summary>
        private Vector2 closedPosition;

        /// <summary>Trạng thái hiện tại của panel.</summary>
        private bool isOpen = false;

        /// <summary>Tham chiếu coroutine đang chạy để tránh chạy nhiều coroutine cùng lúc.</summary>
        private Coroutine slideCoroutine;

        /// <summary>
        /// Đăng ký sự kiện khi game kết thúc để đóng panel.
        /// </summary>
        private void Start()
        {
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.OnGameOver.AddListener(Close);
        }

        /// <summary>
        /// Hủy đăng ký sự kiện khi object bị hủy để tránh lỗi.
        /// </summary>
        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.OnGameOver.RemoveListener(Close);
        }

        /// <summary>
        /// Lưu vị trí mở từ Inspector làm openPosition.
        /// Tính closedPosition bằng cách dịch sang phải slideDistance.
        /// Bắt đầu ở trạng thái đóng.
        /// </summary>
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            // Vị trí đặt trong Inspector = vị trí mở
            openPosition = rectTransform.anchoredPosition;

            // Vị trí đóng = dịch sang phải slideDistance → ẩn ngoài màn hình
            closedPosition = openPosition + new Vector2(slideDistance, 0f);

            // Bắt đầu ở trạng thái đóng
            rectTransform.anchoredPosition = closedPosition;

            // Ẩn panel khi bắt đầu
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Toggle mở/đóng panel.
        /// Dùng để bind vào button.
        /// </summary>
        public void Toggle()
        {
            if (isOpen) Close();
            else Open();
        }

        /// <summary>
        /// Mở panel — bật active rồi trượt vào.
        /// </summary>
        public void Open()
        {
            if (isOpen) return;
            isOpen = true;
            gameObject.SetActive(true); // bật trước
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(SlideTo(openPosition));
        }

        /// <summary>
        /// Đóng panel — trượt ra rồi tắt active.
        /// </summary>
        public void Close()
        {
            if (!isOpen) return;
            isOpen = false;
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(SlideAndHide());
        }

        /// <summary>
        /// Trượt ra rồi SetActive(false) sau khi animation xong.
        /// </summary>
        private IEnumerator SlideAndHide()
        {
            yield return StartCoroutine(SlideTo(closedPosition));
            gameObject.SetActive(false); // ẩn hẳn sau khi trượt xong
        }

        /// <summary>
        /// Coroutine di chuyển panel từ vị trí hiện tại đến target.
        /// Dùng SmoothStep để animation mượt — bắt đầu nhanh, kết thúc chậm dần.
        /// </summary>
        /// <param name="target">Vị trí đích cần di chuyển đến.</param>
        private IEnumerator SlideTo(Vector2 target)
        {
            Vector2 start = rectTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;

                // SmoothStep tạo easing tự nhiên thay vì di chuyển tuyến tính
                float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
                rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
                yield return null;
            }

            // Đảm bảo đến đúng vị trí target sau khi loop kết thúc
            rectTransform.anchoredPosition = target;
        }
    }
}
