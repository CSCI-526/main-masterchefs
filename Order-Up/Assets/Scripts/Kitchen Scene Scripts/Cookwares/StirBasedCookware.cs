using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Cookware that requires stirring (Pot) with spatula motion detection
/// </summary>
public class StirBasedCookware : BaseCookware
{
    [Header("Progress UI")]
    [SerializeField] private Slider progressBar;

    [Header("Cooking Settings")]
    [SerializeField] private float properCookingTime = 5f;
    [SerializeField] private float maxCookingTime = 10f;

    [Header("Stirring Detection")]
    [SerializeField] private string spatulaTag = "Spatula";
    [SerializeField] private float stirSpeedThreshold = 0.5f; // Minimum speed to count as stirring
    [SerializeField] private float circularMotionThreshold = 0.3f; // How circular the motion needs to be
    [SerializeField] private int motionSampleSize = 10; // Number of positions to track

    private bool isStirring = false;
    private GameObject spatulaInPot = null;
    private Queue<Vector3> spatulaPositions = new Queue<Vector3>();
    private Vector3 lastSpatulaPosition;
    private float totalAngleChange = 0f;
    private Vector3 potCenter;

    protected override void Start()
    {
        base.Start();
        potCenter = transform.position;
    }

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
        // Detect stirring motion if spatula is in pot
        if (spatulaInPot != null && isCooking)
        {
            DetectStirringMotion();
        }

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

    private void DetectStirringMotion()
    {
        if (spatulaInPot == null) return;

        Vector3 currentPos = spatulaInPot.transform.position;
        float speed = (currentPos - lastSpatulaPosition).magnitude / Time.deltaTime;

        // Debug to tune threshold
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Stir speed: {speed:F3}");
        }

        // If spatula is moving fast enough, start stirring
        if (speed > stirSpeedThreshold)
        {
            StartStirring();
        }
        else
        {
            StopStirring();
        }

        lastSpatulaPosition = currentPos;
    }


    private bool CheckCircularMotion()
    {
        if (spatulaPositions.Count < 3) return false;

        Vector3[] positions = spatulaPositions.ToArray();

        // Get the center point (pot center in 2D plane)
        Vector3 center = new Vector3(potCenter.x, positions[0].y, potCenter.z);

        // Calculate angle changes
        float angleSum = 0f;
        for (int i = 1; i < positions.Length; i++)
        {
            Vector3 dir1 = positions[i - 1] - center;
            Vector3 dir2 = positions[i] - center;

            // Project to XZ plane (horizontal stirring)
            dir1.y = 0;
            dir2.y = 0;

            if (dir1.magnitude > 0.01f && dir2.magnitude > 0.01f)
            {
                float angle = Vector3.SignedAngle(dir1, dir2, Vector3.up);
                angleSum += Mathf.Abs(angle);
            }
        }

        totalAngleChange = angleSum;

        // If we've rotated enough degrees, it's circular motion
        // A full stir would be 360 degrees, but we check for partial rotation
        return angleSum > 10f; // Minimum rotation to count as stirring
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        // Check for spatula
        if (other.CompareTag(spatulaTag))
        {
            spatulaInPot = other.gameObject;
            lastSpatulaPosition = other.transform.position;
            spatulaPositions.Clear();

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Spatula entered pot");
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        // Check for spatula
        if (other.CompareTag(spatulaTag) && other.gameObject == spatulaInPot)
        {
            spatulaInPot = null;
            StopStirring();
            spatulaPositions.Clear();

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Spatula left pot");
            }
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
                string stirStatus = isStirring ? "Stirring" : "Not Stirring";
                timerDisplayText.text = $"{currentCookingTime:F1}s - {stirStatus}";
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

        if (!isStirring)
        {
            isStirring = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Started stirring");
            }
        }
    }

    public void StopStirring()
    {
        if (isStirring)
        {
            isStirring = false;

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Stopped stirring");
            }
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

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Draw pot center
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(potCenter, 0.1f);

        // Draw spatula positions
        if (spatulaPositions.Count > 0)
        {
            Gizmos.color = Color.green;
            Vector3[] positions = spatulaPositions.ToArray();
            for (int i = 1; i < positions.Length; i++)
            {
                Gizmos.DrawLine(positions[i - 1], positions[i]);
            }
        }
    }

    public bool IsStirring() => isStirring;
    public float GetProperCookingTime() => properCookingTime;
    public float GetMaxCookingTime() => maxCookingTime;
    public float GetTotalAngleChange() => totalAngleChange;
}