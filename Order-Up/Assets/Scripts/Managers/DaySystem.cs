using UnityEngine;
using System.Collections;
using TMPro;

public class DaySystem : MonoBehaviour
{
    [Header("Day Settings")]
    public bool enableDebugLogs = true;
    public GameObject dayPanel;
    public TextMeshProUGUI dayTitleText;
    public TextMeshProUGUI unlockedIngredientsText;
    public float displayDuration = 5f;

    public void ShowDaySummary(int level)
    {
        StartCoroutine(Display(level));
    }
    
    private IEnumerator Display(int level)
    {
        // 1. Update text based on the day
        dayTitleText.text = $"Day {level} Complete!";
        unlockedIngredientsText.text = $"Day {level} Complete!";

        string unlockedText = "New Ingredients Unlocked: \n";
        // if (data != null && data.activeIngredients != null && data.activeIngredients.Length > 0)
        // {
        //     foreach (var ingredient in data.activeIngredients)
        //     {
        //         if (ingredient != null)
        //             unlockedText += $"• {ingredient.name}\n";
        //     }
        // }
        // else
        // {
        //     unlockedText += "(No new ingredients)";
        // }
        // unlockedIngredientsText.text = unlockedText;

        // 2. Show the panel
        dayPanel.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        // 3. Hide popup
        dayPanel.SetActive(false);
    }

}