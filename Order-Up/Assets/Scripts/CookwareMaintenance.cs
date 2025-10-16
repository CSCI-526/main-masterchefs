using UnityEngine;

public class CookwareMaintenance : MonoBehaviour
{
    [Header("Dirt Settings")]
    [SerializeField] private int maxDirtLevel = 5;
    [SerializeField] private Color cleanColor = Color.white;
    [SerializeField] private Color dirtyColor = new Color(0.3f, 0.2f, 0.1f); // Dark brown

    [Header("References")]
    [SerializeField] private Renderer cookwareRenderer;
    [SerializeField] private string materialColorProperty = "_Color"; // Or "_BaseColor" for URP
    [SerializeField] private TrashBin trashBin; // Reference to trash bin

    private int currentDirtLevel = 0;
    private Material cookwareMaterial;
    private bool isDirty = false;

    void Start()
    {
        // Get or create material instance
        if (cookwareRenderer == null)
            cookwareRenderer = GetComponent<Renderer>();

        if (cookwareRenderer != null)
        {
            cookwareMaterial = cookwareRenderer.material;
            UpdateCookwareColor();
        }
        else
        {
            Debug.LogError("No Renderer found on " + gameObject.name);
        }

        // Auto-find trash bin if not assigned
        if (trashBin == null)
        {
            trashBin = FindObjectOfType<TrashBin>();
            if (trashBin == null)
            {
                Debug.LogWarning("No TrashBin found in scene! Cleaning will work but won't update trash bin.");
            }
        }
    }

    void Update()
    {
        // Press C to fully clean the cookware
        if (Input.GetKeyDown(KeyCode.C))
        {
            CleanCookware();
        }
    }

    /// <summary>
    /// Call this method when adding items to the cookware
    /// Color gets progressively darker with each item
    /// </summary>
    public void AddDirt(int amount = 1)
    {
        currentDirtLevel = Mathf.Min(currentDirtLevel + amount, maxDirtLevel);
        isDirty = true;
        UpdateCookwareColor();
        Debug.Log($"{gameObject.name} dirt level: {currentDirtLevel}/{maxDirtLevel}");
    }

    /// <summary>
    /// Call this when items are removed from cookware
    /// Color stays dirty until player presses C to clean
    /// </summary>
    public void RemoveItem()
    {
        // Color does NOT change when removing items
        // Dirt stays until player cleans with C
        Debug.Log($"{gameObject.name} item removed, but still dirty (Level: {currentDirtLevel})");
    }

    /// <summary>
    /// Fully clean the cookware - press C
    /// Returns color to completely clean state
    /// Transfers dirt to trash bin
    /// </summary>
    public void CleanCookware()
    {
        if (isDirty && currentDirtLevel > 0)
        {
            // Try to put dirt in trash bin
            if (trashBin != null)
            {
                bool trashAdded = trashBin.AddTrash(currentDirtLevel);

                if (!trashAdded)
                {
                    // Trash bin is full, cannot clean
                    Debug.Log($"Cannot clean {gameObject.name} - trash bin is full!");
                    return;
                }
            }

            // Clean the cookware
            currentDirtLevel = 0;
            isDirty = false;
            UpdateCookwareColor();
            Debug.Log($"{gameObject.name} is now completely clean!");
        }
        else
        {
            Debug.Log($"{gameObject.name} is already clean!");
        }
    }

    /// <summary>
    /// Update the visual appearance based on dirt level
    /// </summary>
    private void UpdateCookwareColor()
    {
        if (cookwareMaterial == null) return;

        if (!isDirty || currentDirtLevel == 0)
        {
            // Completely clean - original color
            cookwareMaterial.SetColor(materialColorProperty, cleanColor);
        }
        else
        {
            // Dirty - color gets darker based on dirt level
            float dirtPercentage = (float)currentDirtLevel / maxDirtLevel;
            Color targetColor = Color.Lerp(cleanColor, dirtyColor, dirtPercentage);
            cookwareMaterial.SetColor(materialColorProperty, targetColor);
        }
    }

    /// <summary>
    /// Get current dirt level
    /// </summary>
    public int GetDirtLevel()
    {
        return currentDirtLevel;
    }

    /// <summary>
    /// Check if cookware is dirty
    /// </summary>
    public bool IsDirty()
    {
        return isDirty;
    }

    /// <summary>
    /// Check if cookware is at maximum dirt
    /// </summary>
    public bool IsMaxDirty()
    {
        return currentDirtLevel >= maxDirtLevel;
    }

    /// <summary>
    /// Check if cookware is clean
    /// </summary>
    public bool IsClean()
    {
        return !isDirty && currentDirtLevel == 0;
    }

    // Optional: Visualize dirt level in Scene view
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = isDirty ? Color.red : Color.green;
        Vector3 pos = transform.position + Vector3.up * 2f;

#if UNITY_EDITOR
        UnityEditor.Handles.Label(pos, $"Dirt: {currentDirtLevel}/{maxDirtLevel} {(isDirty ? "DIRTY" : "CLEAN")}");
#endif
    }
}