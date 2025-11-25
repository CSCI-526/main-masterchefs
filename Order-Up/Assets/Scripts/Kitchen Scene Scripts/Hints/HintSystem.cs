using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintSystem : MonoBehaviour
{
    [Header("UI References")]
    public Button hintButton;               // Button to request a hint
    public GameObject hintPanel;            // Panel that shows the hint
    public TextMeshProUGUI hintText;        // Text that displays the hint
    public Button closeButton;              // Button to close the panel

    [Header("Settings")]
    public int hintCost = 3;                // Cost per hint in dollars

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private HintDatabase hintDB;
    private int currentHintIndex = 0;       // Tracks which hint to show next (0, 1, or 2)
    private List<string> purchasedHints;


    private void Awake()
    {
        LoadHints();
    }

    private void Start()
    {
        // Connect buttons
        if (hintButton != null)
            hintButton.onClick.AddListener(OnHintButtonClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseHintPanel);

        // Hide panel at start
        if (hintPanel != null)
            hintPanel.SetActive(false);

        purchasedHints = new List<string>();

        ResetForNewRecipe();
    }

    private void LoadHints()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("hints");

        if (jsonFile == null)
        {
            Debug.LogError("[HintSystem] hints.json not found in Resources folder!");
            return;
        }

        hintDB = JsonUtility.FromJson<HintDatabase>(jsonFile.text);

        if (enableDebugLogs)
            Debug.Log($"[HintSystem] Loaded {hintDB.recipes.Length} recipes with hints");
    }

    /// <summary>
    /// Call this when starting a new recipe/round
    /// </summary>
    public void ResetForNewRecipe()
    {
        currentHintIndex = 0;
        purchasedHints.Clear();
        if (hintPanel != null)
            hintPanel.SetActive(false);

        if (enableDebugLogs)
            Debug.Log("[HintSystem] Reset for new recipe");
    }

    private void OnHintButtonClicked()
    {
        // Get the current dish ID from GameData
        int currentDishId = GameData.CurrentDishId;

        if (enableDebugLogs)
            Debug.Log($"[HintSystem] Hint requested for Dish ID: {currentDishId}");

        // Find the recipe with matching ID
        RecipeHint recipe = hintDB.recipes.FirstOrDefault(r => r.id == currentDishId);

        if (recipe == null)
        {
            if (enableDebugLogs)
                Debug.LogError($"[HintSystem] No hints found for Dish ID: {currentDishId}");

            ShowHintPanel("No hints available for this recipe.");
            return;
        }

        // Check if we've run out of hints
        if (currentHintIndex >= recipe.hints.Length)
        {
            if (enableDebugLogs)
                Debug.Log("[HintSystem] No more hints available");

            string combinedHints = string.Join("\n\n", purchasedHints);
            ShowHintPanel(combinedHints);
            return;
        }

        // Check if player can afford the hint
        if (RevenueSystem.Instance != null)
        {
            if (!RevenueSystem.Instance.CanAfford(hintCost))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[HintSystem] Player cannot afford hint. Cost: ${hintCost}, Has: ${RevenueSystem.Instance.GetCurrentMoney()}");

                ShowHintPanel($"Not enough money! Hints cost ${hintCost}.");
                return;
            }

            // Charge the player
            if (RevenueSystem.Instance.SpendMoney(hintCost))
            {
                // Show the hint
                string hint = recipe.hints[currentHintIndex];
                string hintMessage = $"Hint {currentHintIndex + 1}: {hint}";
                purchasedHints.Add(hint);
                string combinedHints = string.Join("\n\n", purchasedHints);

                ShowHintPanel(combinedHints);
                if (enableDebugLogs)
                    Debug.Log($"[HintSystem] Showing hint {currentHintIndex + 1}/{recipe.hints.Length}: {hint}");

                currentHintIndex++;
            }
        }
        else
        {
            Debug.LogError("[HintSystem] RevenueSystem.Instance not found!");

            // Show hint anyway for testing
            string hint = recipe.hints[currentHintIndex];
            ShowHintPanel(hint);
            currentHintIndex++;
        }
    }

    private void ShowHintPanel(string message)
    {
        if (hintText != null)
            hintText.text = message;

        if (hintPanel != null)
            hintPanel.SetActive(true);
    }

    public void CloseHintPanel()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }
}