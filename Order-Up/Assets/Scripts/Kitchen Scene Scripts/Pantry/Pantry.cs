using UnityEngine;

public class Pantry : MonoBehaviour
{
    [Header("Pantry Settings")]
    public bool enableDebugLogs = true;

    [Header("Ingredient Prefabs")]
    public GameObject[] ingredientPrefabs; // Array of prefabs to assign to slots

    [Header("Layout Settings")]
    public float spacing = 1.5f; // Space between slots
    public bool horizontal = true; // true = horizontal row, false = vertical column
    
    [Header("Auto-Setup")]
    public bool autoCreateSlots = true;
    
    private PantrySlot[] slots;

    void Start()
    {
        if (autoCreateSlots)
        {
            CreateSlots();
        }
        else
        {
            slots = GetComponentsInChildren<PantrySlot>();
            AssignPrefabsToSlots();
        }
        
        SpawnInitialIngredients();
    }

    void CreateSlots()
    {
        // Clear existing auto-created slots
        foreach (Transform child in transform)
        {
            if (child.GetComponent<PantrySlot>() != null && child.name.Contains("PantrySlot_"))
                Destroy(child.gameObject);
        }

        slots = new PantrySlot[ingredientPrefabs.Length];

        // Calculate centered starting position
        float totalSize = (ingredientPrefabs.Length - 1) * spacing;
        float startPos = -totalSize * 0.5f;

        for (int i = 0; i < ingredientPrefabs.Length; i++)
        {
            // Create slot
            GameObject slotObj = new GameObject($"PantrySlot_{i}");
            slotObj.transform.SetParent(transform);
            
            // Position slot
            Vector3 localPos = Vector3.zero;
            if (horizontal)
                localPos.x = startPos + (i * spacing);
            else
                localPos.y = startPos + (i * spacing);
                
            slotObj.transform.localPosition = localPos;
            
            // Add PantrySlot component
            PantrySlot slot = slotObj.AddComponent<PantrySlot>();
            slot.ingredientPrefab = ingredientPrefabs[i];
            slot.Initialize(this);
            
            slots[i] = slot;
        }

        if (enableDebugLogs)
            Debug.Log($"[Pantry] Created {slots.Length} slots");
    }

    void AssignPrefabsToSlots()
    {
        for (int i = 0; i < Mathf.Min(slots.Length, ingredientPrefabs.Length); i++)
        {
            slots[i].ingredientPrefab = ingredientPrefabs[i];
            slots[i].Initialize(this);
        }
    }

    void SpawnInitialIngredients()
    {
        if (slots == null) return;

        foreach (PantrySlot slot in slots)
        {
            slot.SpawnIngredient();
        }
    }

    public void OnIngredientDragged(PantrySlot slot)
    {
        if (enableDebugLogs)
            Debug.Log($"[Pantry] Ingredient dragged from {slot.name}, respawning");
        
        slot.SpawnIngredient();
    }

    public void RefreshAllSlots()
    {
        if (slots == null) return;
        
        foreach (PantrySlot slot in slots)
        {
            slot.ClearIngredient();
            slot.SpawnIngredient();
        }
    }

    // Visualize layout in editor
    void OnDrawGizmosSelected()
    {
        if (ingredientPrefabs == null || ingredientPrefabs.Length == 0)
            return;

        Gizmos.color = Color.cyan;
        
        float totalSize = (ingredientPrefabs.Length - 1) * spacing;
        float startPos = -totalSize * 0.5f;
        
        for (int i = 0; i < ingredientPrefabs.Length; i++)
        {
            Vector3 localPos = Vector3.zero;
            if (horizontal)
                localPos.x = startPos + (i * spacing);
            else
                localPos.y = startPos + (i * spacing);
                
            Vector3 worldPos = transform.TransformPoint(localPos);
            Gizmos.DrawWireSphere(worldPos, 0.3f);
        }
    }

}