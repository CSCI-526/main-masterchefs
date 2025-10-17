using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    public bool IsGameInProgress { get; private set; } = false;
    public long SessionID { get; private set; }
    public int CurrentLevel { get; private set; }
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
    public void StartSession()
    {
        SessionID = DateTime.Now.Ticks;
        CurrentLevel = 1;
        Debug.Log($"New session started. ID: {SessionID}, Level: {CurrentLevel}");
    }
    
    public void GoToNextLevel()
    {   
        Debug.Log($"Advanced to Level: {CurrentLevel}");
        CurrentLevel++;
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
