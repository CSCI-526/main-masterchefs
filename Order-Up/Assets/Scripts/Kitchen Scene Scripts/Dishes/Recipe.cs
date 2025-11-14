
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
[System.Serializable]
public class RecipeIngredientRequirement
{
    public IngredientData ingredient;
    public IngredientState requiredState;
    public CookwareType requiredCookware = CookwareType.None;
    public IngredientChoppingState requiredChoppingState = IngredientChoppingState.Whole;
}


// ScriptableObject to define a recipe
[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public string dishName;
    public string ID;

    public List<RecipeIngredientRequirement> requiredIngredients;
    public GameObject dishPrefab; // The final food item to spawn
    public Sprite icon;
    

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