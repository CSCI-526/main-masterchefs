using UnityEngine;

public class LevelDesignManager : MonoBehaviour
{
    [Header("Level Settings")]
    public LevelData[] levels;

    private int currentLevelIndex;

    void Start()
    {
        LoadLevel(GameData.currentLevel);
    }

    public void LoadLevel(int level)
    {
        // Clamp level to available range
        currentLevelIndex = Mathf.Clamp(level - 1, 0, levels.Length - 1);
        LevelData current = levels[currentLevelIndex];

        // Disable everything first
        DisableAllObjects();

        // Enable only the current level’s objects
        foreach (GameObject obj in current.activeCookwares)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in current.activeIngredients)
            if (obj != null) obj.SetActive(true);

        Debug.Log($"[LevelDesignManager] Loaded Level {level}: {current.levelName}");
    }

    private void DisableAllObjects()
    {
        
    }

    // You can call this when player finishes a recipe
    public void OnRecipeComplete()
    {
        GameData.IncrementLevel();
        LoadLevel(GameData.currentLevel);
    }
}
