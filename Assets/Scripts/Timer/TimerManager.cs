using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the game's countdown timer.
/// Uses Singleton pattern for global access.
/// </summary>
public class TimerManager : MonoBehaviour
{
    /// <summary>Singleton instance of TimerManager.</summary>
    public static TimerManager Instance { get; private set; }

    [Header("Config")]
    /// <summary>Default duration of the countdown timer (seconds).</summary>
    [SerializeField] private float defaultDurationSeconds = 300f;

    /// <summary>Warning threshold when time remaining is low (seconds).</summary>
    [SerializeField] private float warningThresholdSeconds = 30f;

    [Header("State")]
    /// <summary>Current time remaining (seconds).</summary>
    [SerializeField] private float timeRemaining;

    /// <summary>Public property to read the remaining time.</summary>
    public float TimeRemaining => timeRemaining;

    /// <summary>Running state of the countdown timer.</summary>
    public bool IsRunning { get; private set; }

    [Header("Events")]
    /// <summary>Event fired once when remaining time is below the warning threshold.</summary>
    public UnityEvent OnTimerWarning = new UnityEvent();

    /// <summary>Event fired when the countdown timer ends (time's up).</summary>
    public UnityEvent OnTimerEnd = new UnityEvent();

    /// <summary>Reference to the running coroutine to stop it when needed.</summary>
    private Coroutine countdownRoutine;

    /// <summary>Flag marking whether the warning has been fired.</summary>
    private bool warned;

    /// <summary>
    /// Initializes the Singleton. Destroys duplicate instances.
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
    /// Listens for the game started event to auto-start the timer.
    /// Registers to call EndGame when time is up.
    /// </summary>
    private void Start()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStarted.AddListener(StartTimerDefault);

            // Automatically end the game when time is up
            OnTimerEnd.AddListener(GameStateManager.Instance.EndGame);
        }
    }

    /// <summary>
    /// Starts the countdown timer with the configured default duration.
    /// </summary>
    public void StartTimerDefault()
    {
        StartTimer(defaultDurationSeconds);
    }

    /// <summary>
    /// Starts the countdown timer with the specified seconds.
    /// Automatically stops the old timer if it's running.
    /// </summary>
    /// <param name="seconds">Number of seconds for the countdown timer.</param>
    public void StartTimer(float seconds)
    {
        StopTimer();

        timeRemaining = Mathf.Max(0f, seconds);
        warned = false;
        IsRunning = true;

        countdownRoutine = StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// Stops the countdown timer and cancels the running coroutine.
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
    /// Coroutine that performs the countdown every frame.
    /// Fires the warning event when time is low and the end event when time is up.
    /// </summary>
    private IEnumerator CountdownCoroutine()
    {
        while (IsRunning)
        {
            // Decrease time based on real time every frame
            timeRemaining -= Time.deltaTime;

            // Check and fire warning once when below threshold
            if (!warned && timeRemaining <= warningThresholdSeconds && timeRemaining > 0f)
            {
                warned = true;
                OnTimerWarning?.Invoke();
            }

            // Time's up -> stop timer and fire end event
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
    /// Returns the remaining time as an "mm:ss" string for UI display.
    /// </summary>
    public string GetTimeText()
    {
        int t = Mathf.CeilToInt(timeRemaining);
        return $"{t / 60:00}:{t % 60:00}";
    }
}