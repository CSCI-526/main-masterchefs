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

 

    void Start()
    {
        // load all recipes
        Recipe[] loadedRecipes = Resources.LoadAll<Recipe>("Recipes");
        allRecipes.AddRange(loadedRecipes);
    }

    public void CheckForCombinations()
    {
        // Get ALL Ingredient components inside the plate hierarchy
        Ingredient[] ingredientsArray = plate.GetComponentsInChildren<Ingredient>();

        List<GameObject> ingredientObjects = new List<GameObject>();
        List<Ingredient> ingredients = new List<Ingredient>();

        foreach (Ingredient ing in ingredientsArray)
        {
            ingredientObjects.Add(ing.gameObject);
            ingredients.Add(ing);
        }

        // Check all recipes to find a match
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                if (enableDebugLogs)
                    Debug.Log("Recipe found: " + recipe.dishName);

                CombineIntoDish(ingredientObjects, recipe);
                return;
            }
        }

        Debug.Log("No matching recipe found for these ingredients");
    }

    void CombineIntoDish(List<GameObject> ingredientsToRemove, Recipe recipe)
    {
        Plate plateScript = plate.GetComponent<Plate>();
        // Remove from plate list before destroy
        foreach (GameObject ing in ingredientsToRemove)
        {
            var drag = ing.GetComponent<DraggableIngredient>();
            if (drag != null)
                plateScript.RemoveIngredient(drag);
        }


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


            DraggableIngredient dragComp = dish.GetComponent<DraggableIngredient>();

            if (plateScript != null && dragComp != null)
            {
                plateScript.AddIngredient(dragComp);
            }
            else
            {
                Debug.LogWarning("CombineIntoDish: missing Plate or DraggableIngredient component on dish.");
            }

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
