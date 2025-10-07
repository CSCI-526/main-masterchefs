using UnityEngine;

public class PantrySlot : MonoBehaviour
{
    [Header("Slot Configuration")]
    public GameObject ingredientPrefab; // Assign the prefab in inspector
    
    private Pantry pantry;
    private GameObject currentIngredient;
    private DraggableIngredient draggableComponent;

    public void Initialize(Pantry parentPantry)
    {
        pantry = parentPantry;
    }

    public void SpawnIngredient()
    {
        // Don't spawn if we already have an ingredient
        if (currentIngredient != null)
        {
            Debug.LogWarning($"[{name}] Already has an ingredient, skipping spawn");
            return;
        }

        if (ingredientPrefab == null)
        {
            Debug.LogError($"[{name}] No ingredient prefab assigned!");
            return;
        }

        // Instantiate the ingredient at this slot's position
        currentIngredient = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
        currentIngredient.transform.SetParent(transform); // Optional: parent to slot for organization
        
        // Get or add DraggableIngredient component
        draggableComponent = currentIngredient.GetComponent<DraggableIngredient>();
        if (draggableComponent == null)
        {
            draggableComponent = currentIngredient.AddComponent<DraggableIngredient>();
        }

        // Set the original position to this slot's world position
        draggableComponent.SetNewOriginalPosition();

        // Subscribe to drag events
        draggableComponent.OnStartDrag += OnIngredientStartDrag;
        draggableComponent.OnDroppedOnPlate += OnIngredientDroppedOnPlate;

        if (pantry != null && pantry.enableDebugLogs)
        {
            Debug.Log($"[{name}] Spawned ingredient: {currentIngredient.name}");
        }
    }

    void OnIngredientStartDrag(DraggableIngredient ingredient)
    {
        // Unparent from slot so it can move freely
        if (currentIngredient != null)
        {
            currentIngredient.transform.SetParent(null);
        }

        if (pantry != null && pantry.enableDebugLogs)
        {
            Debug.Log($"[{name}] Ingredient started dragging");
        }
    }

    void OnIngredientDroppedOnPlate(DraggableIngredient ingredient, Plate plate)
    {
        // Ingredient was successfully placed on a plate
        // Clean up our reference and notify pantry to respawn
        CleanupIngredient();
        
        if (pantry != null)
        {
            pantry.OnIngredientDragged(this);
        }
    }

    void CleanupIngredient()
    {
        if (draggableComponent != null)
        {
            draggableComponent.OnStartDrag -= OnIngredientStartDrag;
            draggableComponent.OnDroppedOnPlate -= OnIngredientDroppedOnPlate;
        }

        currentIngredient = null;
        draggableComponent = null;
    }

    public void ClearIngredient()
    {
        if (currentIngredient != null)
        {
            CleanupIngredient();
            Destroy(currentIngredient);
        }
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        CleanupIngredient();
    }

    // Visualize slot position in editor
    void OnDrawGizmos()
    {
        Gizmos.color = currentIngredient != null ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}