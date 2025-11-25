using UnityEngine;
using TMPro;

public class RevenueSystem : MonoBehaviour
{
    public static RevenueSystem Instance; // Singleton for easy access

    [Header("UI References")]
    public TextMeshProUGUI moneyText;

    [Header("Settings")]
    public int startingMoney = 10;

    [Header("Star Rewards")]
    public int threeStarReward = 10;
    public int twoStarReward = 5;
    public int oneStarReward = 1;
    public int zeroStarReward = 0;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private int currentMoney;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //persist across scenes - customer scene needs access to money
        DontDestroyOnLoad(gameObject); 

        // Load saved money or use starting amount
        //currentMoney = PlayerPrefs.GetInt("PlayerMoney", startingMoney);
        currentMoney = startingMoney; // Temporarily disable saving for testing
    }

    private void Start()
    {

        FindMoneyUI();
        UpdateMoneyUI();

        if (enableDebugLogs)
            Debug.Log($"[RevenueSystem] Starting money: ${currentMoney}");
    }

    private void OnEnable()
    {
        // Re-find UI when scene changes (in case we're in DontDestroyOnLoad)
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindMoneyUI();
        UpdateMoneyUI();
    }

    /// <summary>
    /// Finds the money text UI in the current scene
    /// </summary>
    private void FindMoneyUI()
    {
        if (moneyText == null)
        {
            // Try to find by tag or name
            GameObject moneyUIObject = GameObject.Find("MoneyText"); // Change to your object's name
            if (moneyUIObject != null)
            {
                moneyText = moneyUIObject.GetComponent<TextMeshProUGUI>();

                if (enableDebugLogs && moneyText != null)
                    Debug.Log("[RevenueSystem] Found MoneyText UI");
            }
            else if (enableDebugLogs)
            {
                Debug.LogWarning("[RevenueSystem] Could not find MoneyText in scene");
            }
        }
    }

    /// <summary>
    /// Awards money based on star rating from the rating system
    /// </summary>
    public void AddRevenue(int stars)
    {
        int moneyEarned = 0;

        switch (stars)
        {
            case 3:
                moneyEarned = threeStarReward;
                break;
            case 2:
                moneyEarned = twoStarReward;
                break;
            case 1:
                moneyEarned = oneStarReward;
                break;
            case 0:
            default:
                moneyEarned = zeroStarReward;
                break;
        }

        currentMoney += moneyEarned;

        if (enableDebugLogs)
            Debug.Log($"[RevenueSystem] Earned ${moneyEarned} for {stars} stars. Total: ${currentMoney}");

        UpdateMoneyUI();
        SaveMoney();
    }

    /// <summary>
    /// Attempts to spend money. Returns true if successful, false if not enough money
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;

            if (enableDebugLogs)
                Debug.Log($"[RevenueSystem] Spent ${amount}. Remaining: ${currentMoney}");

            UpdateMoneyUI();
            SaveMoney();
            return true;
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning($"[RevenueSystem] Not enough money! Need ${amount}, have ${currentMoney}");

            return false;
        }
    }

    /// <summary>
    /// Returns the current amount of money the player has
    /// </summary>
    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// Checks if player can afford a purchase
    /// </summary>
    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"${currentMoney}";
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", currentMoney);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Resets money to starting amount (useful for testing)
    /// </summary>
    public void ResetMoney()
    {
        currentMoney = startingMoney;
        UpdateMoneyUI();
        SaveMoney();

        if (enableDebugLogs)
            Debug.Log($"[RevenueSystem] Money reset to ${startingMoney}");
    }
}