using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    public TMP_Text moneyText;

    void Update()
    {
        if (RevenueSystem.Instance != null)
        {
            moneyText.text = "Current Money:$" + RevenueSystem.Instance.GetCurrentMoney();
        }
    }
}
