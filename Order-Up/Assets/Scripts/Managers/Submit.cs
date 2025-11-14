using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    public RatingSystem ratingSystem;

    /// <summary>
    /// Submit the dish and stop the timer, disable the button to prevent multiple submissions.
    /// </summary>
    public void OnSubmitClicked()
    {
        // Stop the global timer when the dish is submitted
        if (Timer.Instance != null)
        {
            float total = Timer.Instance.StopTimer();
            Debug.Log($"[Submit] Timer stopped at {total:F2}s ({Timer.Instance.FormattedElapsedTime}).");
            // Send current level and time spent to google form
            long sessionID = GameManager.Instance.SessionID;
            Debug.Log($"[Submit] Game Session ID: {sessionID}, Level: {GameData.CurrentLevel}, Round: {GameData.CurrentRound}");
            //AnalyticsManager.Instance.Send(sessionID, level, total);

        }

        ratingSystem.SubmitDish();
    }
    
}