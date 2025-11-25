using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Manages pantry slots that spawn only 1 ingredient at a time.
/// Works with Unity's Grid Layout Group component.
/// Each child GameObject with this component acts as a slot.
/// </summary>
public class PantryIngredient : MonoBehaviour
{
    [Header("Ingredient Prefab")]
    [Tooltip("The ingredient prefab this pantry slot will spawn")]
    public GameObject ingredientPrefab;

    [Header("Settings")]
    [SerializeField] private bool enableDebugLogs = false;

    private bool hasActiveIngredient = false;
    private GameObject currentIngredient;
    private DraggableIngredient draggableComponent;
    private Vector3 slotPosition;

    void Start()
    {
        // Delay spawning to next frame so Grid Layout has time to position slots
        StartCoroutine(DelayedSpawn());
    }

    System.Collections.IEnumerator DelayedSpawn()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        SpawnIngredient();
    }

    public void OnIngredientTrashed()
    {
        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient trashed - respawning");

        hasActiveIngredient = false;
        SpawnIngredient();
    }

    void SpawnIngredient()
    {
        // Don't spawn if we already have an ingredient
        if (hasActiveIngredient == true)
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
        currentIngredient.transform.SetParent(transform, worldPositionStays: true);


        // Get DraggableIngredient component
        draggableComponent = currentIngredient.GetComponent<DraggableIngredient>();
        if (draggableComponent == null)
        {
            Debug.LogError($"[Pantry:{name}] Ingredient prefab missing DraggableIngredient component!");
            Destroy(currentIngredient);
            return;
        }

        // Subscribe to events
        draggableComponent.OnStartDrag += OnIngredientStartDrag;
        draggableComponent.OnDroppedOnPlate += OnIngredientDroppedOnPlate;
        draggableComponent.OnDroppedOnCookware += OnIngredientDroppedOnCookware;

        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Spawned ingredient: {currentIngredient.name}");

        draggableComponent.SetNewOriginalPosition();
        hasActiveIngredient = true;
    }

    void OnIngredientStartDrag(DraggableIngredient ingredient)
    {
        // Unparent so it can move freely
        if (currentIngredient != null)
        {
            currentIngredient.transform.SetParent(null);

            if (draggableComponent != null)
            {
                draggableComponent.OnStartDrag -= OnIngredientStartDrag;
            }

            currentIngredient = null;
            draggableComponent = null;
        }

        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient started dragging");
    }

    void OnIngredientDroppedOnPlate(DraggableIngredient ingredient, Plate plate)
    {
        // Successfully dropped on plate - respawn if unlimited
        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient dropped on plate: {plate.name}");

        CleanupCurrentIngredient();

    }

    void OnIngredientDroppedOnCookware(DraggableIngredient ingredient, BaseCookware cookware)
    {
        // Successfully dropped on cookware - respawn if unlimited
        if (enableDebugLogs)
            Debug.Log($"[Pantry:{name}] Ingredient dropped on cookware: {cookware.name}");

        CleanupCurrentIngredient();
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