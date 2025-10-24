using UnityEngine;
using System;

/// <summary>
/// Manages pantry slots that spawn unlimited ingredients.
/// Works with Unity's Grid Layout Group component.
/// Each child GameObject with this component acts as a slot.
/// </summary>
public class PantryIngredient : MonoBehaviour
{
    [Header("Ingredient Prefab")]
    [Tooltip("The ingredient prefab this pantry slot will spawn")]
    public GameObject ingredientPrefab;

    [Header("Settings")]
    public bool isUnlimitedSlot = true; // If true, respawns ingredient after dragging
    [SerializeField] private bool enableDebugLogs = false;

    private GameObject currentIngredient;
    private DraggableIngredient draggableComponent;
    private Vector3 slotPosition;

    public System.Action<DraggableIngredient> OnPantryIngredientDragged;

    void Start()
    {
        // Delay spawning to next frame so Grid Layout has time to position slots
        StartCoroutine(DelayedSpawn());
    }

    System.Collections.IEnumerator DelayedSpawn()
    {
        // Wait for layout to complete
        yield return new WaitForEndOfFrame();
        SpawnIngredient();
    }

    void SpawnIngredient()
    {
        // Don't spawn if we already have an ingredient
        if (currentIngredient != null)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"[Pantry:{name}] Already has an ingredient");
            return;
        }

        if (ingredientPrefab == null)
        {
            Debug.LogError($"[Pantry:{name}] No ingredient prefab assigned!");
            return;
        }

        slotPosition = transform.position;
        
        // Instantiate at slot position
        currentIngredient = Instantiate(ingredientPrefab, slotPosition, Quaternion.identity);
        currentIngredient.transform.SetParent(transform);

        // Get DraggableIngredient component
        draggableComponent = currentIngredient.GetComponent<DraggableIngredient>();
        if (draggableComponent == null)
        {
            Debug.LogError($"[Pantry:{name}] Ingredient prefab missing DraggableIngredient component!");
            Destroy(currentIngredient);
            return;
        }

        // Set original position
        draggableComponent.SetNewOriginalPosition();

        // Subscribe to events
        draggableComponent.OnStartDrag += OnIngredientStartDrag;
        draggableComponent.OnDroppedOnPlate += OnIngredientDroppedOnPlate;
        draggableComponent.OnDroppedOnCookware += OnIngredientDroppedOnCookware;

        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Spawned ingredient: {currentIngredient.name}");
    }

    void OnIngredientStartDrag(DraggableIngredient ingredient)
    {
        // Unparent so it can move freely
        if (currentIngredient != null)
        {
            currentIngredient.transform.SetParent(null);
        }

        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient started dragging");
            
        OnPantryIngredientDragged?.Invoke(ingredient);
    }

    void OnIngredientDroppedOnPlate(DraggableIngredient ingredient, Plate plate)
    {
        // Successfully dropped on plate - respawn if unlimited
        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient dropped on plate: {plate.name}");

        CleanupCurrentIngredient();

        if (isUnlimitedSlot)
        {
            SpawnIngredient();
        }
    }

    void OnIngredientDroppedOnCookware(DraggableIngredient ingredient, Cookwares cookware)
    {
        // Successfully dropped on cookware - respawn if unlimited
        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient dropped on cookware: {cookware.name}");

        CleanupCurrentIngredient();

        if (isUnlimitedSlot)
        {
            SpawnIngredient();
        }
    }

    void CleanupCurrentIngredient()
    {
        if (draggableComponent != null)
        {
            draggableComponent.OnStartDrag -= OnIngredientStartDrag;
            draggableComponent.OnDroppedOnPlate -= OnIngredientDroppedOnPlate;
            draggableComponent.OnDroppedOnCookware -= OnIngredientDroppedOnCookware;
        }

        currentIngredient = null;
        draggableComponent = null;
    }

    public void RefreshSlot()
    {
        ClearIngredient();
        SpawnIngredient();
    }

    public void ClearIngredient()
    {
        if (currentIngredient != null)
        {
            Destroy(currentIngredient);
            CleanupCurrentIngredient();
        }
    }

    void OnDestroy()
    {
        CleanupCurrentIngredient();
    }

    // Visual feedback in editor
    void OnDrawGizmos()
    {
        Gizmos.color = currentIngredient != null ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.4f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}