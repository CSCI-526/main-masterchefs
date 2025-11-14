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
    
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;

    [Header("UI References")]
    public GameObject evaluationPanel;
    public TMPro.TextMeshProUGUI evaluationResults;
    public Image[] starDisplay = new Image[3];
    public Sprite filledStar;
    public Sprite emptyStar;
    [SerializeField] private Button submitButton;

    // Tracking variables
    private static List<float> completionTimes = new List<float>();
    private static List<int> starRatings = new List<int>();


    /// <summary>
    /// Submits the current dish for evaluation, calculates its rating, and handles the subsequent game flow based on
    /// the result.
    /// </summary>
    /// <remarks>This method evaluates the dish by calculating its star rating and recording the time taken to
    /// complete the task. The results are stored for future reference, and the player's attempt is recorded if
    /// applicable. Depending on the remaining attempts, the method either transitions to the next scene or allows the
    /// player to retry.</remarks>
    public void SubmitDish()
    {
        submitButton.interactable = false; // Prevent multiple submissions
        int stars = CalculateRating();
        Debug.Log("Rating: " + stars + " Stars!");

        float timeTaken = Timer.Instance.StopTimer();
        completionTimes.Add(timeTaken);
        starRatings.Add(stars);

        // Record the attempt
        if (Attempts.Instance != null)
        {
            Attempts.Instance.RecordAttempt(stars);
        }

        DisplayEvaluation(stars);

        // Only transition if there are no attempts remaining
        if (Attempts.Instance != null && !Attempts.Instance.HasAttemptsRemaining())
        {
            // All attempts used for this round - move to next round or level
            Invoke("TransitionToNextRound", 2f);
        }
        else
        {
            Invoke("HideEvaluationPanel", 2f);
            submitButton.interactable = true; // Re-enable the button for retry
        }
    }

    void UpdateAverageDisplays()
    {
        // TODO: Update the average time after every round
        // TODO: Update the average score after every round
        evaluationPanel.SetActive(false);
    }

    int CalculateRating()
    {
        /**
        * 0 stars - no correct ingredient on plate
        * percentages of correct ingredients = number of correct ingredients / total ingredients used
        * 0 star - 0-25% correct ingredients
        * 1 star - 26-50% correct ingredients
        * 2 stars - 51-75% correct ingredients
        * 3 stars - 76-100% correct ingredients
        * -10% deduction for every overcooked ingredient used
        * no double deduction
        */
        
        // Get the expected dish ID from GameData
        int expectedDishId = GameData.CurrentDishId;

        if (enableDebugLogs)
            Debug.Log($"[RatingSystem] Expected Dish ID: {expectedDishId}");

        // Get the expected recipe
        Recipe expectedRecipe = GetExpectedRecipe(expectedDishId);
        if (expectedRecipe == null)
        {
            if (enableDebugLogs)
                Debug.LogError($"[RatingSystem] Could not find recipe for dish ID {expectedDishId}");
            return 0;
        }

        // Check if there's a completed dish on the plate first
        GameObject completedDish = FindDishOnPlate();
        if (completedDish != null)
        {
            Dish dishComponent = completedDish.GetComponent<Dish>();
            if (dishComponent != null && dishComponent.recipe != null)
            {
                // Compare the dish recipe ID with expected recipe ID
                if (dishComponent.recipe.ID == expectedDishId.ToString())
                {
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] Perfect dish match! {dishComponent.recipe.dishName}");
                    return 3; // Perfect match = 3 stars
                }
                else
                {
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] Wrong dish! Got {dishComponent.recipe.dishName}, expected recipe ID {expectedDishId}");
                    return 1; // Wrong dish = 1 star
                }
            }
        }

        // If no completed dish, get individual ingredients on the plate
        List<Ingredient> ingredientsOnPlate = GetIngredientsOnPlate();
        
        if (ingredientsOnPlate.Count == 0)
        {
            if (enableDebugLogs)
                Debug.LogWarning("[RatingSystem] No ingredients or dish found on plate!");
            return 0;
        }

        // Calculate correct ingredients
        int correctIngredients = 0;
        int totalIngredientsUsed = ingredientsOnPlate.Count;
        int overcookedCount = 0;
        HashSet<Ingredient> matchedIngredients = new HashSet<Ingredient>();

        foreach (RecipeIngredientRequirement requirement in expectedRecipe.requiredIngredients)
        {
            foreach (Ingredient ingredient in ingredientsOnPlate)
            {
                if (matchedIngredients.Contains(ingredient))
                    continue; // Already matched this ingredient

                if (ingredient.ingredientData == requirement.ingredient && 
                    ingredient.currentState == requirement.requiredState &&
                    (requirement.requiredCookware == CookwareType.None || ingredient.currentCookware == requirement.requiredCookware))
                {
                    correctIngredients++;
                    matchedIngredients.Add(ingredient);
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] Matched ingredient: {ingredient.ingredientData.ingredientName} ({ingredient.currentState})");
                    break;
                }
            }
        }

        // Count overcooked ingredients (without double counting)
        foreach (Ingredient ingredient in ingredientsOnPlate)
        {
            if (ingredient.currentState == IngredientState.Overcooked && !matchedIngredients.Contains(ingredient))
            {
                overcookedCount++;
            }
        }

        // Calculate percentage of correct ingredients
        float correctPercentage = (float)correctIngredients / expectedRecipe.requiredIngredients.Count * 100f;
        
        // Apply overcooked penalty (10% per overcooked ingredient)
        float overcookedPenalty = overcookedCount * 10f;
        float finalPercentage = correctPercentage - overcookedPenalty;

        if (enableDebugLogs)
        {
            Debug.Log($"[RatingSystem] Correct Ingredients: {correctIngredients}/{expectedRecipe.requiredIngredients.Count}");
            Debug.Log($"[RatingSystem] Total Used: {totalIngredientsUsed}");
            Debug.Log($"[RatingSystem] Overcooked: {overcookedCount}");
            Debug.Log($"[RatingSystem] Correct %: {correctPercentage:F1}%");
            Debug.Log($"[RatingSystem] Penalty: {overcookedPenalty:F1}%");
            Debug.Log($"[RatingSystem] Final %: {finalPercentage:F1}%");
        }

        // Determine star rating
        int stars = 0;
        if (finalPercentage > 75f)
            stars = 3;
        else if (finalPercentage > 50f)
            stars = 2;
        else if (finalPercentage > 25f)
            stars = 1;
        else
            stars = 0;

        if (enableDebugLogs)
            Debug.Log($"[RatingSystem] Rating: {stars} stars!");

        return stars;
    }

    Recipe GetExpectedRecipe(int dishId)
    {
        // Try to find CombinationSystem to access all recipes
        CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
        if (combSystem != null && combSystem.allRecipes != null)
        {
            foreach (Recipe recipe in combSystem.allRecipes)
            {
                if (recipe.ID == dishId.ToString())
                {
                    return recipe;
                }
            }
        }

        if (enableDebugLogs)
            Debug.LogWarning($"[RatingSystem] Recipe with ID {dishId} not found in CombinationSystem");
        return null;
    }

    List<Ingredient> GetIngredientsOnPlate()
    {
        List<Ingredient> ingredients = new List<Ingredient>();

        if (plateTransform == null)
        {
            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
            if (combSystem != null)
                plateTransform = combSystem.plate;
        }

        if (plateTransform == null)
        {
            Debug.LogError("[RatingSystem] No plate transform found!");
            return ingredients;
        }

        // Look for individual ingredients (not completed dishes)
        foreach (Transform child in plateTransform)
        {
            // Skip if it's a completed dish
            Dish dish = child.GetComponent<Dish>();
            if (dish != null)
                continue;

            // Look for individual ingredients
            Ingredient ingredient = child.GetComponent<Ingredient>();
            if (ingredient != null)
            {
                ingredients.Add(ingredient);
                if (enableDebugLogs)
                    Debug.Log($"[RatingSystem] Found ingredient: {ingredient.ingredientData.ingredientName} ({ingredient.currentState})");
            }
        }

        return ingredients;
    }

    GameObject FindDishOnPlate()
    {
        if (plateTransform == null)
        {
            // Try to find the plate if not assigned
            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
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

    void TransitionToNextRound()
    {
        if (enableDebugLogs)
            Debug.Log("[RatingSystem] All attempts used. Checking if more rounds remain...");

        // Reset attempts for next round
        if (Attempts.Instance != null)
        {
            Attempts.Instance.ResetAttempts();
        }

        // Check if there are more recipes in the current level
        LevelDesignManager levelDesignManager = FindFirstObjectByType<LevelDesignManager>();
        if (levelDesignManager != null && levelDesignManager.levels != null)
        {
            int currentLevel = GameData.CurrentLevel;
            int currentRound = GameData.CurrentRound;

            if (currentLevel > 0 && currentLevel <= levelDesignManager.levels.Count)
            {
                LevelData levelData = levelDesignManager.levels[currentLevel - 1];
                
                // Check if there are more recipes (rounds) in this level
                if (currentRound + 1 < levelData.recipes.Length)
                {
                    // More rounds remaining - increment round and go to CustomerScene
                    GameData.IncrementRound();
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] Moving to next round: {GameData.CurrentRound + 1} of {levelData.recipes.Length}");
                    
                    SceneManager.LoadScene("CustomerScene");
                    return;
                }
                else
                {
                    // All rounds complete - go to ReviewScene (next level)
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] All rounds complete for level {currentLevel}. Moving to ReviewScene.");
                    
                    SceneManager.LoadScene("ReviewScene");
                    return;
                }
            }
        }

        // Fallback: just go to CustomerScene if we can't determine next step
        if (enableDebugLogs)
            Debug.LogWarning("[RatingSystem] Could not determine next step. Defaulting to CustomerScene.");
        SceneManager.LoadScene("CustomerScene");
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

    private void HideEvaluationPanel()
    {
        if (evaluationPanel != null)
        {
            evaluationPanel.SetActive(false);
        }
    }

}