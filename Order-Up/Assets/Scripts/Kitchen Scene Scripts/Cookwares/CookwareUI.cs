using UnityEngine;
using UnityEngine.UI;

public class CookwareUI : MonoBehaviour
{
    [SerializeField] private Cookwares cookware;
    [SerializeField] private Button startCookingButton;
    [SerializeField] private Button stopCookingButton;
    
    void Start()
    {
        // Set up button listeners
        if (startCookingButton != null)
        {
            startCookingButton.onClick.AddListener(OnStartCookingClicked);
        }
        
        if (stopCookingButton != null)
        {
            stopCookingButton.onClick.AddListener(OnStopCookingClicked);
            stopCookingButton.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // Update button states based on cookware state
        if (cookware != null)
        {
            if (startCookingButton != null)
            {
                startCookingButton.interactable = cookware.GetIngredientCount() > 0 && !cookware.IsCooking();
            }
            
            if (stopCookingButton != null)
            {
                stopCookingButton.gameObject.SetActive(cookware.IsCooking());
            }
        }
    }
    
    private void OnStartCookingClicked()
    {
        if (cookware != null)
        {
            cookware.StartCooking();
        }
    }
    
    private void OnStopCookingClicked()
    {
        if (cookware != null)
        {
            cookware.StopCooking();
        }
    }
}
