using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private Text timerText;

    private void Awake()
    {
        if (timerText == null)
            timerText = GetComponent<Text>();
    }

    private void Update()
    {
        if (TimerManager.Instance == null || timerText == null) return;
        timerText.text = TimerManager.Instance.GetTimeText();
    }
}
