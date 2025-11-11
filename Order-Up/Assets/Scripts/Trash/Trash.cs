using UnityEngine;

/// <summary>
/// Handles trash functionality:
/// - Accepts dragged ingredients and notifies their pantry to respawn
/// </summary>
/// 
[RequireComponent(typeof(PolygonCollider2D))]
public class Trash : MonoBehaviour, IDropZone
{
    [SerializeField] private bool enableDebugLogs = false;

    /// <summary>
    /// Called when an ingredient is dropped on the trash
    /// This notifies the appropriate pantry to respawn
    /// </summary>
    public void OnIngredientDropped(DraggableIngredient ingredient)
    {
        if (ingredient == null)
        {
            Debug.LogWarning("[Trash] Null ingredient dropped");
            return;
        }

        if (enableDebugLogs)
            Debug.Log($"[Trash] Ingredient dropped: {ingredient.name}");

        // Find which pantry this ingredient came from
        PantryIngredient sourcePantry = FindPantryForIngredient(ingredient);

        if (sourcePantry != null)
        {
            // Notify the pantry to respawn
            sourcePantry.OnIngredientTrashed();

            if (enableDebugLogs)
                Debug.Log($"[Trash] Notified pantry {sourcePantry.name} to respawn");
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning($"[Trash] Could not find source pantry for ingredient: {ingredient.name}");
        }

        // Destroy the ingredient
        Destroy(ingredient.gameObject);
    }

    /// <summary>
    /// Finds which pantry spawned this ingredient by checking the ingredient prefab
    /// </summary>
    private PantryIngredient FindPantryForIngredient(DraggableIngredient ingredient)
    {
        PantryIngredient[] pantries = FindObjectsByType<PantryIngredient>(FindObjectsSortMode.None);

        foreach (PantryIngredient pantry in pantries)
        {
            if (pantry.ingredientPrefab != null)
            {
                // Check if the ingredient's name matches the prefab (Unity adds "(Clone)" suffix)
                string prefabName = pantry.ingredientPrefab.name;
                string ingredientName = ingredient.name.Replace("(Clone)", "").Trim();

                // FLAG: Change this to ingredient type
                ingredientName = ingredient.name.Replace("_Cooked", "_Raw").Replace("_Overcooked", "_Raw").Trim();
                if (ingredientName == prefabName)
                {
                    return pantry;
                }
            }
        }

        return null;
    }

    // IDropZone implementation
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    void OnDrawGizmos()
    {
        // Visual indicator for trash in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}