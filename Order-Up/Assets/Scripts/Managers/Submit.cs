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
        // if (Timer.Instance != null)
        // {
        //     float total = Timer.Instance.StopTimer();
        //     Debug.Log($"[Submit] Timer stopped at {total:F2}s ({Timer.Instance.FormattedElapsedTime}).");
        //     
        //
        // } // logic moved to rating system
        
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnOrderUpClicked();
        }

        ratingSystem.SubmitDish();
    }
    
    
}