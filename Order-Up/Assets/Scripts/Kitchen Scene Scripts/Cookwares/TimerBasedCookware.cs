using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Cookware that uses a timer slider (Oven, Air Fryer)
/// </summary>
/// <upgrade>
/// maxCookingTime decrements from 15 to 10 to 5 seconds as appliance is upgraded
/// </upgrade>
public class TimerBasedCookware : BaseCookware
{
    [Header("UI")]
    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private Slider cookingTimeSlider; // from timer to progress bar

    [Header("Timer Settings")]
    [SerializeField] private float minCookingTime = 5f;
    [SerializeField] private float maxCookingTime = 10f;
    private float selectedCookingTime = 10f; // default value, can decrement if they upgrade the appliance

    protected override void Start()
    {
        base.Start();

        if (sliderPanel != null)
        {
            sliderPanel.SetActive(true);
        }
    }

    protected override void InitializeUI()
    {
        if (cookingTimeSlider != null)
        {
            cookingTimeSlider.minValue = 0f;
            cookingTimeSlider.maxValue = maxCookingTime;
            cookingTimeSlider.value = 0f;
            cookingTimeSlider.interactable = false;

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Slider initialized: min={minCookingTime}, max={maxCookingTime}");
            }
        }

        //UpdateTimerDisplay();
    }

    protected override void UpdateCookingLogic()
    {
        currentCookingTime += Time.deltaTime;
        //Debug.Log($"[{cookwareName}] Cooking time updated: {currentCookingTime:F2}s / {selectedCookingTime:F2}s");
        // only show the slider progress for now
        //UpdateTimerDisplay();

        // Update progress bar
        if (cookingTimeSlider != null)
        {
            cookingTimeSlider.value = Mathf.Clamp(currentCookingTime, 0f, selectedCookingTime);
        }

        if (currentCookingTime >= selectedCookingTime)
        {
            FinishCooking();
        }
    }

    protected override void OnIngredientEntered(GameObject ingredient)
    {
        //Debug.Log($"[{cookwareName}] OnIngredientEntered called.");
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient entered: {ingredient.name}");
        }
    }

    protected override void OnIngredientExited(GameObject ingredient)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient exited: {ingredient.name}");
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerDisplayText != null)
        {
            if (isCooking)
            {
                timerDisplayText.text = $"{currentCookingTime:F1}s / {selectedCookingTime:F1}s";
            }
            else
            {
                timerDisplayText.text = $"Cook Time: {selectedCookingTime:F1}s";
            }
        }
    }

    private void UpdateSliderState()
    {
        Debug.Log($"[{cookwareName}] Updating slider state. Ingredient inside: {(ingredientInside)}, Is cooking: {isCooking}");
        if (cookingTimeSlider != null)
        {
            bool hasIngredient = ingredientInside != null;

            // Visual feedback
            ColorBlock colors = cookingTimeSlider.colors;
            if (!hasIngredient)
            {
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            cookingTimeSlider.colors = colors;
        }
    }

    // Toggle slider panel on click
    private void OnMouseDown()
    {
        if (sliderPanel != null)
        {
            sliderPanel.SetActive(!sliderPanel.activeSelf);

            if (sliderPanel.activeSelf)
            {
                UpdateSliderState();
            }
        }
    }

    public override void StartCooking()
    {
        base.StartCooking();

        if (cookingTimeSlider != null)
        {
            cookingTimeSlider.interactable = false;
        }
    }

    public override void StopCooking()
    {
        base.StopCooking();
        UpdateSliderState();
        //UpdateTimerDisplay();
    }

    protected override void FinishCooking()
    {
        base.FinishCooking();
        UpdateSliderState();
        //UpdateTimerDisplay();
    }

    public float GetSelectedCookingTime() => selectedCookingTime;
}