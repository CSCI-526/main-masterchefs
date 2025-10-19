using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Persistent game timer that can auto-start on a specific scene (e.g., Kitchen)
/// or be started manually (e.g., from GoToKitchen()).
/// Attach this to a GameObject in your bootstrap scene, or it will auto-create
/// itself the first time someone accesses Timer.Instance.
/// </summary>
public class Timer : MonoBehaviour
{
    // Singleton
    public static Timer Instance { get; private set; }

    [Header("Behavior")]
    [Tooltip("If true, the timer starts automatically when the Kitchen scene loads.")]
    public bool autoStartOnKitchenScene = true;

    [Tooltip("Scene name that should auto-start the timer when loaded.")]
    public string kitchenSceneName = "KitchenScene";

    [Tooltip("Use unscaled time so timer ignores timeScale changes (pauses, slow-mo).")]
    public bool useUnscaledDeltaTime = false;

    [Header("Optional UI")]
    [Tooltip("Optional TextMeshProUGUI to display the timer while running.")]
    public TextMeshProUGUI timerText;

    [Header("Debug")] public bool enableDebugLogs = false;

    private bool running = false;
    private float elapsedSeconds = 0f;

    // Event fired when the timer stops; passes total elapsed seconds
    public System.Action<float> OnTimerStopped;

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!running) return;
        if (enableDebugLogs) Debug.Log($"running: {running}");
        elapsedSeconds += useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
        UpdateText();
    }
    #endregion

    #region Scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!autoStartOnKitchenScene) return;

        if (scene.name == kitchenSceneName)
        {
            ResetTimer();
            StartTimer();

            if (enableDebugLogs)
                Debug.Log($"[Timer] Auto-started on scene '{scene.name}'.");
        }
    }
    #endregion

    #region Public API
    /// <summary>Start (or resume) the timer.</summary>
    public void StartTimer()
    {
        running = true;
        if (enableDebugLogs) Debug.Log("[Timer] Started.");
    }

    /// <summary>Stop the timer and return the elapsed time in seconds.</summary>
    public float StopTimer()
    {
        running = false;
        if (enableDebugLogs) { Debug.Log($"[Timer] Stopped at {elapsedSeconds:F2}s. Running state: {running}"); }
        OnTimerStopped?.Invoke(elapsedSeconds);
        return elapsedSeconds;
    }

    /// <summary>Reset the timer to 0 without starting it.</summary>
    public void ResetTimer()
    {
        elapsedSeconds = 0f;
        UpdateText();
        if (enableDebugLogs) Debug.Log("[Timer] Reset.");
    }

    /// <summary>Total elapsed seconds.</summary>
    public float ElapsedSeconds => elapsedSeconds;

    /// <summary>Formatted time string mm:ss (e.g., 01:23).</summary>
    public string FormattedElapsedTime
    {
        get
        {
            int totalSeconds = Mathf.FloorToInt(elapsedSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
    #endregion

    #region Helpers
    private void UpdateText()
    {
        if (timerText != null)
        {
            timerText.text = FormattedElapsedTime;
        }
    }
    #endregion
}
