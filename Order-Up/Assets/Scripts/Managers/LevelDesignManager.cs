using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Debug.Log($"[LevelDesignManager.Start] Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} | GameData.Level={GameData.CurrentLevel} GameData.Round={GameData.CurrentRound}");

        int levelIndex = Mathf.Clamp(Level - 1, 0, levels.Count - 1);
        LevelData data = levels[levelIndex];
        bool roundIsValid = Round < data.recipes.Length;

        if (roundIsValid)
        {
            // normal behavior
            Debug.Log($"[LevelDesignManager.Start] Scene: Reload with level={GameData.CurrentLevel} GameData.Round={GameData.CurrentRound}");

            StartCoroutine(LoadLevelWithLayoutDelay(GameData.CurrentLevel));
        }
        else
        {
            // round was completed → go straight to ReviewScene
            SceneManager.LoadScene("ReviewScene");
            return;
    }

    GameData.allLevels = levels;
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
        Debug.Log($"[LevelDesignManager] Finished layout update, pantry refreshed for Level {level}");
    }

    public void LoadLevel(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, levels.Count - 1);
        LevelData data = levels[index];

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

        Debug.Log($"[LevelDesignManager] Loading level {level}, round {round}");

        // Get new recipe
        currRecipe = data.recipes[round];
        GameData.CurrentRecipe = currRecipe;

        LoadLevel(level);
    }

    public void OnRecipeComplete()
    {
        int levelIndex = GameData.CurrentLevel - 1;
        LevelData data = levels[levelIndex];

        GameData.IncrementRound();
        Debug.Log($"[LevelDesignManager] Completed level {Level}, round {Round}");

        // If still more recipes, load next round
        if (GameData.CurrentRound < data.recipes.Length)
        {
            LoadRound(GameData.CurrentLevel, GameData.CurrentRound);
            return;
        }

        // LEVEL COMPLETE → Go to Review Scene
        SceneManager.LoadScene("ReviewScene");
        // StartCoroutine(LoadLevelWithLayoutDelay(GameData.CurrentLevel));
    }
}
