using UnityEngine;
using UnityEngine.UI;

public class CookwareMaintenance : MonoBehaviour
{
    public enum DirtLevel { Clean = 0, Mild = 1, Dirty = 2, VeryDirty = 3, Extreme = 4 }

    [Header("Refs")]
    public Image cookwareImage;
    public bool listenForKeyboardInput = true;
    public KeyCode cleanOneStepKey = KeyCode.C;

    [Header("Colors per level")]
    public Color cleanColor = Color.white;
    public Color mildlyDirtyColor = new Color(1f, 0.9f, 0.6f);
    public Color dirtyColor = new Color(1f, 0.7f, 0.3f);
    public Color veryDirtyColor = new Color(0.9f, 0.4f, 0.2f);
    public Color extremeDirtyColor = new Color(0.8f, 0.2f, 0.2f);

    [Header("Usage thresholds (inclusive lower bounds)")]
    // Level jumps at these usage counts: 0→Clean, 1→Mild, 3→Dirty, 5→VeryDirty, 7→Extreme
    public int[] thresholds = { 0, 1, 3, 5, 7 };

    [Tooltip("Total times this cookware has been used.")]
    public int usageCount = 0;

    public DirtLevel CurrentLevel { get; private set; } = DirtLevel.Clean;

    void Start()
    {
        RecomputeLevelFromUsage();
        ApplyLevelColor();
    }

    void Update()
    {
        if (listenForKeyboardInput && Input.GetKeyDown(cleanOneStepKey))
        {
            CleanOneStep();
        }
    }

    // Call this when the cookware is used in an order
    public void IncrementUsage()
    {
        usageCount++;
        RecomputeLevelFromUsage();
        ApplyLevelColor();
    }

    // Pressing C: go back exactly one level (cannot go below Clean)
    public void CleanOneStep()
    {
        if (CurrentLevel == DirtLevel.Clean) return;

        CurrentLevel -= 1;

        // Optionally align usageCount to the start of the new level
        usageCount = thresholds[(int)CurrentLevel];

        ApplyLevelColor();
    }

    // Full reset (if you still want a button that makes it brand new)
    public void CleanAll()
    {
        usageCount = 0;
        CurrentLevel = DirtLevel.Clean;
        ApplyLevelColor();
    }

    void RecomputeLevelFromUsage()
    {
        // Determine highest level whose threshold <= usageCount
        int level = 0;
        for (int i = 0; i < thresholds.Length; i++)
        {
            if (usageCount >= thresholds[i]) level = i;
        }
        CurrentLevel = (DirtLevel)Mathf.Clamp(level, 0, thresholds.Length - 1);
    }

    void ApplyLevelColor()
    {
        if (cookwareImage == null) return;

        switch (CurrentLevel)
        {
            case DirtLevel.Clean: cookwareImage.color = cleanColor; break;
            case DirtLevel.Mild: cookwareImage.color = mildlyDirtyColor; break;
            case DirtLevel.Dirty: cookwareImage.color = dirtyColor; break;
            case DirtLevel.VeryDirty: cookwareImage.color = veryDirtyColor; break;
            case DirtLevel.Extreme: cookwareImage.color = extremeDirtyColor; break;
        }
    }
}

