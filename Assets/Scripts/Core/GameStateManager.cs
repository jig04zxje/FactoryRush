using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Enum defining the main game states.
/// </summary>
public enum GameState
{
    /// <summary>Main menu screen.</summary>
    MainMenu,
    /// <summary>Active gameplay.</summary>
    Playing,
    /// <summary>Game over screen.</summary>
    GameOver
}

/// <summary>
/// Manages the game state (MainMenu, Playing, GameOver).
/// Uses Singleton pattern for global access.
/// Fires events on state changes for other systems to react.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    /// <summary>Singleton instance of GameStateManager.</summary>
    public static GameStateManager Instance { get; private set; }

    /// <summary>Current game state, default is MainMenu.</summary>
    [SerializeField] private GameState state = GameState.MainMenu;

    /// <summary>Read-only property to access game state.</summary>
    public GameState State => state;

    /// <summary>Event fired when the game starts (Playing state).</summary>
    public UnityEvent OnGameStarted = new UnityEvent();

    /// <summary>Event fired when the game ends (GameOver state).</summary>
    public UnityEvent OnGameOver = new UnityEvent();

    /// <summary>
    /// Initializes the Singleton. Destroys duplicate instances.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // BUG FIX: Added DontDestroyOnLoad so game state persists across scene reloads.
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Starts the game. Changes state to Playing and fires OnGameStarted.
    /// Ignored if game is already running.
    /// </summary>
    public void StartGame()
    {
        if (state == GameState.Playing) return;
        state = GameState.Playing;
        OnGameStarted?.Invoke();
    }

    /// <summary>
    /// Ends the game. Changes state to GameOver and fires OnGameOver.
    /// Only works when in Playing state.
    /// </summary>
    public void EndGame()
    {
        if (state != GameState.Playing) return;
        state = GameState.GameOver;
        OnGameOver?.Invoke();
    }

    /// <summary>
    /// Reloads the current scene (hard reset).
    /// </summary>
    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}