using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Pot : MonoBehaviour
{
    [Header("Pot Settings")]
    public Transform ingredientParent; // Where ingredients will be parented (optional)

    [Header("Visual Feedback")]
    public GameObject highlightEffect; // Optional highlight when hovering
    public Color highlightColor;
    public float cookTime = 5f;
    public Slider stirProgressBar; // For slider 

    //---------------FLAG: don't do draggable ingredients because what if the food needs to get dragged
    // for simplicity, don't drag the recipe dish, just click on the trash to delete the food

    private DraggableIngredient ingredientInPot;
    private SpriteRenderer plateRenderer;
    private Color originalColor;
    private Dictionary<DraggableIngredient, float> cookProgress = new Dictionary<DraggableIngredient, float>();
    private bool isCooking = false;
    private float stirMultiplier = 1f;
    private bool isStirring = false;

    // Events
    public System.Action<DraggableIngredient> OnIngredientAdded;
    public System.Action<DraggableIngredient> OnIngredientRemoved;
    public System.Action OnPotFull;
    public System.Action OnPotEmpty;

    void Start()
    {
        ingredientInPot = null;
        highlightColor = Color.yellow;

        plateRenderer = GetComponent<SpriteRenderer>();
        if (plateRenderer != null)
            originalColor = plateRenderer.color;

        // If no ingredient parent specified, use this transform
        if (ingredientParent == null)
            ingredientParent = transform;

        if (highlightEffect != null)
            highlightEffect.SetActive(false);

        if (stirProgressBar != null)
            stirProgressBar.value = 0f;
    }

    void Update()
    {
        if (ingredientInPot == null || !isCooking) return;

        if (isStirring)
        {
            if (!cookProgress.ContainsKey(ingredientInPot)) return;

            cookProgress[ingredientInPot] += Time.deltaTime * stirMultiplier;

            if (stirProgressBar != null)
                stirProgressBar.value = cookProgress[ingredientInPot] / cookTime;

            if (cookProgress[ingredientInPot] >= cookTime)
            {
                CookIngredient(ingredientInPot);
                if (stirProgressBar != null)
                    stirProgressBar.value = 0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DraggableIngredient ingredient = other.GetComponent<DraggableIngredient>();
        AddIngredient(ingredient);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        DraggableIngredient ingredient = other.GetComponent<DraggableIngredient>();
        RemoveIngredient(ingredient);
    }

    public bool AddIngredient(DraggableIngredient ingredient)
    {
        // Check if we can add this ingredient
        if (!CanAddIngredient(ingredient)) return false;

        if (ingredientInPot != null) return false;

        // Add to our list
        ingredientInPot = ingredient;
        cookProgress[ingredient] = 0f;
        isCooking = true;

        if (stirProgressBar != null)
            stirProgressBar.value = 0f;

        // Parent the ingredient to the pot
        if (ingredientParent != null)
            ingredient.transform.SetParent(ingredientParent);

        // Trigger events
        OnIngredientAdded?.Invoke(ingredient);

        if (IsFull())
            OnPotFull?.Invoke();

        Debug.Log($"Added {ingredient.name} to pot. Total ingredients: 1");
        return true;
    }

    public bool RemoveIngredient(DraggableIngredient ingredient)
    {
        if (ingredientInPot == null) return false;

        bool wasEmpty = IsEmpty();

        ingredientInPot = null;
        cookProgress.Remove(ingredient);
        isCooking = false;

        if (stirProgressBar != null)
            stirProgressBar.value = 0f;

        // Unparent and re-enable dragging
        ingredient.transform.SetParent(null);
        ingredient.EnableDragging();

        OnIngredientRemoved?.Invoke(ingredient);

        if (!wasEmpty && IsEmpty())
            OnPotEmpty?.Invoke();

        Debug.Log($"Removed {ingredient.name} from pot. Total ingredients: 0");
        return true;
    }

    bool CanAddIngredient(DraggableIngredient ingredient)
    {
        // Check if plate is full
        if (IsFull())
        {
            Debug.Log("pot is full!");
            return false;
        }

        return true;
    }

    void CookIngredient(DraggableIngredient ingredient)
    {
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
        isCooking = false;

        Debug.Log($"[{gameObject.name}] Overcooked!");
        cookProgress[cookedIngredient] = 0f;
    }

    public void ApplyStirring(float intensity)
    {
        isStirring = true;
        stirMultiplier = 1f + intensity;
        Debug.Log($"[{gameObject.name}] Stirring applied. Multiplier = {stirMultiplier:F2}");
    }

    public void StopStirring()
    {
        isStirring = false;
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
    public bool IsFull() => ingredientInPot != null;
    public bool IsEmpty() => ingredientInPot == null;
    public DraggableIngredient GetIngredient() => ingredientInPot;

    // Button to reset the dish
    public void ClearPot()
    {
        RemoveIngredient(ingredientInPot);
    }
}
