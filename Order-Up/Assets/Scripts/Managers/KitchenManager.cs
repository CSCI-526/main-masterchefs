using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    // The array to hold your dish prefabs. 
    public GameObject[] dishPrefabs; 
    public Transform orderWindow;

    private int currentDishId;

    void Awake()
    {
        // retrieve currentDishId
        currentDishId = GameData.currentDishId;

        // reset currentDishId
        // GameData.currentDishId = -1;

        if (currentDishId >= 0 && currentDishId < dishPrefabs.Length)
        {
            DisplayDishPrefab(currentDishId);
        }
        else
        {
            Debug.LogError("Invalid Dish ID: " + currentDishId +
                           ". Make sure the ID is within the range of the dishPrefabs array size.");
        }
    }

    private void DisplayDishPrefab(int dishID)
    {
        // Use the dishID as the index to get the correct prefab
        GameObject dishPrefab = dishPrefabs[dishID];

        // Instantiate (create) the prefab in the scene
        Instantiate(dishPrefab, orderWindow.position, Quaternion.identity); 

        Debug.Log("Successfully loaded dish: " + dishPrefab.name + " (ID: " + dishID + ")");
    }
}