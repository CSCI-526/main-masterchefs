//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using System.Linq;
//using TMPro;

//public class RatingSystem : MonoBehaviour
//{
//    [Header("Rating Settings")]
//    public Transform plateTransform;
//    public bool enableDebugLogs = true;
//    public GameObject evaluationPanel;
//    public TMPro.TextMeshProUGUI evaluationResults;
//    public Image[] starDisplay = new Image[3];
//    public Sprite filledStar;
//    public Sprite emptyStar;
//    public TMPro.TextMeshProUGUI averageTimeText;
//    public TMPro.TextMeshProUGUI averageScoreText;

//    // Tracking variables
//    private static List<float> completionTimes = new List<float>();
//    private static List<int> starRatings = new List<int>();

//    public void SubmitDish()
//    {
//        int stars = CalculateRating();
//        Debug.Log("Rating: " + stars + " Stars!");

//        float timeTaken = Timer.Instance.StopTimer();
//        completionTimes.Add(timeTaken);
//        starRatings.Add(stars);

//        DisplayEvaluation(stars);

//        // Only progress if player got 3 stars
//        if (stars == 3)
//        {
//            GameData.currentDishId++;
//            GameData.CheckAndIncrementLevel();
//            if (enableDebugLogs)
//                Debug.Log($"[RatingSystem] Perfect! Moving to next dish. Current dish ID: {GameData.currentDishId}");

//            // Transition to Customer Scene after 2 seconds
//            Invoke("TransitionToCustomerScene", 2f);
//        }
//        else
//        {
//            if (enableDebugLogs)
//                Debug.Log($"[RatingSystem] Less than 3 stars. Retry same dish. Current dish ID: {GameData.currentDishId}");

//            // Close evaluation panel after 2 seconds, stay in Kitchen Scene
//            Invoke("CloseEvaluationPanel", 2f);
//        }
//    }

//    void CloseEvaluationPanel()
//    {
//        evaluationPanel.SetActive(false);

//        // Optional: Clear the plate so player can start fresh
//        ClearPlate();

//        if (enableDebugLogs)
//            Debug.Log("[RatingSystem] Evaluation panel closed. Try again!");
//    }

//    void ClearPlate()
//    {
//        // Clear all ingredients from the plate
//        if (plateTransform == null)
//        {
//            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
//            if (combSystem != null)
//                plateTransform = combSystem.plate;
//        }

//        if (plateTransform != null)
//        {
//            // Destroy all children (ingredients and dishes on the plate)
//            foreach (Transform child in plateTransform)
//            {
//                Destroy(child.gameObject);
//            }

//            if (enableDebugLogs)
//                Debug.Log("[RatingSystem] Plate cleared for retry!");
//        }
//    }

//    void UpdateAverageDisplays()
//    {
//        // Update the average time after every round
//        // Update the average score after every round
//        evaluationPanel.SetActive(false);
//    }

//    int CalculateRating()
//    {
//        int expectedDishId = GameData.currentDishId;

//        if (enableDebugLogs)
//            Debug.Log($"[RatingSystem] Expected Dish ID: {expectedDishId}");

//        Recipe expectedRecipe = GetExpectedRecipe(expectedDishId);
//        if (expectedRecipe == null)
//        {
//            if (enableDebugLogs)
//                Debug.LogError($"[RatingSystem] Could not find recipe for dish ID {expectedDishId}");
//            return 0;
//        }

//        GameObject completedDish = FindDishOnPlate();
//        if (completedDish != null)
//        {
//            Dish dishComponent = completedDish.GetComponent<Dish>();
//            if (dishComponent != null && dishComponent.recipe != null)
//            {
//                if (dishComponent.recipe.ID == expectedDishId.ToString())
//                {
//                    if (enableDebugLogs)
//                        Debug.Log($"[RatingSystem] Perfect dish match! {dishComponent.recipe.dishName}");
//                    return 3;
//                }
//                else
//                {
//                    if (enableDebugLogs)
//                        Debug.Log($"[RatingSystem] Wrong dish! Got {dishComponent.recipe.dishName}, expected recipe ID {expectedDishId}");
//                    return 1;
//                }
//            }
//        }

//        List<Ingredient> ingredientsOnPlate = GetIngredientsOnPlate();

//        if (ingredientsOnPlate.Count == 0)
//        {
//            if (enableDebugLogs)
//                Debug.LogWarning("[RatingSystem] No ingredients or dish found on plate!");
//            return 0;
//        }

//        int correctIngredients = 0;
//        int totalIngredientsUsed = ingredientsOnPlate.Count;
//        int overcookedCount = 0;
//        HashSet<Ingredient> matchedIngredients = new HashSet<Ingredient>();

//        foreach (RecipeIngredientRequirement requirement in expectedRecipe.requiredIngredients)
//        {
//            foreach (Ingredient ingredient in ingredientsOnPlate)
//            {
//                if (matchedIngredients.Contains(ingredient))
//                    continue;

//                if (ingredient.ingredientData == requirement.ingredient &&
//                    ingredient.currentState == requirement.requiredState &&
//                    (requirement.requiredCookware == CookwareType.None || ingredient.currentCookware == requirement.requiredCookware))
//                {
//                    correctIngredients++;
//                    matchedIngredients.Add(ingredient);
//                    if (enableDebugLogs)
//                        Debug.Log($"[RatingSystem] Matched ingredient: {ingredient.ingredientData.ingredientName} ({ingredient.currentState})");
//                    break;
//                }
//            }
//        }

//        foreach (Ingredient ingredient in ingredientsOnPlate)
//        {
//            if (ingredient.currentState == IngredientState.Overcooked && !matchedIngredients.Contains(ingredient))
//            {
//                overcookedCount++;
//            }
//        }

//        float correctPercentage = (float)correctIngredients / expectedRecipe.requiredIngredients.Count * 100f;
//        float overcookedPenalty = overcookedCount * 10f;
//        float finalPercentage = correctPercentage - overcookedPenalty;

//        if (enableDebugLogs)
//        {
//            Debug.Log($"[RatingSystem] Correct Ingredients: {correctIngredients}/{expectedRecipe.requiredIngredients.Count}");
//            Debug.Log($"[RatingSystem] Total Used: {totalIngredientsUsed}");
//            Debug.Log($"[RatingSystem] Overcooked: {overcookedCount}");
//            Debug.Log($"[RatingSystem] Correct %: {correctPercentage:F1}%");
//            Debug.Log($"[RatingSystem] Penalty: {overcookedPenalty:F1}%");
//            Debug.Log($"[RatingSystem] Final %: {finalPercentage:F1}%");
//        }

//        int stars = 0;
//        if (finalPercentage > 75f)
//            stars = 3;
//        else if (finalPercentage > 50f)
//            stars = 2;
//        else if (finalPercentage > 25f)
//            stars = 1;
//        else
//            stars = 0;

//        if (enableDebugLogs)
//            Debug.Log($"[RatingSystem] Rating: {stars} stars!");

//        return stars;
//    }

//    Recipe GetExpectedRecipe(int dishId)
//    {
//        CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
//        if (combSystem != null && combSystem.allRecipes != null)
//        {
//            foreach (Recipe recipe in combSystem.allRecipes)
//            {
//                if (recipe.ID == dishId.ToString())
//                {
//                    return recipe;
//                }
//            }
//        }

//        if (enableDebugLogs)
//            Debug.LogWarning($"[RatingSystem] Recipe with ID {dishId} not found in CombinationSystem");
//        return null;
//    }

//    List<Ingredient> GetIngredientsOnPlate()
//    {
//        List<Ingredient> ingredients = new List<Ingredient>();

//        if (plateTransform == null)
//        {
//            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
//            if (combSystem != null)
//                plateTransform = combSystem.plate;
//        }

//        if (plateTransform == null)
//        {
//            Debug.LogError("[RatingSystem] No plate transform found!");
//            return ingredients;
//        }

//        foreach (Transform child in plateTransform)
//        {
//            Dish dish = child.GetComponent<Dish>();
//            if (dish != null)
//                continue;

//            Ingredient ingredient = child.GetComponent<Ingredient>();
//            if (ingredient != null)
//            {
//                ingredients.Add(ingredient);
//                if (enableDebugLogs)
//                    Debug.Log($"[RatingSystem] Found ingredient: {ingredient.ingredientData.ingredientName} ({ingredient.currentState})");
//            }
//        }

//        return ingredients;
//    }

//    GameObject FindDishOnPlate()
//    {
//        if (plateTransform == null)
//        {
//            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
//            if (combSystem != null)
//                plateTransform = combSystem.plate;
//        }

//        if (plateTransform == null)
//        {
//            Debug.LogError("[RatingSystem] No plate transform found!");
//            return null;
//        }

//        foreach (Transform child in plateTransform)
//        {
//            Dish dish = child.GetComponent<Dish>();
//            if (dish != null)
//            {
//                if (enableDebugLogs)
//                    Debug.Log($"[RatingSystem] Found dish: {child.name}");
//                return child.gameObject;
//            }
//        }

//        return null;
//    }

//    void TransitionToCustomerScene()
//    {
//        if (enableDebugLogs)
//            Debug.Log("[RatingSystem] Transitioning to CustomerScene...");

//        SceneManager.LoadScene("CustomerScene");
//    }

//    private void DisplayEvaluation(int stars)
//    {
//        if (stars == 3)
//        {
//            evaluationResults.text = "Order Complete! Perfect Dish!";
//        }
//        else if (stars == 2)
//        {
//            evaluationResults.text = "Almost! Good Effort! Try Again.";
//        }
//        else if (stars == 1)
//        {
//            evaluationResults.text = "Not Quite Right. Try Again!";
//        }
//        else
//        {
//            evaluationResults.text = "Incorrect Dish! Try Again.";
//        }

//        for (int i = 0; i < starDisplay.Length; i++)
//        {
//            if (i < stars)
//            {
//                starDisplay[i].sprite = filledStar;
//            }
//            else
//            {
//                starDisplay[i].sprite = emptyStar;
//            }
//        }

//        evaluationPanel.SetActive(true);
//    }
//}













using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RatingSystem : MonoBehaviour
{
    [Header("Rating Settings")]
    public Transform plateTransform;
    public bool enableDebugLogs = true;
    public GameObject evaluationPanel;
    public TMPro.TextMeshProUGUI evaluationResults;
    public Image[] starDisplay = new Image[3];
    public Sprite filledStar;
    public Sprite emptyStar;
    public TMPro.TextMeshProUGUI averageTimeText;
    public TMPro.TextMeshProUGUI averageScoreText;

    // Tracking variables
    private static List<float> completionTimes = new List<float>();
    private static List<int> starRatings = new List<int>();

    // Attempt tracking
    private int submissionAttempts = 0;
    private const int MAX_ATTEMPTS = 3;

    public void SubmitDish()
    {
        int stars = CalculateRating();
        Debug.Log("Rating: " + stars + " Stars!");

        float timeTaken = Timer.Instance.StopTimer();
        completionTimes.Add(timeTaken);
        starRatings.Add(stars);

        DisplayEvaluation(stars);

        // Increment attempt counter
        submissionAttempts++;

        // Only progress if player got 3 stars
        if (stars == 3)
        {
            GameData.currentDishId++;
            GameData.CheckAndIncrementLevel();
            if (enableDebugLogs)
                Debug.Log($"[RatingSystem] Perfect! Moving to next dish. Current dish ID: {GameData.currentDishId}");

            // Reset attempts for next dish
            submissionAttempts = 0;

            // Transition to Customer Scene after 2 seconds
            Invoke("TransitionToCustomerScene", 2f);
        }
        else if (submissionAttempts >= MAX_ATTEMPTS)
        {
            // After 3 attempts, move to next scene even with low rating
            if (enableDebugLogs)
                Debug.Log($"[RatingSystem] Max attempts reached ({MAX_ATTEMPTS}). Moving to next scene anyway.");

            GameData.currentDishId++;
            GameData.CheckAndIncrementLevel();

            // Reset attempts for next dish
            submissionAttempts = 0;

            // Transition to Customer Scene after 2 seconds
            Invoke("TransitionToCustomerScene", 2f);
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log($"[RatingSystem] Less than 3 stars. Attempt {submissionAttempts}/{MAX_ATTEMPTS}. Retry same dish. Current dish ID: {GameData.currentDishId}");

            // Close evaluation panel after 2 seconds, stay in Kitchen Scene
            Invoke("CloseEvaluationPanel", 2f);
        }
    }

    void CloseEvaluationPanel()
    {
        evaluationPanel.SetActive(false);

        // Optional: Clear the plate so player can start fresh
        ClearPlate();

        if (enableDebugLogs)
            Debug.Log("[RatingSystem] Evaluation panel closed. Try again!");
    }

    void ClearPlate()
    {
        // Clear all ingredients from the plate
        if (plateTransform == null)
        {
            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
            if (combSystem != null)
                plateTransform = combSystem.plate;
        }

        if (plateTransform != null)
        {
            // Destroy all children (ingredients and dishes on the plate)
            foreach (Transform child in plateTransform)
            {
                Destroy(child.gameObject);
            }

            if (enableDebugLogs)
                Debug.Log("[RatingSystem] Plate cleared for retry!");
        }
    }

    void UpdateAverageDisplays()
    {
        // Update the average time after every round
        // Update the average score after every round
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

        Recipe expectedRecipe = GetExpectedRecipe(expectedDishId);
        if (expectedRecipe == null)
        {
            if (enableDebugLogs)
                Debug.LogError($"[RatingSystem] Could not find recipe for dish ID {expectedDishId}");
            return 0;
        }

        GameObject completedDish = FindDishOnPlate();
        if (completedDish != null)
        {
            Dish dishComponent = completedDish.GetComponent<Dish>();
            if (dishComponent != null && dishComponent.recipe != null)
            {
                if (dishComponent.recipe.ID == expectedDishId.ToString())
                {
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] Perfect dish match! {dishComponent.recipe.dishName}");
                    return 3;
                }
                else
                {
                    if (enableDebugLogs)
                        Debug.Log($"[RatingSystem] Wrong dish! Got {dishComponent.recipe.dishName}, expected recipe ID {expectedDishId}");
                    return 1;
                }
            }
        }

        List<Ingredient> ingredientsOnPlate = GetIngredientsOnPlate();

        if (ingredientsOnPlate.Count == 0)
        {
            if (enableDebugLogs)
                Debug.LogWarning("[RatingSystem] No ingredients or dish found on plate!");
            return 0;
        }

        int correctIngredients = 0;
        int totalIngredientsUsed = ingredientsOnPlate.Count;
        int overcookedCount = 0;
        HashSet<Ingredient> matchedIngredients = new HashSet<Ingredient>();

        foreach (RecipeIngredientRequirement requirement in expectedRecipe.requiredIngredients)
        {
            foreach (Ingredient ingredient in ingredientsOnPlate)
            {
                if (matchedIngredients.Contains(ingredient))
                    continue;

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

        foreach (Ingredient ingredient in ingredientsOnPlate)
        {
            if (ingredient.currentState == IngredientState.Overcooked && !matchedIngredients.Contains(ingredient))
            {
                overcookedCount++;
            }
        }

        float correctPercentage = (float)correctIngredients / expectedRecipe.requiredIngredients.Count * 100f;
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

        foreach (Transform child in plateTransform)
        {
            Dish dish = child.GetComponent<Dish>();
            if (dish != null)
                continue;

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
            CombinationSystem combSystem = FindFirstObjectByType<CombinationSystem>();
            if (combSystem != null)
                plateTransform = combSystem.plate;
        }

        if (plateTransform == null)
        {
            Debug.LogError("[RatingSystem] No plate transform found!");
            return null;
        }

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

    private void DisplayEvaluation(int stars)
    {
        if (stars == 3)
        {
            evaluationResults.text = "Order Complete! Perfect Dish!";
        }
        else if (stars == 2)
        {
            evaluationResults.text = "Almost! Good Effort! Try Again.";
        }
        else if (stars == 1)
        {
            evaluationResults.text = "Not Quite Right. Try Again!";
        }
        else
        {
            evaluationResults.text = "Incorrect Dish! Try Again.";
        }

        for (int i = 0; i < starDisplay.Length; i++)
        {
            if (i < stars)
            {
                starDisplay[i].sprite = filledStar;
            }
            else
            {
                starDisplay[i].sprite = emptyStar;
            }
        }

        evaluationPanel.SetActive(true);
    }
}