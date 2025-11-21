using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}