
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ScriptableObject to define a recipe
[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public string dishName;
    public string ID;
    public List<IngredientData> requiredIngredients = new List<IngredientData>();
    public GameObject dishPrefab; // The final food item to spawn

    public Sprite icon;

    // Check if the given ingredients match this recipe
    public bool MatchesIngredients(List<Ingredient> ingredients)
    {
        if (ingredients.Count != requiredIngredients.Count)
            return false;

         // Extract the data from the given ingredient instances
        var providedData = ingredients.Select(i => i.ingredientData).OrderBy(d => d.ingredientName).ToList();
        var requiredData = requiredIngredients.OrderBy(d => d.ingredientName).ToList();


        // Check if all ingredients match
        for (int i = 0; i < providedData.Count; i++)
        {
            if (providedData[i] != requiredData[i])
                return false;
        }

        return true;
    }
}