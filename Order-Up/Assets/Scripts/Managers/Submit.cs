using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    public RatingSystem ratingSystem;


    // Connect this to your button's OnClick event in the Inspector
    public void OnSubmitClicked()
    {

        // Stop the global timer when the dish is submitted
        if (Timer.Instance != null)
        {
            float total = Timer.Instance.StopTimer();
            Debug.Log($"[Submit] Timer stopped at {total:F2}s ({Timer.Instance.FormattedElapsedTime}).");

            sendTimeData(total);
        }

        ratingSystem.SubmitDish();
    }

    private void sendTimeData(float total)
    {
        // Send current level and time spent to google form
        long sessionID = GameManager.Instance.SessionID;
        int level = GameManager.Instance.CurrentLevel;
        AnalyticsManager.Instance.SendLevelTimeData(sessionID, level, total);
        GameManager.Instance.GoToNextLevel();
    }

    private void sendLevelCompleteData()
    {
        return;
    }
    
}