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
    public int CurrentRound { get; private set; }

    public GameObject TutorialManager;
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (TutorialManager != null)
        {
            if (GameData.HasCompletedTutorial)
                TutorialManager.SetActive(false);
            else
                TutorialManager.SetActive(true);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Make it persistent across scenes
    }
    public void StartSession()
    {
        SessionID = DateTime.Now.Ticks;
        CurrentLevel = GameData.CurrentLevel;
        CurrentRound = GameData.CurrentRound;
        Debug.Log($"New session started. ID: {SessionID}, Level: {CurrentLevel}, Round: {CurrentRound}");
    }
    
    public void GoToNextLevel()
    {   
        GameData.IncrementLevel();
        CurrentLevel = GameData.CurrentLevel;
        Debug.Log($"Advanced to Level: {CurrentLevel}");
    }
    public void GoToNextRound()
    {   
        GameData.IncrementRound();
        CurrentRound = GameData.CurrentRound;
        Debug.Log($"Advanced to Round: {CurrentRound}");
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
        SceneManager.LoadScene("KitchenScene");
    }

    public void EndLevel()
    {
        // Load Review scene
        SceneManager.LoadScene("ReviewScene");
    }

    public void EndGame()
    {
        // Handle game end logic
        // e.g., player progress, score tracking, etc.
        IsGameInProgress = false;
        GameData.ResetGameData();
        Debug.Log("Game Over!");
        SceneManager.LoadScene("IntroScene");
    }
    // e.g., player progress, score tracking, etc.
}
