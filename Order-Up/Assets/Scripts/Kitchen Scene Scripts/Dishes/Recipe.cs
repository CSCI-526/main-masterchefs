using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
[System.Serializable]
public class RecipeIngredientRequirement
{
    public IngredientData ingredient;
    public IngredientState requiredState;
    public CookwareType requiredCookware = CookwareType.None;
}


// ScriptableObject to define a recipe
[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public string dishName;
    public string ID;

    //public List<IngredientData> requiredIngredients = new List<IngredientData>();
    public List<RecipeIngredientRequirement> requiredIngredients;
    public GameObject dishPrefab; // The final food item to spawn

    public Sprite icon;

    // Check if the given ingredients match this recipe
    // public bool MatchesIngredients(List<Ingredient> ingredients)
    // {
    //     if (ingredients.Count != requiredIngredients.Count)
    //         return false;

    //      // Extract the data from the given ingredient instances
    //     var providedData = ingredients.Select(i => i.ingredientData).OrderBy(d => d.ingredientName).ToList();
    //     var requiredData = requiredIngredients.OrderBy(d => d.ingredientName).ToList();


    //     // Check if all ingredients match
    //     for (int i = 0; i < providedData.Count; i++)
    //     {
    //         if (providedData[i] != requiredData[i])
    //             return false;
    //     }

    //     return true;
    // }

    public bool MatchesIngredients(List<Ingredient> provided)
    {
        //check the ingredient nums
        if(provided.Count != requiredIngredients.Count)
            return false;
        
        
        // check ingredients
        foreach (RecipeIngredientRequirement req in requiredIngredients)
        {
            bool found = false;
            foreach (Ingredient ing in provided)
            {
                Debug.Log($"Checking {ing.ingredientData.name} ({ing.currentState}) vs {req.ingredient.name} ({req.requiredState})");

                if (ing.ingredientData == req.ingredient && ing.currentState == req.requiredState && 
                (req.requiredCookware == CookwareType.None || ing.currentCookware == req.requiredCookware))
                {
                    found = true;
                    Debug.Log($"Found recipe: {req.ingredient}");
                    break;
                }
            }

            if(!found)
                return false;
        }
        return true;
    }
}