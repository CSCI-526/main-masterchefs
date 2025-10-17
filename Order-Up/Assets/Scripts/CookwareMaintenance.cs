using UnityEngine;

public class CookwareMaintenance : MonoBehaviour
{
    [Header("Dirt Settings")]
    [SerializeField] private int maxDirtLevel = 5;
    [SerializeField] private Color cleanColor = Color.white;

    // Dark brown
    [SerializeField] private Color dirtyColor = new Color(0.3f, 0.2f, 0.1f); 

    [Header("References")]
    [SerializeField] private Renderer cookwareRenderer;
    [SerializeField] private string materialColorProperty = "_Color";

    // Reference to trash bin
    [SerializeField] private TrashBin trashBin; 

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

    /// Call this method when adding items to the cookware
    /// Color gets progressively darker with each item
    public void AddDirt(int amount = 1)
    {
        currentDirtLevel = Mathf.Min(currentDirtLevel + amount, maxDirtLevel);
        isDirty = true;
        UpdateCookwareColor();
        Debug.Log($"{gameObject.name} dirt level: {currentDirtLevel}/{maxDirtLevel}");
    }

    /// Call this when items are removed from cookware
    /// Color stays dirty until player presses C to clean
    public void RemoveItem()
    {
        // Color does NOT change when removing items
        // Dirt stays until player cleans with C
        Debug.Log($"{gameObject.name} item removed, but still dirty (Level: {currentDirtLevel})");
    }

    /// Fully clean the cookware - press C
    /// Returns color to completely clean state
    /// Transfers dirt to trash bin
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

    /// Update the visual appearance based on dirt level
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

    /// Get current dirt level
    public int GetDirtLevel()
    {
        return currentDirtLevel;
    }

    /// Check if cookware is dirty
    public bool IsDirty()
    {
        return isDirty;
    }

    /// Check if cookware is at maximum dirt
    public bool IsMaxDirty()
    {
        return currentDirtLevel >= maxDirtLevel;
    }

    /// Check if cookware is clean
    public bool IsClean()
    {
        return !isDirty && currentDirtLevel == 0;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = isDirty ? Color.red : Color.green;
        Vector3 pos = transform.position + Vector3.up * 2f;
    }
}