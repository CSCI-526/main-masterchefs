//using UnityEngine;
//using UnityEngine.UI;

//public class CookwareMaintenance : MonoBehaviour
//{
//    public enum DirtLevel { Clean = 0, Mild = 1, Dirty = 2, VeryDirty = 3, Extreme = 4 }

//    [Header("Refs")]
//    public Image cookwareImage;
//    public bool listenForKeyboardInput = true;
//    public KeyCode cleanOneStepKey = KeyCode.C;

//    [Header("Colors per level")]
//    public Color cleanColor = Color.white;
//    public Color mildlyDirtyColor = new Color(1f, 0.9f, 0.6f);
//    public Color dirtyColor = new Color(1f, 0.7f, 0.3f);
//    public Color veryDirtyColor = new Color(0.9f, 0.4f, 0.2f);
//    public Color extremeDirtyColor = new Color(0.8f, 0.2f, 0.2f);

//    [Header("Usage thresholds (inclusive lower bounds)")]
//    // Level jumps at these usage counts: 0→Clean, 1→Mild, 3→Dirty, 5→VeryDirty, 7→Extreme
//    public int[] thresholds = { 0, 1, 3, 5, 7 };

//    [Tooltip("Total times this cookware has been used.")]
//    public int usageCount = 0;

//    public DirtLevel CurrentLevel { get; private set; } = DirtLevel.Clean;

//    void Start()
//    {
//        RecomputeLevelFromUsage();
//        ApplyLevelColor();
//    }

//    void Update()
//    {
//        if (listenForKeyboardInput && Input.GetKeyDown(cleanOneStepKey))
//        {
//            CleanOneStep();
//        }
//    }

//    // Call this when the cookware is used in an order
//    public void IncrementUsage()
//    {
//        usageCount++;
//        RecomputeLevelFromUsage();
//        ApplyLevelColor();
//    }

//    // Pressing C: go back exactly one level (cannot go below Clean)
//    public void CleanOneStep()
//    {
//        if (CurrentLevel == DirtLevel.Clean) return;

//        CurrentLevel -= 1;

//        // Optionally align usageCount to the start of the new level
//        usageCount = thresholds[(int)CurrentLevel];

//        ApplyLevelColor();
//    }

//    // Full reset (if you still want a button that makes it brand new)
//    public void CleanAll()
//    {
//        usageCount = 0;
//        CurrentLevel = DirtLevel.Clean;
//        ApplyLevelColor();
//    }

//    void RecomputeLevelFromUsage()
//    {
//        // Determine highest level whose threshold <= usageCount
//        int level = 0;
//        for (int i = 0; i < thresholds.Length; i++)
//        {
//            if (usageCount >= thresholds[i]) level = i;
//        }
//        CurrentLevel = (DirtLevel)Mathf.Clamp(level, 0, thresholds.Length - 1);
//    }

//    void ApplyLevelColor()
//    {
//        if (cookwareImage == null) return;

//        switch (CurrentLevel)
//        {
//            case DirtLevel.Clean: cookwareImage.color = cleanColor; break;
//            case DirtLevel.Mild: cookwareImage.color = mildlyDirtyColor; break;
//            case DirtLevel.Dirty: cookwareImage.color = dirtyColor; break;
//            case DirtLevel.VeryDirty: cookwareImage.color = veryDirtyColor; break;
//            case DirtLevel.Extreme: cookwareImage.color = extremeDirtyColor; break;
//        }
//    }
//}

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