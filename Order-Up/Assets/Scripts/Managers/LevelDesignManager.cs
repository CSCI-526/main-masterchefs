using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesignManager : MonoBehaviour
{
    [Header("Level Data Settings")]
    public List<LevelData> levels;

    [Header("Game Data")]
    public int Level;
    public int Round;
    public GameObject currRecipe;

    [Header("Pantry Grid")]
    public Transform pantryGrid;

    [Header("Debug Settings")]
    [SerializeField] bool enableDebugLogs = false;

    private PantryIngredient[] pantrySlots;

    private void Start()
    {
        Level = GameData.CurrentLevel;
        Round = GameData.CurrentRound;
        currRecipe = null;
        pantrySlots = pantryGrid.GetComponentsInChildren<PantryIngredient>(true);
        StartCoroutine(LoadLevelWithLayoutDelay(GameData.CurrentLevel));
    }

    private void Update()
    {
        if (enableDebugLogs)
            Debug.Log($"[LevelDesignManager] Current Level: {GameData.CurrentLevel}, Current Round: {GameData.CurrentRound}");
    }

    private IEnumerator LoadLevelWithLayoutDelay(int level)
    {
        LoadLevel(level);

        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return new WaitForEndOfFrame();

        foreach (var slot in pantrySlots)
        {
            if (slot.gameObject.activeSelf)
            {
                slot.RefreshSlot();
            }
        }

        if (enableDebugLogs)
            Debug.Log($"[LevelDesignManager] Finished layout update, pantry refreshed for Level {level}");
    }

    public void LoadLevel(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, levels.Count - 1);
        LevelData data = levels[index];

        if (enableDebugLogs)
            Debug.Log($"[LevelDesignManager] Loading level {level}: {data.levelName}");

        // Disable all slots first
        foreach (var slot in pantrySlots)
            slot.gameObject.SetActive(false);

        // Enable only required slots
        for (int i = 0; i < data.activeIngredients.Length && i < pantrySlots.Length; i++)
        {
            var slot = pantrySlots[i];
            slot.ingredientPrefab = data.activeIngredients[i];
            slot.gameObject.SetActive(true);
        }
    }

    public void LoadRound(int level, int round)
    {
        int index = Mathf.Clamp(level - 1, 0, levels.Count - 1);
        LevelData data = levels[index];

        if (enableDebugLogs)
            Debug.Log($"[LevelDesignManager] Loading level {level}, round {round}");

        // Set recipe
        currRecipe = data.recipes[round];
    }

    public void OnRecipeComplete()
    {
        GameData.IncrementRound();


        GameData.IncrementLevel();
        StartCoroutine(LoadLevelWithLayoutDelay(GameData.CurrentLevel));
    }
}
