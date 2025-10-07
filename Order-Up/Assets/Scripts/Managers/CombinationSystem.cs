using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CombinationSystem : MonoBehaviour
{
    public Transform plate; // Reference to the Plate/Dish GameObject
    public List<Recipe> allRecipes = new List<Recipe>(); // Assign in Inspector
    public bool enableDebugLogs = true;

    // Call this whenever ingredients are added to the plate
    /**
    * Combination system should allow players to select which ingredient to combine.
    * For simplicity, this example checks all ingredients on the plate and tries to match any recipe.
    */
    public void CheckForCombinations()
    {
        // Get all ingredient children on the plate
        List<GameObject> ingredientObjects = new List<GameObject>();
        List<Ingredient> ingredients = new List<Ingredient>();

        foreach (Transform child in plate)
        {
            Ingredient ing = child.GetComponent<Ingredient>();
            if (ing != null)
            {
                ingredientObjects.Add(child.gameObject);
                ingredients.Add(ing);
            }
        }

        // Need at least 2 ingredients to combine
        if (ingredients.Count < 2)
            return;

        // Check all recipes to find a match
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                if (enableDebugLogs)
                    Debug.Log("Recipe found: " + recipe.dishName);
                CombineIntoDish(ingredientObjects, recipe);
                Debug.Log($"{GameData.currentDishId} is the current dish id");
                return; // Stop after first match
            }
        }

        Debug.Log("No matching recipe found for these ingredients");
    }

    void CombineIntoDish(List<GameObject> ingredientsToRemove, Recipe recipe)
    {
        // Remove all ingredient GameObjects
        foreach (GameObject ing in ingredientsToRemove)
        {
            Destroy(ing);
        }

        // Spawn the combined dish
        if (recipe.dishPrefab != null)
        {
            GameObject dish = Instantiate(recipe.dishPrefab, plate.position, Quaternion.identity);
            dish.transform.SetParent(plate);
            dish.transform.localPosition = Vector3.zero;
            if (enableDebugLogs)
                Debug.Log("Created dish: " + recipe.dishName);
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning("Recipe " + recipe.dishName + " has no dish prefab assigned!");
        }
    }

    // Alternative: Check for specific number of ingredients
    public void CheckForCombinationsWithCount(int minIngredients)
    {
        List<GameObject> ingredientObjects = new List<GameObject>();
        List<Ingredient> ingredients = new List<Ingredient>();

        foreach (Transform child in plate)
        {
            Ingredient ing = child.GetComponent<Ingredient>();
            if (ing != null)
            {
                ingredientObjects.Add(child.gameObject);
                ingredients.Add(ing);
            }
        }

        if (ingredients.Count < minIngredients)
            return;

        // Try to find exact matches first
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                CombineIntoDish(ingredientObjects, recipe);
                return;
            }
        }
    }
}
