using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

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

    private static Dictionary<string, string> failureReasons = new Dictionary<string, string>();


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
        submitButton.interactable = false; // Disable the button to prevent multiple submissions
        int stars = CalculateRating();
        if (enableDebugLogs)
            Debug.Log($"[RatingSystem] Dish submitted. Calculated Rating: {stars} stars.");
        float timeTaken = Timer.Instance.StopTimer();
        completionTimes.Add(timeTaken);
        starRatings.Add(stars);
        RevenueSystem.Instance.AddRevenue(stars);

        // Record the attempt
        if (Attempts.Instance != null)
        {
            Attempts.Instance.RecordAttempt(stars);
        }

        DisplayEvaluation(stars);
        

        
        bool hasAttemptsLeft = Attempts.Instance.HasAttemptsRemaining();
        bool isPerfectScore = (stars == 3);
        // if player did not get 3 stars send failure analytics data 
        if (!isPerfectScore)
        {
            List<string> reasons = failureReasons.Keys.ToList();
            string failureReasonsString = string.Join(", ", reasons);
            failureReasons.TryGetValue("Missing Ingredients", out string missingIngredients);
            failureReasons.TryGetValue("Wrong Ingredients", out string wrongIngredients);
            failureReasons.TryGetValue("Overcooked Ingredients", out string overcookedIngredients);
            failureReasons.TryGetValue("Raw Ingredients", out string rawIngredients);
            failureReasons.TryGetValue("Wrong Cookware", out string wrongCookware);

            sendFailureData(Attempts.Instance.GetCurrentAttempt(), GameData.CurrentDishId, failureReasonsString,missingIngredients ?? "", wrongIngredients ?? "", overcookedIngredients ?? "", rawIngredients ?? "", wrongCookware?? "");
        }
       
        // clear failure reason list for next round
        failureReasons.Clear();
        
        
        // Player moves on if they have perfect score or out of attempts
        if (isPerfectScore || !hasAttemptsLeft)
        {
            if (enableDebugLogs)
                Debug.Log($"[RatingSystem] Round complete. PerfectScore: {isPerfectScore}, HasAttempts: {hasAttemptsLeft}");
     
            // Send level data to Analytics manager
            // sendTimeData(timeTaken);
            sendLevelCompleteData(isPerfectScore, timeTaken, Attempts.Instance.GetCurrentAttempt(), stars);
            
            //Attempts.Instance.CompleteLevel(); 
            
            Invoke("TransitionToNextRound", 2f);

        }
        // Player retries (Score < 3 AND has attempts left)
        else
        {
            if (enableDebugLogs)
                Debug.Log($"[RatingSystem] Imperfect score. Retrying round.");
            
            Invoke("PrepareForRetry", 2f); 
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
                    // log reason of failure
                    failureReasons.Add("wrong dish", dishComponent.recipe.dishName);
                    return 1; // Wrong dish = 1 star
                }
            }
        }

        // If no completed dish, get individual ingredients on the plate
        List<Ingredient> ingredientsOnPlate = GetIngredientsOnPlate();
        
        if (ingredientsOnPlate.Count == 0)
        {
            // log reason of failure
            failureReasons.Add("no ingredients", "true");
            if (enableDebugLogs)
                Debug.LogWarning("[RatingSystem] No ingredients or dish found on plate!");
            return 0;
        }

        // Calculate correct ingredients
        int correctIngredients = 0;
        int totalIngredientsUsed = ingredientsOnPlate.Count;
        int overcookedCount = 0;
        HashSet<Ingredient> matchedIngredients = new HashSet<Ingredient>();
        
        // record reasons of failure
        string missingIngredients = "";
        string wrongIngredients = "";
        string overcookedIngredients = "";
        string wrongCookware = "";
        string rawIngredients = "";
        HashSet<Ingredient> processedIngredients = new HashSet<Ingredient>(); // list to keep track of correct ingredient(regardless of state) we have seen so far
        
        foreach (RecipeIngredientRequirement requirement in expectedRecipe.requiredIngredients)
        {
            bool perfectMatchFound = false; // perfect match flag
            Ingredient bestPartialMatch = null; // Stores an ingredient that matches name but not state
            foreach (Ingredient ingredient in ingredientsOnPlate)
            {
                if (processedIngredients.Contains(ingredient))
                    continue; // Already processed this ingredient
                string plateIngredient = ingredient.ingredientData.ingredientName.Split('_')[0];
                string requiredIngredient = requirement.ingredient.ingredientName.Split('_')[0];
                if (plateIngredient == requiredIngredient)
                {
                    bool isStateCorrect = (ingredient.currentState == requirement.requiredState);
                    bool isCookwareCorrect = (requirement.requiredCookware == CookwareType.None || ingredient.currentCookware == requirement.requiredCookware);
                    if (isStateCorrect && isCookwareCorrect)
                    {
                        // perfect match
                        correctIngredients++;
                        matchedIngredients.Add(ingredient);
                        processedIngredients.Add(ingredient);
                        perfectMatchFound = true;
                        if (enableDebugLogs)
                            Debug.Log($"[RatingSystem] Matched ingredient: {ingredient.ingredientData.ingredientName} ({ingredient.currentState})");
                        break;
                    }
                    else
                    {
                        if (bestPartialMatch == null)
                        { 
                            bestPartialMatch = ingredient;     
                        }

                    }
        
                }
                
            }
            //
            if (!perfectMatchFound)
            {
                if (bestPartialMatch != null)
                {
                    // found the ingredient, but it was in the wrong state.
                    processedIngredients.Add(bestPartialMatch);
                    bool cookwareMismatch = (requirement.requiredCookware != CookwareType.None && 
                                             bestPartialMatch.currentCookware != requirement.requiredCookware);
                    if (cookwareMismatch)
                    {
                        wrongCookware += name + "/";
                    }
                    else if (bestPartialMatch.currentState == IngredientState.Raw)
                    {
                        // it's raw
                        rawIngredients += bestPartialMatch.ingredientData.ingredientName + "/";
                    }
                    else if (bestPartialMatch.currentState == IngredientState.Overcooked)
                    {
                        // it's overcooked
                        overcookedIngredients += bestPartialMatch.ingredientData.ingredientName + "/";
                    }
                    else
                    {
                        // not sure why the ingredient is not matching
                        Debug.Log("Correct ingredient but not matching, please check");
                    }
                }
                else
                {
                    // no match for ingredient is found, this ingredient is missing
                    missingIngredients += requirement.ingredient.ingredientName + "/";
                } 
            }
        }
        
        // Record wrong ingredients
        foreach (Ingredient ingredient in ingredientsOnPlate)
        {
            // check if the ingredient was used for either a perfect match or a partial match error
            if (!processedIngredients.Contains(ingredient))
            {
                wrongIngredients += ingredient.ingredientData.ingredientName + "/";
            }
            // Count overcooked ingredients (without double counting)
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
        
        // populate failure reasons list
        if (!string.IsNullOrEmpty(missingIngredients))
        {
            failureReasons.Add("Missing Ingredients", missingIngredients);
        }

        if (!string.IsNullOrEmpty(wrongIngredients))
        {
            failureReasons.Add("Wrong Ingredients", wrongIngredients);
        }

        if (!string.IsNullOrEmpty(overcookedIngredients))
        {
            failureReasons.Add("Overcooked Ingredients", overcookedIngredients);
        }

        if (!string.IsNullOrEmpty(rawIngredients))
        {
            failureReasons.Add("Raw Ingredients", rawIngredients);
        }

        if (!string.IsNullOrEmpty(wrongCookware))
        {
            failureReasons.Add("Wrong Cookware", wrongCookware);
        }

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
            Debug.Log($"[RatingSystem] Current Level: {currentLevel}, Current Round: {currentRound}");

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
    
    private void PrepareForRetry()
    {
        // 1. Hide the panel
        HideEvaluationPanel();

        // 2. Tell the Attempts script to reset the level
        if (Attempts.Instance != null)
        {
            Attempts.Instance.PrepareNextAttempt();
        }
    }
    public void EnableSubmitButton()
    {
        if (submitButton != null)
        {
            submitButton.interactable = true;
        }
    }
    
    // private void sendTimeData(float total)
    // {
    //     // Send current level and time spent to google form
    //     long sessionID = GameManager.Instance.SessionID;
    //     int level = GameData.CurrentLevel;
    //     int round = GameData.CurrentRound;
    //     
    //     Debug.Log($"[RatingSystem] Game Session ID: {sessionID}, Level: {level}, Round: {round}, Time(s): {total}");
    //     
    //     AnalyticsManager.Instance.SendLevelTimeData(sessionID, level, round, total);
    // }

    private void sendLevelCompleteData(bool pass, float time, int attempts, int rating)
    {
        long sessionID = GameManager.Instance.SessionID;
        int level = GameData.CurrentLevel;
        int round = GameData.CurrentRound;
        string status = pass ? "pass" : "fail";
        
        Debug.Log($"[RatingSystem] Send level complete data...Game Session ID: {sessionID}, Level: {level}. Round: {round}, Status: {status}, Attempts: {attempts}, Rating: {rating}");
        
        AnalyticsManager.Instance.SendLevelComplete(sessionID, level, round, status, time, attempts, rating);
    }
    private void sendFailureData(int attempt, int dishID, string reason, string missingIngredients, string wrongIngredients, string overcookedIngredients, string rawIngredients, string wrongCookware)
    {
        long sessionID = GameManager.Instance.SessionID;
        int level = GameData.CurrentLevel;
        int round = GameData.CurrentRound;
        string dishName = DishNameLibrary.GetName(dishID);
        
        Debug.Log($"[RatingSystem] Sending failure data...Game Session ID: {sessionID}, Level: {level}. Round: {round}, Attempt: {attempt}, Dish: {dishName}, Reason: {reason}");
        
        AnalyticsManager.Instance.SendFailureData(sessionID, level, round, attempt, dishName, reason, missingIngredients, wrongIngredients, overcookedIngredients, rawIngredients, wrongCookware);
    }
    

}