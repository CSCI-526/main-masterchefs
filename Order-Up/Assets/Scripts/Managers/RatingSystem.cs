using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RatingSystem : MonoBehaviour
{
    [Header("Rating Settings")]
    public Transform plateTransform; // Reference to the plate where dishes are created
    public bool enableDebugLogs = true;
    public GameObject evaluationPanel;
    public TMPro.TextMeshProUGUI evaluationResults;
    public Image[] starDisplay = new Image[3];
    public Sprite filledStar;
    public Sprite emptyStar;

    // compare gameData with the Dish.recipe id to see if it is a match
    // if it is then 3 stars 
    // if not 1 stars

    public void SubmitDish()
    {
        int stars = CalculateRating();
        Debug.Log("Rating: " + stars + " Stars!");

        // Display visual feedback
        DisplayEvaluation(stars);

        // Transition back to CustomerScene after a short delay
        Invoke("TransitionToCustomerScene", 2f);
    }

    int CalculateRating()
    {
        // Get the current dish ID from GameData
        int expectedDishId = GameData.currentDishId;

        if (enableDebugLogs)
            Debug.Log($"[RatingSystem] Expected Dish ID: {expectedDishId}");

        // Find the dish that was created on the plate
        GameObject createdDish = FindDishOnPlate();

        if (createdDish == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("[RatingSystem] No dish found on plate!");
            return 1; // No dish = 1 star
        }

        // Get the Dish component and its recipe
        Dish dishComponent = createdDish.GetComponent<Dish>();
        if (dishComponent == null || dishComponent.recipe == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("[RatingSystem] Dish has no Recipe component!");
            return 1; // No recipe = 1 star
        }

        // Compare IDs
        string dishRecipeId = dishComponent.recipe.ID;

        if (enableDebugLogs)
            Debug.Log($"[RatingSystem] Created Dish ID: {dishRecipeId}, Expected: {expectedDishId}");

        // Check if the dish ID matches the expected ID
        // Convert expected ID to string for comparison (assuming Recipe.ID is string)
        if (dishRecipeId == expectedDishId.ToString())
        {
            if (enableDebugLogs)
                Debug.Log("[RatingSystem] Perfect match! 3 stars!");
            return 3; // Perfect match = 3 stars
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("[RatingSystem] Wrong dish! 1 star.");
            return 1; // Wrong dish = 1 star
        }
    }

    GameObject FindDishOnPlate()
    {
        if (plateTransform == null)
        {
            // Try to find the plate if not assigned
            CombinationSystem combSystem = FindObjectOfType<CombinationSystem>();
            if (combSystem != null)
                plateTransform = combSystem.plate;
        }

        if (plateTransform == null)
        {
            Debug.LogError("[RatingSystem] No plate transform found!");
            return null;
        }

        // Look for a dish (object with Dish component) on the plate
        foreach (Transform child in plateTransform)
        {
            Dish dish = child.GetComponent<Dish>();
            if (dish != null)
            {
                if (enableDebugLogs)
                    Debug.Log($"[RatingSystem] Found dish: {child.name}");
                return child.gameObject;
            }
        }

        return null;
    }

    void TransitionToCustomerScene()
    {
        if (enableDebugLogs)
            Debug.Log("[RatingSystem] Transitioning to CustomerScene...");

        SceneManager.LoadScene("CustomerScene");
    }

    // void DisplayStars(int stars)
    // {
    //     string starDisplay = "";
    //     for (int i = 0; i < stars; i++)
    //     {
    //         starDisplay += "★ ";
    //     }
    //     Debug.Log(starDisplay);
    // }
    private void DisplayEvaluation(int stars)
    {
        // 1. Update text based on the score
        if (stars == 3)
        {
            evaluationResults.text = "Order Complete! Perfect Dish!";
        }
        else if (stars == 2)
        {
            evaluationResults.text = "Almost! Good Effort!";
        }
        else
        {
            evaluationResults.text = "Incorrect Dish! Try Again.";
        }

        // 2. Update the star images
        for (int i = 0; i < starDisplay.Length; i++)
        {
            if (i < stars)
            {
                starDisplay[i].sprite = filledStar; // Show filled stars
            }
            else
            {
                starDisplay[i].sprite = emptyStar; // Show empty stars
            }
        }

        // 3. Show the panel
        evaluationPanel.SetActive(true);
    }

}