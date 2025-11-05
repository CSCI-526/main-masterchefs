using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class KitchenManager : MonoBehaviour
{
    [Header("Order Window Settings")]
    // The array to hold your dish prefabs. 
    public GameObject[] dishPrefabs;
    public Transform orderWindow;
    public TMPro.TMP_Text dishNameText;

    private int currentDishId;
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = false;

    // Play/Customize Button
    [Header("Play/Customize Button")]
    public Button playButton;
    public Button customizeButton;
    public Button submitButton;
    void Start()
    {
        // retrieve currentDishId
        currentDishId = GameData.currentDishId; // todo: uncomment
        Debug.Log("Current Dish ID in KitchenManager: " + currentDishId);
        if (currentDishId >= 0 && currentDishId < dishPrefabs.Length)
            DisplayDishPrefab(currentDishId);
        else
            Debug.LogError("Invalid Dish ID: " + currentDishId +
                           ". Make sure the ID is within the range of the dishPrefabs array size.");

        // Add button listeners
        playButton.onClick.AddListener(OnPlayClicked);
        if (customizeButton != null)
            customizeButton.onClick.AddListener(OnCustomizeClicked);

        UpdatePlayButtonState();
        //GameData.CheckAndIncrementLevel();
    }


    private void DisplayDishPrefab(int dishID)
    {
        // Use the dishID as the index to get the correct prefab
        GameObject dishPrefab = dishPrefabs[dishID];
        dishNameText.text = dishPrefab.name;
        // Instantiate (create) the prefab in the scene
        Instantiate(dishPrefab, orderWindow.position, Quaternion.identity);
        if (enableDebugLogs) Debug.Log("Successfully loaded dish: " + dishPrefab.name + " (ID: " + dishID + ")");
    }

    // call the game manager start game
    public void OnPlayClicked()
    {
        GameManager.Instance.StartGame();
    }
    public void OnCustomizeClicked()
    {
        // for now debug log the click 
        Debug.Log("Customize Button Clicked!");
    }

    private void UpdatePlayButtonState()
    {
        if (GameManager.Instance == null)
            Debug.LogError("GameManager.Instance is NULL!");
        if (playButton == null)
            Debug.LogError("playButton is NULL!");
        if (submitButton == null)
            Debug.LogError("submitButton is NULL!");

        playButton.gameObject.SetActive(!GameManager.Instance.IsGameInProgress);
        submitButton.gameObject.SetActive(GameManager.Instance.IsGameInProgress);
    }
}
