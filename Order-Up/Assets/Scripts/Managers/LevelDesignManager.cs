using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelDesignManager : MonoBehaviour
{

    [Header("Level Data Settings")]
    public List<LevelData> levels;

    [Header("Pantry Grid")]
    public Transform pantryGrid; // Parent object that contains PantryIngredient slots

    [Header("Debug Settings")]
    [SerializeField] bool enableDebugLogs = false;

    [Header("Pantry")]
    private PantryIngredient[] pantrySlots;

    private void Start()
    {
        pantrySlots = pantryGrid.GetComponentsInChildren<PantryIngredient>(true);
        LoadLevel(GameData.currentLevel);
    }

    private void Update()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[LevelDesignManager] Current Level: {GameData.currentLevel}");
        }
    }

    public void LoadLevel(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, levels.Count - 1);
        LevelData data = levels[index];

        Debug.Log($"[LevelDesignManager] Loading level {level}: {data.levelName}");

        // Step 1: Disable all pantry slots
        foreach (var slot in pantrySlots)
        {
            slot.gameObject.SetActive(false);
        }

        // Step 2: Enable only the slots corresponding to this level’s ingredients
        for (int i = 0; i < data.activeIngredients.Length && i < pantrySlots.Length; i++)
        {
            var slot = pantrySlots[i];
            slot.ingredientPrefab = data.activeIngredients[i];
            slot.gameObject.SetActive(true);
            slot.RefreshSlot();
        }
    }

    public void OnRecipeComplete()
    {
        GameData.IncrementLevel();
        LoadLevel(GameData.currentLevel);
    }
}