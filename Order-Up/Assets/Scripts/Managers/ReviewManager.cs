using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ReviewSceneManager : MonoBehaviour
{
    [Header("Text Fields")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI roundsText;
    public TextMeshProUGUI coinsText;

    [Header("Containers")]
    public Transform newIngredientsContainer;
    public Transform newCookwareContainer;

    [Header("Prefabs")]
    public GameObject uiListItemPrefab;
    public GameObject takeOrderBtn;

    void Start()
    {
        LoadReviewData();
    }

    private void LoadReviewData()
    {
        int level = GameData.CurrentLevel;
        var levels = GameData.AllLevels;
        Debug.Log($"[ReviewManager] At level {level}. Moving to CustomerScene.");

        LevelData current = levels[level - 1];

        // UI basics haha
        levelText.text = $"Level {level} Complete!";
        roundsText.text = $"Recipes Completed: {current.recipes.Length}";
        coinsText.text = $"Total Coins: {RevenueSystem.Instance.GetCurrentMoney()}";
        // HARDCODE FOR NOW

        // Determine next level data (if level == 1, nothing is new)
        LevelData next = (level < levels.Count) ? levels[level] : null;

        // -------- NEW INGREDIENTS --------
        var newIngredients = GetNewItems(current.activeIngredients, next?.activeIngredients);

        foreach (var ingredient in newIngredients)
        {
            var obj = Instantiate(uiListItemPrefab, newIngredientsContainer);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.name;
        }

        // -------- NEW COOKWARE --------
        var newCookwares = GetNewItems(current.activeCookwares, next?.activeCookwares);

        foreach (var cookware in newCookwares)
        {
            var obj = Instantiate(uiListItemPrefab, newCookwareContainer);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = cookware.name;
        }
    }

    private List<GameObject> GetNewItems(GameObject[] oldItems, GameObject[] newItems)
    {
        List<GameObject> result = new List<GameObject>();

        if (newItems == null)
            return result;

        foreach (var item in newItems)
        {
            bool alreadyHad = false;

            if (oldItems != null)
            {
                foreach (var old in oldItems)
                {
                    if (old == item)
                    {
                        alreadyHad = true;
                        break;
                    }
                }
            }

            if (!alreadyHad)
                result.Add(item);
        }

        return result;
    }


    public void Continue()
    {
        GameData.IncrementLevel();
        SceneManager.LoadScene("CustomerScene");
    }
}