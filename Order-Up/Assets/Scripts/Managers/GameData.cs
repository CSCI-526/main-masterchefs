using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// Storing the order data
public static class GameData
{
    private static int currentDishId = 0;
    private static int currentLevel = 0;
    private static int currentRound = 0;
    private static int currentDialogId = 0;
    private static GameObject currentRecipe = null;
    public static List<LevelData> allLevels;
    public static int numberToSkipToReview = 0;
    private static int totalCoin = 0;

    public static int CurrentDishId { get => currentDishId; set => currentDishId = value; }
    public static int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public static int CurrentRound { get => currentRound; set => currentRound = value; }
    public static int CurrentDialogId { get => currentDialogId; set => currentDialogId = value; }
    public static GameObject CurrentRecipe { get => currentRecipe; set => currentRecipe = value; }
    public static List<LevelData> AllLevels { get => allLevels; set => allLevels = value; }
    public static int TotalCoin { get => totalCoin; set => totalCoin = value; }

    public static void ResetGameData()
    {
        CurrentDishId = -1;
        currentLevel = 1;
        currentRound = 0;
        CurrentRecipe = null;
    }

    public static void IncrementLevel()
    {
        Debug.Log($"Level INCREMENTED!: {CurrentLevel} " + new System.Diagnostics.StackTrace());
        currentLevel++;
        currentRound = 0;
    }
    public static void IncrementRound()
    {
        Debug.Log("Round INCREMENTED!: " + new System.Diagnostics.StackTrace());
        currentRound++;
    }

    public static bool HasCompletedTutorial
    {
        get
        {
            return PlayerPrefs.GetInt("HasCompletedTutorial", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("HasCompletedTutorial", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}



