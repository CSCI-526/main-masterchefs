using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Attempts : MonoBehaviour
{
    public static Attempts Instance { get; private set; }

    [Header("Attempt Settings")]
    [SerializeField] private int maxAttempts = 3;
    private int currentAttempt = 0;
    private int starsEarned = 0;

    [Header("UI References")]
    public TextMeshProUGUI attemptText;
    public Image[] attemptIndicators = new Image[3]; // Visual indicators for attempts
    public Sprite attemptUsedSprite;
    public Sprite attemptAvailableSprite;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ResetAttempts();
        UpdateAttemptUI();
    }

    /// <summary>
    /// Call this method when the player submits a dish
    /// </summary>
    public void RecordAttempt(int stars)
    {
        currentAttempt++;
        starsEarned = Mathf.Max(starsEarned, stars); // Keep the best score

        if (enableDebugLogs)
            Debug.Log($"[Attempts] Attempt {currentAttempt}/{maxAttempts} - Stars: {stars} (Best: {starsEarned})");

        UpdateAttemptUI();

        // Check if player has used all attempts
        if (currentAttempt >= maxAttempts)
        {
            if (enableDebugLogs)
                Debug.Log($"[Attempts] All attempts used! Moving to next round...");
        }
    }

    /// <summary>
    /// Returns true if player has attempts remaining
    /// </summary>
    public bool HasAttemptsRemaining()
    {
        return currentAttempt < maxAttempts;
    }

    /// <summary>
    /// Returns the number of attempts remaining
    /// </summary>
    public int GetAttemptsRemaining()
    {
        return maxAttempts - currentAttempt;
    }

    /// <summary>
    /// Returns the current attempt number (1-indexed)
    /// </summary>
    public int GetCurrentAttempt()
    {
        return currentAttempt;
    }

    /// <summary>
    /// Returns the best star rating achieved
    /// </summary>
    public int GetBestStars()
    {
        return starsEarned;
    }

    /// <summary>
    /// Reset attempts for a new level
    /// </summary>
    public void ResetAttempts()
    {
        currentAttempt = 0;
        starsEarned = 0;

        if (enableDebugLogs)
            Debug.Log("[Attempts] Attempts reset for new level");

        UpdateAttemptUI();
    }

    /// <summary>
    /// Update the UI to show current attempts
    /// </summary>
    private void UpdateAttemptUI()
    {
        // Update text display
        if (attemptText != null)
        {
            attemptText.text = $"Attempts: {currentAttempt}/{maxAttempts}";
        }

        // Update visual indicators
        if (attemptIndicators != null && attemptIndicators.Length > 0)
        {
            for (int i = 0; i < attemptIndicators.Length; i++)
            {
                if (attemptIndicators[i] != null)
                {
                    // Show used/available sprites
                    if (i < currentAttempt)
                    {
                        attemptIndicators[i].sprite = attemptUsedSprite;
                    }
                    else
                    {
                        attemptIndicators[i].sprite = attemptAvailableSprite;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when all attempts are used - transitions are now handled by RatingSystem
    /// This method is kept for backwards compatibility but no longer handles transitions
    /// </summary>
    public void CompleteLevel()
    {
        if (enableDebugLogs)
            Debug.Log($"[Attempts] All attempts used! Best rating: {starsEarned} stars. Transition handled by RatingSystem.");
        
        // Note: Transitions are now handled by RatingSystem.TransitionToNextRound()
        // This method is kept for backwards compatibility
    }

    /// <summary>
    /// Optional: Allow manual progression if player gets 3 stars
    /// </summary>
    public void CheckForPerfectScore(int stars)
    {
        if (stars == 3)
        {
            if (enableDebugLogs)
                Debug.Log("[Attempts] Perfect score achieved! Player can choose to continue or advance.");

            // You could add UI here to let player choose to:
            // - Continue practicing (use remaining attempts)
            // - Move to next level immediately
        }
    }
}