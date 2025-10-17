using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    public bool IsGameInProgress { get; private set; } = false;
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make it persistent across scenes
    }

    // --- SCENE MANAGEMENT ---
    public void StartGame()
    {
        // Load Kitchen scene (replace with your actual scene name)
        if (IsGameInProgress) return; // Prevent double start
        IsGameInProgress = true;
        GameData.ResetGameData();

        SceneManager.LoadScene("CustomerScene");
    }

    public void GoToKitchen()
    {
        // Load Kitchen scene
        SceneManager.LoadScene("Kitchen");
    }

    public void EndGame()
    {
        // Handle game end logic
        // e.g., player progress, score tracking, etc.
        IsGameInProgress = false;
        GameData.ResetGameData();
        Debug.Log("Game Over!");
        SceneManager.LoadScene("Kitchen");
    }
    // e.g., player progress, score tracking, etc.
}
