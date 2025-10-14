using UnityEngine;
using System.Collections.Generic;

public class Trash : MonoBehaviour
{
    [Header("Target References")]
    [Tooltip("Assign the Plate GameObject (containing the Plate component). If left empty, we'll try to use 'dish' or auto-find a Plate in scene.")]
    public Plate targetPlate;

    [Tooltip("(Optional) Legacy reference. If assigned, we'll attempt to get Plate component from this GameObject.")]
    public GameObject dish; // Backward compatibility

    [Header("Behavior")] 
    [Tooltip("Also destroy ingredient GameObjects after clearing the plate list.")]
    public bool alsoDestroyIngredients = false;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private void Awake()
    {
        // Try to resolve targetPlate if not assigned in Inspector
        if (targetPlate == null && dish != null)
        {
            targetPlate = dish.GetComponent<Plate>();
            if (enableDebugLogs && targetPlate != null)
                Debug.Log("[Trash] Resolved Plate from 'dish' reference.");
        }

        if (targetPlate == null)
        {
            targetPlate = FindAnyObjectByType<Plate>();
            if (enableDebugLogs && targetPlate != null)
                Debug.Log("[Trash] Auto-found a Plate in the scene.");
        }
    }

    private void OnMouseDown()
    {
        if (targetPlate == null)
        {
            if (enableDebugLogs) Debug.LogWarning("[Trash] No Plate assigned or found. Assign 'targetPlate' in Inspector.");
            return;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[Trash] Current plate ingredient count: {targetPlate.GetIngredientCount()}");
        }

        if (targetPlate.IsEmpty())
        {
            if (enableDebugLogs) Debug.Log("[Trash] Plate is already empty.");
            return;
        }

        // Optionally capture the current ingredients before clearing
        List<DraggableIngredient> toDestroy = null;
        if (alsoDestroyIngredients)
        {
            toDestroy = targetPlate.GetIngredients(); // Copies the list
        }

        // Clear the logical list and detach ingredients from the plate
        targetPlate.ClearPlate();

        if (alsoDestroyIngredients && toDestroy != null)
        {
            // Now destroy the ingredient GameObjects themselves
            foreach (var ing in toDestroy)
            {
                if (ing != null)
                {
                    Destroy(ing.gameObject);
                    if (enableDebugLogs) Debug.Log($"[Trash] Destroyed ingredient GameObject: {ing.name}");
                }
            }
        }

        if (enableDebugLogs)
        {
            Debug.Log("[Trash] Plate cleared.");
        }
    }
}
