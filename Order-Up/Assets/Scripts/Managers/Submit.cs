using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    public RatingSystem ratingSystem;
    
    // Connect this to your button's OnClick event in the Inspector
    public void OnSubmitClicked()
    {
        ratingSystem.SubmitDish();
    }
}