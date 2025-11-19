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

        // UI basics
        levelText.text = $"Level {level} Complete!";
        roundsText.text = $"Recipes Completed: {current.recipes.Length}";
        coinsText.text = $"Total Coins: 0";
        // HARDCODE FOR NOW

        // Determine previous level data (if level == 1, nothing is new)
        LevelData previous = (level > 1) ? levels[level - 2] : null;

        // -------- NEW INGREDIENTS --------
        var newIngredients = GetNewItems(previous?.activeIngredients, current.activeIngredients);

        foreach (var ingredient in newIngredients)
        {
            var obj = Instantiate(uiListItemPrefab, newIngredientsContainer);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = ingredient.name;
        }

        // -------- NEW COOKWARE --------
        var newCookwares = GetNewItems(previous?.activeCookwares, current.activeCookwares);

        foreach (var cookware in newCookwares)
        {
            var obj = Instantiate(uiListItemPrefab, newCookwareContainer);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = cookware.name;
        }
    }

    private List<GameObject> GetNewItems(GameObject[] previous, GameObject[] current)
    {
        List<GameObject> newOnes = new List<GameObject>();

        // No previous level → everything is new
        if (previous == null)
        {
            newOnes.AddRange(current);
            return newOnes;
        }

        foreach (var obj in current)
        {
            bool alreadyHad = false;

            foreach (var prev in previous)
            {
                if (prev == obj)
                {
                    alreadyHad = true;
                    break;
                }
            }

            if (!alreadyHad)
                newOnes.Add(obj);
        }

        return newOnes;
    }

    public void Continue()
    {
        GameData.IncrementLevel();
        SceneManager.LoadScene("CustomerScene");
    }
}