using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HintSystem : MonoBehaviour
{
    [Header("UI References")]
    public Button hintButton;               // button to use a hint
    public GameObject hintPopup;            // popup window
    public TextMeshProUGUI hintPopupText;   // text inside popup
    public TextMeshProUGUI hintsLeftText;   // text showing hints left

    [Header("Settings")]
    public int maxHintsPerRound = 3; // hints per round 
    public int freeHintsPerLevel = 3; // free hints per level
    public int hintCost = 5;          // cost after free hints

    private HintDatabase hintDB; // hints from json file
    private int currentRecipeID = 0; // to track the ID of the recipe
    private int levelHintsUsed = 0;  // total hints used this level

    private void Awake()
    {
        LoadHints();
    }

    private void Start()
    {
        // connect hint button
        if (hintButton != null)
            hintButton.onClick.AddListener(ShowNextHint);

        UpdateHintsLeftUI();
        hintPopup.SetActive(false); // hide popup at start
    }

    private void LoadHints()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("hints"); // load in hints from json file

        if (jsonFile == null)
        {
            Debug.LogError("json not found");
            return;
        }

        hintDB = JsonUtility.FromJson<HintDatabase>(jsonFile.text);
    }

    public void SetCurrentRecipe(int recipeID)
    {
        currentRecipeID = recipeID;
        hintPopup.SetActive(false);
    }

    public void ResetFreeHintsForLevel() // reset used hints for level
    {
        levelHintsUsed = 0;
        UpdateHintsLeftUI();
        hintPopup.SetActive(false);
    }

    private int GetHintCost()
    {
        if (levelHintsUsed < freeHintsPerLevel)
            return 0;

        return hintCost;
    }

    public void ShowNextHint()
    {
        int cost = GetHintCost();

        if (cost > 0 && RevenueSystem.totalCoins < cost)
        {
            hintPopupText.text = "Not enough coins!";
            hintPopup.SetActive(true);
            return;
        }

        RecipeHint r = hintDB.recipes.FirstOrDefault(rec => rec.id == currentRecipeID); // to access recipe in json

        if (r == null) // no recipe found
        {
            hintPopupText.text = "Recipe not found";
            hintPopup.SetActive(true);
            return;
        }

        int hintIndex = Mathf.Min(levelHintsUsed, r.hints.Length - 1);

        if (hintIndex < r.hints.Length) // if there are hints available
        {
            hintPopupText.text = r.hints[hintIndex];  // show hints in different indices
            hintPopup.SetActive(true);
        }

        if (cost > 0)
        {
            RevenueSystem.totalCoins -= cost;
            PlayerPrefs.SetInt("TotalCoins", RevenueSystem.totalCoins);
            PlayerPrefs.Save();
        }

        levelHintsUsed++;
        UpdateHintsLeftUI();
    }

    public void CloseHintPopup() // to have button to close popup
    {
        hintPopup.SetActive(false);
    }

    private void UpdateHintsLeftUI() // to show how many hints left
    {
        int left = Mathf.Max(0, freeHintsPerLevel - levelHintsUsed);
        hintsLeftText.text = "Hints Left: " + left;
    }
}
