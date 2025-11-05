using UnityEngine;


// Storing the order data
public static class GameData
{
    public static int currentDishId = 0;
    public static int currentLevel = 1;

    public static void ResetGameData()
    {
        currentDishId = -1;
        currentLevel = 0;
    }

    public static void IncrementLevel()
    {
        currentLevel++;
    }
}



