using UnityEngine;
using TMPro;

public class ReviewManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI recipesCompletedText;
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private GameObject newCookwareContainer;
    [SerializeField] private TextMeshProUGUI newCookwareText;

    [Header("End Game")]
    [SerializeField] private GameObject thanksForPlayingText;

    [Header("Coin Data (Optional)")]
    [SerializeField] private int totalCoins = 0; // Manual entry or pull from another system

    private void Start()
    {
        UpdateReviewScreen();
    }

    private void UpdateReviewScreen()
    {
        // Access the static GameData class
        int reviewNumber = GameData.numberToSkipToReview;
        totalCoins = GameData.TotalCoin;

        // Hide thanks message by default
        if (thanksForPlayingText != null)
        {
            thanksForPlayingText.SetActive(false);
        }

        switch (reviewNumber)
        {
            case 1:
                SetReviewData("Level 1 Complete", 1, false, "");
                break;

            case 2:
                SetReviewData("Level 2 Complete", 4, false, "");
                break;

            case 3:
                SetReviewData("Level 3 Complete", 4, true, "Pan");
                break;

            case 4:
                SetReviewData("Level 4 Complete", 4, false, "");
                // Show thanks for playing message
                if (thanksForPlayingText != null)
                {
                    thanksForPlayingText.SetActive(true);
                }
                break;

            default:
                Debug.LogWarning($"Unhandled review number: {reviewNumber}. Current value: {reviewNumber}");
                break;
        }

        // Display total coins
        if (totalCoinsText != null)
        {
            totalCoinsText.text = $"Total Coins: {totalCoins}";
        }
    }

    private void SetReviewData(string level, int recipesCompleted, bool hasCookware, string cookwareName)
    {
        // Set level text
        if (levelText != null)
        {
            levelText.text = level;
        }

        // Set recipes completed text
        if (recipesCompletedText != null)
        {
            recipesCompletedText.text = $"Recipes Completed: {recipesCompleted}";
        }

        // Handle cookware display
        if (newCookwareContainer != null)
        {
            newCookwareContainer.SetActive(hasCookware);
        }

        if (newCookwareText != null)
        {
            if (hasCookware)
            {
                newCookwareText.text = $"New Cookware: {cookwareName}";
            }
            else
            {
                newCookwareText.text = "New Cookware: None";
            }
        }
    }

    // Optional: Method to manually trigger review update if needed
    public void RefreshReview()
    {
        UpdateReviewScreen();
    }

    // Optional: Method to set coins from external source
    public void SetTotalCoins(int coins)
    {
        totalCoins = coins;
        if (totalCoinsText != null)
        {
            totalCoinsText.text = $"Total Coins: {totalCoins}";
        }
    }
}