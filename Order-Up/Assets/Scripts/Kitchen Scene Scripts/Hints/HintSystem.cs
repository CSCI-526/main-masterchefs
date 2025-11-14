using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HintSystem : MonoBehaviour
{
    [Header("UI References")]
    public Button hintButton;               // button to use a hint
    public GameObject hintPopup;            // popup window
    public Button closeHintButton;
    public TextMeshProUGUI hintPopupText;   // text inside popup
    public TextMeshProUGUI hintsLeftText;   // text showing hints left

    [Header("Settings")]
    public int maxHintsPerRound = 3; // hints per round

    private HintDatabase hintDB; // hints from json file
    private int currentRecipeID = 0; // to track the ID of the recipe
    private int hintsUsed = 0; // to track hints player has used on round

    private void Awake()
    {
        LoadHints();
    }

    private void Start()
    {
        // connect hint button
        if (hintButton != null)
            hintButton.onClick.AddListener(ShowNextHint);
        if (closeHintButton != null)
            closeHintButton.onClick.AddListener(CloseHintPopup);

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
        ResetHintCount();
    }

    private void ResetHintCount() // reset used hints for round
    {
        hintsUsed = 0;
        UpdateHintsLeftUI();
        hintPopup.SetActive(false);
    }

    public void ShowNextHint()
    {
        if (hintsUsed >= maxHintsPerRound)
        {
            hintPopupText.text = "No more hints available";
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

        if (hintsUsed < r.hints.Length) // if there are hints available
        {
            hintPopupText.text = r.hints[hintsUsed];  // show hints in different indices
            hintPopup.SetActive(true);
        }

        hintsUsed++; 
        UpdateHintsLeftUI();
    }

    public void CloseHintPopup() // to have button to close popup
    {
        hintPopup.SetActive(false);
    }

    private void UpdateHintsLeftUI() // to show how many hints left
    {
        int left = maxHintsPerRound - hintsUsed;
        hintsLeftText.text = "Hints Left: " + left;
    }
}
