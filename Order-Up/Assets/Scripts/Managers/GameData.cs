using UnityEngine;


// Storing the order data
public static class GameData
{
    private static int currentDishId = 0;
    private static int currentLevel = 1;
    private static int currentRound = 0;
    private static GameObject currentRecipe = null;

    public static int CurrentDishId { get => currentDishId; set => currentDishId = value; }
    public static int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public static int CurrentRound { get => currentRound; set => currentRound = value; }
    public static GameObject CurrentRecipe { get => currentRecipe; set => currentRecipe = value; }

    public static void ResetGameData()
    {
        CurrentDishId = -1;
        currentLevel = 1;
        currentRound = 0;
        CurrentRecipe = null;
    }

    public static void IncrementLevel()
    {
        currentLevel++;
        currentRound = 0;
    }
    public static void IncrementRound()
    {
        currentRound++;
    }
}



