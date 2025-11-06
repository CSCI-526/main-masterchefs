using UnityEngine;


// Storing the order data
public static class GameData
{
    private static int currentDishId = 0;
    private static int currentLevel = 0;

    public static int CurrentDishId { get => currentDishId; set => currentDishId = value; }
    public static int CurrentLevel { get => currentLevel; set => currentLevel = value; }

    public static void ResetGameData()
    {
        CurrentDishId = -1;
        currentLevel = 0;
    }

    public static void IncrementLevel()
    {
        currentLevel++;
    }
}



