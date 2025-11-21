using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
//using System.Numerics;

public class Plate : MonoBehaviour, IDropZone
{
    [Header("Plate Settings")]
    public Transform ingredientParent;
    public float maxIngredients = 4f;
    public bool allowDuplicates = false;
    public float ingredientSpacing = 0.2f;

    [Header("Visual Feedback")]
    public GameObject highlightEffect;
    public Color highlightColor;
    private SpriteRenderer plateRenderer;

    [Header("Dish Display Settings")]
    public TextMeshProUGUI dishIngredientsText; // Text component to show dish ingredients
    public float maxTextWidth = 200f; // Maximum width for text box
    public float minFontSize = 12f;
    public float maxFontSize = 24f;
    private Color originalColor;

    [Header("CombinationSystem")]
    private List<DraggableIngredient> ingredientsOnPlate; // make sure to take it out if the ingredient is removed
    public CombinationSystem comboSystem;

    // Events
    public System.Action<DraggableIngredient> OnIngredientAdded;
    public System.Action<DraggableIngredient> OnIngredientRemoved;
    public System.Action OnPlateFull;
    public System.Action OnPlateEmpty;

    void Start()
    {
        ingredientsOnPlate = new List<DraggableIngredient>();
        highlightColor = Color.yellow;

        plateRenderer = GetComponent<SpriteRenderer>();
        if (plateRenderer != null)
            originalColor = plateRenderer.color;

        if (ingredientParent == null)
            ingredientParent = transform;

        if (highlightEffect != null)
            highlightEffect.SetActive(false);

        UpdateDishDisplay(); // Initialize display
    }
    #region IDropZone Implementation
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    #endregion

    #region Ingredient Management
    public bool AddIngredient(DraggableIngredient ingredient)
    {
        if (!CanAddIngredient(ingredient)) 
            return false;

        ingredientsOnPlate.Add(ingredient);
        //PositionIngredientOnPlate(ingredient);

        if (ingredientParent != null)
            ingredient.transform.SetParent(ingredientParent);

        OnIngredientAdded?.Invoke(ingredient);

        if (IsFull())
            OnPlateFull?.Invoke();

        if (ingredientsOnPlate.Count > 1)
            comboSystem.CheckForCombinations();

        UpdateDishDisplay(); // Update display after adding ingredient

         
        PopupManager.Instance.ShowPopup("Plate is full!", this.transform);

        Debug.Log($"Added {ingredient.name} to plate. Total ingredients: {ingredientsOnPlate.Count}");
        return true;
    }

    public bool RemoveIngredient(DraggableIngredient ingredient)
    {
        if (!ingredientsOnPlate.Contains(ingredient))
            return false;

        bool wasEmpty = IsEmpty();

        ingredientsOnPlate.Remove(ingredient);

        ingredient.transform.SetParent(null);

        Destroy(ingredient.gameObject);
        //RepositionIngredients();

        OnIngredientRemoved?.Invoke(ingredient);

        if (!wasEmpty && IsEmpty())
            OnPlateEmpty?.Invoke();

        UpdateDishDisplay(); // Update display after removing ingredient

        Debug.Log($"Removed {ingredient.name} from plate. Total ingredients: {ingredientsOnPlate.Count}");
        return true;
    }

    bool CanAddIngredient(DraggableIngredient ingredient)
    {
        if (IsFull())
        {
            Debug.Log("Plate is full!");

            return false;
        }

        if (!allowDuplicates)
        {
            foreach (DraggableIngredient existing in ingredientsOnPlate)
            {
                if (existing.name == ingredient.name ||
                    existing.gameObject.name == ingredient.gameObject.name)
                {
                    Debug.Log("Duplicate ingredient not allowed!");
                    return false;
                }
            }
        }

        return true;
    }
    #endregion

    #region Positioning
    /// <summary>
    /// Should rearrange the ingredients from top down left right in a circular pattern
    /// no more than 4 ingredients on the plate, so the position shouldn't be overlapping
    /// </summary>
    void PositionIngredientOnPlate(DraggableIngredient ingredient)
    {
        Vector3 plateCenter = transform.position;
        int index = ingredientsOnPlate.Count - 1;

        float angle = index * 60f * Mathf.Deg2Rad;
        float radius = 0.3f + (index / 6f) * 0.2f;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            -0.1f
        );

        ingredient.transform.position = plateCenter + offset;
    }

    void RepositionIngredients()
    {
        for (int i = 0; i < ingredientsOnPlate.Count; i++)
        {
            if (ingredientsOnPlate[i] != null)
            {
                var temp = ingredientsOnPlate[i];
                ingredientsOnPlate.RemoveAt(i);
                ingredientsOnPlate.Insert(i, temp);

                Vector3 plateCenter = transform.position;
                float angle = i * 60f * Mathf.Deg2Rad;
                float radius = 0.3f + (i / 6f) * 0.2f;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    -0.1f
                );

                ingredientsOnPlate[i].transform.position = plateCenter + offset;
            }
        }
    }
    #endregion

    #region Visual Feedback
    void OnMouseEnter()
    {
        ShowHighlight();
    }

    void OnMouseExit()
    {
        HideHighlight();
    }

    void ShowHighlight()
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(true);
        else if (plateRenderer != null)
            plateRenderer.color = highlightColor;
    }

    void HideHighlight()
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
        else if (plateRenderer != null)
            plateRenderer.color = originalColor;
    }
    #endregion


    public void ClearPlate()
    {
        while (ingredientsOnPlate.Count > 0)
        {
            RemoveIngredient(ingredientsOnPlate[0]);
        }

        // remove every child under plate, even if they aren't ingredients
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        UpdateDishDisplay(); // Update display after clearing
    }

    public void UpdateDishDisplay()
    {
        if (dishIngredientsText == null)
            return;

        // Check if a recipe is formed
        Recipe matchedRecipe = GetMatchingRecipe();

        if (matchedRecipe != null)
        {
            // Show recipe name if found
            dishIngredientsText.text = matchedRecipe.dishName;
            AdjustTextSize(matchedRecipe.dishName);
        }
        else if (IsEmpty())
        {
            // Show "Dish Empty" when no ingredients
            dishIngredientsText.text = "Dish Empty";
            dishIngredientsText.fontSize = maxFontSize;
        }
        else
        {
            // Show ingredient names separated by " + "
            string displayText = GetIngredientsDisplayText();
            dishIngredientsText.text = displayText;
            AdjustTextSize(displayText);
        }
    }
    
    private string GetIngredientsDisplayText()
    {
        if (IsEmpty())
            return "Dish Empty";

        List<string> ingredientNames = new List<string>();
        
        // Get ingredient names from the plate
        foreach (Transform child in ingredientParent)
        {
            Ingredient ingredient = child.GetComponent<Ingredient>();
            if (ingredient != null && ingredient.ingredientData != null)
            {
                ingredientNames.Add(ingredient.ingredientData.ingredientName);
            }
            else
            {
                // Fallback to object name if no Ingredient component
                ingredientNames.Add(child.gameObject.name);
            }
        }

        return string.Join(" + ", ingredientNames);
    }

    /// <summary>
    /// Adjusts text size to fit within the specified width
    /// </summary>
    private void AdjustTextSize(string text)
    {
        if (dishIngredientsText == null)
            return;

        // Start with max font size
        dishIngredientsText.fontSize = maxFontSize;
        dishIngredientsText.text = text;

        // Force update to calculate size
        dishIngredientsText.ForceMeshUpdate();

        // Get the preferred width of the text
        float textWidth = dishIngredientsText.preferredWidth;

        // If text is too wide, reduce font size
        if (textWidth > maxTextWidth)
        {
            float scaleFactor = maxTextWidth / textWidth;
            float newFontSize = Mathf.Max(minFontSize, maxFontSize * scaleFactor);
            dishIngredientsText.fontSize = newFontSize;
        }
    }

    /// <summary>
    /// Checks if current ingredients match any recipe
    /// </summary>
    private Recipe GetMatchingRecipe()
    {
        if (comboSystem == null || ingredientParent == null)
            return null;

        List<Ingredient> ingredients = new List<Ingredient>();

        // Get all ingredients from the plate
        foreach (Transform child in ingredientParent)
        {
            Ingredient ing = child.GetComponent<Ingredient>();
            if (ing != null)
            {
                ingredients.Add(ing);
            }
        }

        if (ingredients.Count == 0)
            return null;

        // Check all recipes to find a match
        foreach (Recipe recipe in comboSystem.allRecipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                return recipe;
            }
        }

        return null;
    }

    public List<string> GetIngredientNames()
    {
        List<string> names = new List<string>();
        foreach (DraggableIngredient ingredient in ingredientsOnPlate)
        {
            names.Add(ingredient.name);
        }
        return names;
    }

    public string getDishName()
    {
        if (IsEmpty())
            return "Empty Plate";

        foreach (Transform child in transform)
        {
            GameObject childObject = child.gameObject;
            Debug.Log("Child object under plate: " + childObject.name);
            return childObject.name;
        }

        return "";
    }
    public bool IsFull() => ingredientsOnPlate.Count >= maxIngredients;
    public bool IsEmpty() => ingredientsOnPlate.Count == 0;
    public int GetIngredientCount() => ingredientsOnPlate.Count;
    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsOnPlate);
}