using UnityEngine;


// Storing the order data
public static class GameData
{
    public static int currentDishId = -1;
    public static int currentLevel = 1;

    public static void ResetGameData()
    {
        currentDishId = -1;
        currentLevel = 0;
        Debug.Log("[GameData] Game data reset.");
    }

    public static void CheckAndIncrementLevel()
    {
        if (currentDishId == 2 || currentDishId == 5 || currentDishId == 8)
        {
            currentLevel++;
            Debug.Log($"[GameData] Level increased to: {currentLevel}");
        }
    }
}



