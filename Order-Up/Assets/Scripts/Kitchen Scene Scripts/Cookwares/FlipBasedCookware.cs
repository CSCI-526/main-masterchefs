using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Cookware that requires flipping/tossing (Pan)
/// Player holds and moves the pan to flip ingredients
/// </summary>
public class FlipBasedCookware : BaseCookware
{
    [Header("Progress UI")]
    [SerializeField] private Slider progressBar;

    [Header("Cooking Settings")]
    [SerializeField] private float properCookingTime = 4f;
    [SerializeField] private float maxCookingTime = 20f;

    [Header("Flip/Toss Settings")]
    [SerializeField] private float flipAngleThreshold = 45f; // Angle needed to register a flip
    [SerializeField] private float flipCooldown = 0.5f; // Time between flips

    private bool isBeingHeld = false;
    private bool isFlipping = false;
    private float lastFlipTime = 0f;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    protected override void InitializeUI()
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = properCookingTime;
            progressBar.value = 0f;
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;

        UpdateTimerDisplay();
    }

    protected override void UpdateCookingLogic()
    {
        // Only progress time if being flipped
        if (isFlipping)
        {
            currentCookingTime += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp(currentCookingTime, 0f, properCookingTime);
            }
        }

        UpdateTimerDisplay();

        // Check for flipping motion if being held
        if (isBeingHeld)
        {
            CheckForFlipMotion();
        }

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
                timerDisplayText.text = $"Ready";
            }
        }
    }

    private void OnMouseDown()
    {
        isBeingHeld = true;
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Pan grabbed");
        }
    }

    private void OnMouseDrag()
    {
        if (!isBeingHeld) return;

        // Move pan with mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        transform.position = mousePos;
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;
        isFlipping = false;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Pan released");
        }
    }

    private void CheckForFlipMotion()
    {
        // Check if enough time has passed since last flip
        if (Time.time - lastFlipTime < flipCooldown) return;

        // Calculate angle change
        float angleChange = Quaternion.Angle(lastRotation, transform.rotation);

        // Calculate movement speed
        float movementSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        // If pan is tilted enough or moved quickly, register a flip
        if (angleChange > flipAngleThreshold || movementSpeed > 5f)
        {
            StartFlipping();
            lastFlipTime = Time.time;

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Flip detected! Angle: {angleChange}, Speed: {movementSpeed}");
            }
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    public void StartFlipping()
    {
        if (!isCooking) return;
        isFlipping = true;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Started flipping");
        }
    }

    public void StopFlipping()
    {
        isFlipping = false;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Stopped flipping");
        }
    }

    public override void StopCooking()
    {
        base.StopCooking();

        isFlipping = false;

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        UpdateTimerDisplay();
    }

    protected override void FinishCooking()
    {
        base.FinishCooking();

        isFlipping = false;

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

    public bool IsBeingHeld() => isBeingHeld;
    public bool IsFlipping() => isFlipping;
    public float GetProperCookingTime() => properCookingTime;
    public float GetMaxCookingTime() => maxCookingTime;
}