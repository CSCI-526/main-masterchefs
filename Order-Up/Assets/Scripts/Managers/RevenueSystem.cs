using UnityEngine;
using TMPro;

public class RevenueSystem : MonoBehaviour
{
    public static int totalCoins = 0;
    public TextMeshProUGUI coinText;
    public bool enableDebugLogs = true;

    private void Start()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0); // to load player's coin count
        UpdateCoinText();
    }

    public void AddRevenue(int stars) //to add coins based on stars
    {
        int coinsEarned = 0;

        switch (stars)
        {
            case 3:
                coinsEarned = 30; // 30 for 3 star
                break;
            case 2:
                coinsEarned = 15; // 15 for 2 star
                break;
            case 1:
                coinsEarned = 5;
                break;
            default:
                coinsEarned = 0;
                break;
        }

        totalCoins += coinsEarned;

        UpdateCoinText();
        SaveCoins();
    }
    private void UpdateCoinText()
    {

        if (coinText != null)
            coinText.text = $"Coins: {totalCoins}";
    }
    private void SaveCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
        GameData.TotalCoin = totalCoins;
    }
    public int GetTotalCoins()
    {
        return totalCoins;
    }
}