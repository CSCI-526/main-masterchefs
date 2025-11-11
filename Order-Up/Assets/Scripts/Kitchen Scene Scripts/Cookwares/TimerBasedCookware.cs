using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Cookware that uses a timer slider (Oven, Air Fryer)
/// </summary>
public class TimerBasedCookware : BaseCookware
{
    [Header("Timer UI")]
    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private Slider cookingTimeSlider;

    [Header("Timer Settings")]
    [SerializeField] private float minCookingTime = 1f;
    [SerializeField] private float maxCookingTime = 10f;
    private float selectedCookingTime = 5f;

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
            cookingTimeSlider.minValue = minCookingTime;
            cookingTimeSlider.maxValue = maxCookingTime;
            cookingTimeSlider.value = selectedCookingTime;
            cookingTimeSlider.interactable = false;

            cookingTimeSlider.onValueChanged.AddListener(OnSliderValueChanged);

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Slider initialized: min={minCookingTime}, max={maxCookingTime}");
            }
        }

        UpdateTimerDisplay();
    }

    protected override void UpdateCookingLogic()
    {
        currentCookingTime += Time.deltaTime;
        UpdateTimerDisplay();

        if (currentCookingTime >= selectedCookingTime)
        {
            FinishCooking();
        }
    }

    protected override void OnIngredientEntered(GameObject ingredient)
    {
        Debug.Log($"[{cookwareName}] OnIngredientEntered called.");
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient entered: {ingredient.name}");
        }

        UpdateSliderState();
    }

    protected override void OnIngredientExited(GameObject ingredient)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient exited: {ingredient.name}");
        }

        UpdateSliderState();
    }

    private void OnSliderValueChanged(float value)
    {
        selectedCookingTime = value;

        if (!isCooking)
        {
            UpdateTimerDisplay();
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
            cookingTimeSlider.interactable = hasIngredient && !isCooking;

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
        UpdateTimerDisplay();
    }

    protected override void FinishCooking()
    {
        base.FinishCooking();
        UpdateSliderState();
        UpdateTimerDisplay();
    }

    public float GetSelectedCookingTime() => selectedCookingTime;
}