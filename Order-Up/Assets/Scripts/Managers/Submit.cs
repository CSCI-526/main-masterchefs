using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    public RatingSystem ratingSystem;

    //// Connect this to your button's OnClick event in the Inspector
    //public void OnSubmitClicked()
    //{

    //    // Stop the global timer when the dish is submitted
    //    if (Timer.Instance != null)
    //    {
    //        float total = Timer.Instance.StopTimer();
    //        Debug.Log($"[Submit] Timer stopped at {total:F2}s ({Timer.Instance.FormattedElapsedTime}).");

    //        // Send current level and time spent to google form
    //        long sessionID = GameManager.Instance.SessionID;
    //        int level = GameManager.Instance.CurrentLevel;
    //        AnalyticsManager.Instance.Send(sessionID, level, total);
    //        GameManager.Instance.GoToNextLevel();
    //    }

    //    ratingSystem.SubmitDish();
    //}


    public void OnSubmitClicked()
    {
        // Stop the global timer when the dish is submitted
        if (Timer.Instance != null)
        {
            float total = Timer.Instance.StopTimer();
            Debug.Log($"[Submit] Timer stopped at {total:F2}s ({Timer.Instance.FormattedElapsedTime}).");

            // Send analytics data
            long sessionID = GameManager.Instance.SessionID;
            int level = GameManager.Instance.CurrentLevel;
            AnalyticsManager.Instance.Send(sessionID, level, total);

            // DON'T call GoToNextLevel here - let RatingSystem handle it based on stars
            // GameManager.Instance.GoToNextLevel();  // <-- REMOVE OR COMMENT THIS LINE
        }

        ratingSystem.SubmitDish();  // This will now handle progression based on stars
    }

}