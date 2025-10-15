using UnityEngine;
using System.Collections.Generic;

public class Pot : MonoBehaviour
{
    [Header("Pot Settings")]
    public Transform ingredientParent; // Where ingredients will be parented (optional)

    [Header("Visual Feedback")]
    public GameObject highlightEffect; // Optional highlight when hovering
    public Color highlightColor;

    //---------------FLAG: dont do draggable ingredients because whatif the food needs to get dragged
    // for simplicity, dont drag the recipe dish, just click on the trash to delete the food

    private List<DraggableIngredient> ingredientsInPot; 
    private SpriteRenderer plateRenderer;
    private Color originalColor;
    private Dictionary<DraggableIngredient, float> cookProgress = new Dictionary<DraggableIngredient, float>();
    private bool isCooking = false;
    private float stirMultiplier = 1f;
    private float cookTime = 5f;

    // Events
    public System.Action<DraggableIngredient> OnIngredientAdded;
    public System.Action<DraggableIngredient> OnIngredientRemoved;
    public System.Action OnPotFull;
    public System.Action OnPotEmpty;

    void Start()
    {
        ingredientsInPot = new List<DraggableIngredient>();
        highlightColor = Color.yellow;

        plateRenderer = GetComponent<SpriteRenderer>();
        if (plateRenderer != null)
            originalColor = plateRenderer.color;

        // If no ingredient parent specified, use this transform
        if (ingredientParent == null)
            ingredientParent = transform;

        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }

    void Update()
    {
        if (!isCooking) return;

        foreach (var ingredient in ingredientsInPot)
        {
            cookProgress[ingredient] += Time.deltaTime * stirMultiplier;

            if (cookProgress[ingredient] >= cookTime)
            {
                CookIngredient(ingredient);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DraggableIngredient ingredient = other.GetComponent<DraggableIngredient>();
        if (!ingredientsInPot.Contains(ingredient))
        {
            AddIngredient(ingredient);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        DraggableIngredient ingredient = other.GetComponent<DraggableIngredient>();
        if (ingredientsInPot.Contains(ingredient))
        {
            RemoveIngredient(ingredient);
        }
    }

    public bool AddIngredient(DraggableIngredient ingredient)
    {
        // Check if we can add this ingredient
        if (!CanAddIngredient(ingredient))
            return false;

        // Add to our list
        ingredientsInPot.Add(ingredient);
        cookProgress[ingredient] = 0f;
        isCooking = true;

        // Parent the ingredient to the plate (optional)
        if (ingredientParent != null)
            ingredient.transform.SetParent(ingredientParent);

        // Trigger events
        OnIngredientAdded?.Invoke(ingredient);

        if (IsFull())
            OnPotFull?.Invoke();

        Debug.Log($"Added {ingredient.name} to plate. Total ingredients: {ingredientsInPot.Count}");
        return true;
    }

    public bool RemoveIngredient(DraggableIngredient ingredient)
    {
        if (!ingredientsInPot.Contains(ingredient))
            return false;

        bool wasEmpty = IsEmpty();

        ingredientsInPot.Remove(ingredient);
        cookProgress.Remove(ingredient);
        isCooking = false;

        // Unparent and re-enable dragging
        ingredient.transform.SetParent(null);
        ingredient.EnableDragging();

        OnIngredientRemoved?.Invoke(ingredient);

        if (!wasEmpty && IsEmpty())
            OnPotEmpty?.Invoke();

        Debug.Log($"Removed {ingredient.name} from plate. Total ingredients: {ingredientsInPot.Count}");
        return true;
    }

    bool CanAddIngredient(DraggableIngredient ingredient)
    {
        // Check if plate is full
        if (IsFull())
        {
            Debug.Log("Plate is full!");
            return false;
        }

        return true;
    }

    void CookIngredient(DraggableIngredient ingredient)
    {
        // Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        // IngredientData data = ingredientComponent.ingredientData;

        // if (data.cookedResult == null)
        // {
        //     Debug.Log($"[{gameObject.name}] Cannot be cooked!");
        //     return;
        // }

        // ingredientComponent.ingredientData = data.cookedResult;
        // ingredientComponent.SetSprite(data.cookedResult.icon, data.cookedResult.ingredientName);

        if (ingredient.cookedResult == null)
        {
            Debug.Log($"[{gameObject.name}] Cannot be cooked!");
            return;
        }

        // Remember transform info
        Vector3 pos = ingredient.transform.position;
        Quaternion rot = ingredient.transform.rotation;
        Transform parent = ingredient.transform.parent;

        // remove raw ingredient
        RemoveIngredient(ingredient);
        Destroy(ingredient.gameObject);

        // spawn cooked ingredient
        DraggableIngredient cookedIngredient = Instantiate(ingredient.cookedResult, pos, rot, parent);
        cookedIngredient.SetNewOriginalPosition();
        AddIngredient(cookedIngredient);

        Debug.Log($"[{gameObject.name}] Overcooked!");
        cookProgress[cookedIngredient] = 0f;
    }

    public void ApplyStirring(float intensity)
    {
        stirMultiplier = 1f + intensity;
        Debug.Log($"[{gameObject.name}] Stirring applied. Multiplier = {stirMultiplier:F2}");
    }

    public void StopStirring()
    {
        stirMultiplier = 1f;
    }

    // Visual feedback methods
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


    //----------------------------------- UTILITITY METHODS -----------------------------------//
    public bool IsFull() => ingredientsInPot.Count == 1;
    public bool IsEmpty() => ingredientsInPot.Count == 0;
    public int GetIngredientCount() => ingredientsInPot.Count;
    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsInPot);

    // Button to reset the dish
    public void ClearPot()
    {
        while (ingredientsInPot.Count > 0)
        {
            RemoveIngredient(ingredientsInPot[0]);
        }
    }
}