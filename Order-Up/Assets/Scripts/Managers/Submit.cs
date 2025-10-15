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
        }

        ratingSystem.SubmitDish();
    }
}