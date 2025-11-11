using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Cookware that requires stirring (Pot)
/// </summary>
public class StirBasedCookware : BaseCookware
{
    [Header("Progress UI")]
    [SerializeField] private Slider progressBar;

    [Header("Cooking Settings")]
    [SerializeField] private float properCookingTime = 5f;
    [SerializeField] private float maxCookingTime = 10f;

    private bool isStirring = false;

    protected override void InitializeUI()
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = properCookingTime;
            progressBar.value = 0f;
        }

        UpdateTimerDisplay();
    }

    protected override void UpdateCookingLogic()
    {
        // Only progress time if stirring
        if (isStirring)
        {
            currentCookingTime += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp(currentCookingTime, 0f, properCookingTime);
            }
        }

        UpdateTimerDisplay();

        // Check if overcooked
        if (currentCookingTime >= maxCookingTime)
        {
            OvercookIngredient();
            StopCooking();
        }
        // Check if properly cooked
        else if (currentCookingTime >= properCookingTime)
        {
            FinishCooking();
        }
    }

    protected override void OnIngredientEntered(GameObject ingredient)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient entered - starting cooking");
        }

        StartCooking();
    }

    protected override void OnIngredientExited(GameObject ingredient)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient exited");
        }

        StopCooking();
    }

    private void UpdateTimerDisplay()
    {
        if (timerDisplayText != null)
        {
            if (isCooking)
            {
                timerDisplayText.text = $"{currentCookingTime:F1}s";
            }
            else
            {
                timerDisplayText.text = $"Cook Time: {currentCookingTime:F1}s";
            }
        }
    }

    public void StartStirring()
    {
        if (!isCooking) return;
        isStirring = true;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Started stirring");
        }
    }

    public void StopStirring()
    {
        isStirring = false;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Stopped stirring");
        }
    }

    public override void StopCooking()
    {
        base.StopCooking();

        isStirring = false;

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        UpdateTimerDisplay();
    }

    protected override void FinishCooking()
    {
        base.FinishCooking();

        isStirring = false;

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }
    }

    private void OvercookIngredient()
    {
        if (ingredientInside == null) return;

        Ingredient ing = ingredientInside.GetComponent<Ingredient>();
        if (ing != null && ing.currentState == IngredientState.Cooked)
        {
            ing.OvercookIngredient();
            if (ing.ingredientData.overcookedResult != null)
            {
                ing.ingredientData = ing.ingredientData.overcookedResult;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} Overcooked");
            }
        }
    }

    public bool IsStirring() => isStirring;
    public float GetProperCookingTime() => properCookingTime;
    public float GetMaxCookingTime() => maxCookingTime;
}