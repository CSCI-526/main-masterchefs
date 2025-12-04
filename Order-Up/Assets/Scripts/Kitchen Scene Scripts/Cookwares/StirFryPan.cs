using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Pan that requires shaking/stirring motion to cook ingredients
/// Ingredients bounce around inside using physics
/// </summary>
/// <upgrade>
/// Pan's properCookingTime decreases from 10s to 8s to 5s as appliance is upgraded
/// </upgrade>
public class StirFryPan : BaseCookware
{
    [Header("Progress UI")]
    [SerializeField] private Slider progressBar;

    [Header("Cooking Settings")]
    [SerializeField] private float properCookingTime = 10f; // Time needed with good stirring
    [SerializeField] private float maxCookingTime = 20f; // Overcook threshold

    [Header("Pan Boundaries")]
    [SerializeField] private CircleCollider2D panBoundary; // The circular boundary
    [SerializeField] private float boundaryRadius; // Auto-set if panBoundary exists

    [Header("Stir Detection")]
    [SerializeField] private float stirSpeedThreshold = 3f; // How fast pan must move
    [SerializeField] private float stirCheckInterval = 0.1f; // How often to check stirring

    [Header("Ingredient Physics")]
    [SerializeField] private float ingredientBounciness = 0.6f; // How bouncy ingredients are
    [SerializeField] private float ingredientFriction = 0.3f; // Friction on pan surface
    [SerializeField] private float panPushForce = 5f; // How much pan movement affects ingredient

    private bool isBeingHeld = false;
    private Vector3 lastPanPosition;
    private Vector3 panVelocity;
    private float lastStirCheckTime = 0f;
    private float totalStirTime = 0f; // Accumulated time spent stirring
    private bool isStirring = false;

    // For ingredient physics
    private Rigidbody2D ingredientRb;
    private PhysicsMaterial2D originalPhysicsMaterial;
    private PhysicsMaterial2D panPhysicsMaterial;

    protected override void Awake()
    {
        base.Awake();

        // Create physics material for bouncy pan surface
        panPhysicsMaterial = new PhysicsMaterial2D("PanSurface");
        panPhysicsMaterial.bounciness = ingredientBounciness;
        panPhysicsMaterial.friction = ingredientFriction;

        // Set up pan boundary
        if (panBoundary != null)
        {
            panBoundary.sharedMaterial = panPhysicsMaterial;
        }
    }

    protected override void InitializeUI()
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = properCookingTime;
            progressBar.value = 0f;
        }

        lastPanPosition = transform.position;
        UpdateTimerDisplay();
    }

    protected override void Update()
    {
        base.Update();

        // Calculate pan velocity if being held
        if (isBeingHeld)
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = transform.position.z;
            panVelocity = (currentMousePos - lastPanPosition) / Time.deltaTime;
            lastPanPosition = currentMousePos;

            // Check if stirring fast enough
            if (Time.time - lastStirCheckTime > stirCheckInterval)
            {
                CheckStirringMotion();
                lastStirCheckTime = Time.time;
            }

            // Apply force to ingredient based on pan movement
            if (isCooking && ingredientRb != null)
            {
                ApplyPanForceToIngredient();
            }
        }
        else
        {
            panVelocity = Vector3.zero;
            isStirring = false;
        }
    }

    protected override void UpdateCookingLogic()
    {
        // Only increase cooking progress when actively stirring
        if (isStirring)
        {
            totalStirTime += Time.deltaTime;
        }

        // Update progress bar based on stir time
        if (progressBar != null)
        {
            progressBar.value = Mathf.Clamp(totalStirTime, 0f, properCookingTime);
        }

        UpdateTimerDisplay();

        // Check if properly cooked
        if (totalStirTime >= properCookingTime)
        {
            FinishCooking();
        }
        // Check if time ran out (overcooked)
        else if (currentCookingTime >= maxCookingTime)
        {
            //OvercookIngredient();
            StopCooking();
        }

        currentCookingTime += Time.deltaTime;
    }

    /// <summary>
    /// place the ingredent at the center position of the pan if OnMouseUp is called
    /// once OnMouseUp is called, meaning the player has dropped the pan, deplay for a short time
    /// so that the ingredient can settle in the pan, and player doesn't accidentally start cooking
    /// </summary>
    /// <param name="ingredient"></param>
    /// 
    private void DelaySettleIngredient(GameObject ingredient)
    {
        // Start a coroutine to delay settling
        StartCoroutine(SettleIngredientAfterDelay(ingredient, 0.2f));
    }

    private System.Collections.IEnumerator SettleIngredientAfterDelay(GameObject ingredient, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Position ingredient at center of pan
        ingredient.transform.position = transform.position;
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient settled in pan after delay");
        }
    }

    protected override void OnIngredientEntered(GameObject ingredient)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient entered pan");
        }

        // Set up ingredient physics
        ingredientRb = ingredient.GetComponent<Rigidbody2D>();
        if (ingredientRb != null)
        {
            // Save original material
            Collider2D ingredientCollider = ingredient.GetComponent<Collider2D>();
            if (ingredientCollider != null)
            {
                originalPhysicsMaterial = ingredientCollider.sharedMaterial;
                // Apply bouncy material
                ingredientCollider.sharedMaterial = panPhysicsMaterial;
            }

            // Make ingredient dynamic with physics
            ingredientRb.bodyType = RigidbodyType2D.Dynamic;
            ingredientRb.gravityScale = 0f; // No gravity (top-down view)
            ingredientRb.mass = 1f;
            ingredientRb.linearDamping = 10.5f; // Some air resistance
            ingredientRb.angularDamping = 10.5f;

            // Position ingredient at center of pan
            ingredient.transform.position = transform.position;
            ingredientRb.linearVelocity = Vector2.zero;
        }

        if (fireOn)
            StartCooking();
        else
            CenterIngredient();

    }
    public override void SetFire(bool on)
    {
        fireOn = on;

        if (!fireOn)
        {
            StopCooking();
            isStirring = false;
        }
    }

    protected override void OnIngredientExited(GameObject ingredient)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Ingredient left pan");
        }

        // Restore original physics
        if (ingredientRb != null)
        {
            ingredientRb.bodyType = RigidbodyType2D.Kinematic;
            ingredientRb.linearVelocity = Vector2.zero;

            Collider2D ingredientCollider = ingredient.GetComponent<Collider2D>();
            if (ingredientCollider != null && originalPhysicsMaterial != null)
            {
                ingredientCollider.sharedMaterial = originalPhysicsMaterial;
            }
        }

        StopCooking();
    }

    private void UpdateTimerDisplay()
    {
        if (timerDisplayText != null)
        {
            if (isCooking)
            {
                if (isStirring)
                {
                    timerDisplayText.text = $"🔥 Stirring! {totalStirTime:F1}s";
                }
                else
                {
                    timerDisplayText.text = $"Move pan faster!";
                }
            }
            else
            {
                timerDisplayText.text = $"Drag to Stir";
            }
        }
    }

    private void OnMouseDown()
    {
        isBeingHeld = true;
        lastPanPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastPanPosition.z = transform.position.z;

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
        if (!fireOn && ingredientInside != null)
        {
            CenterIngredient();
        }
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;
        panVelocity = Vector3.zero;
        isStirring = false;
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Pan released");
        }
    }

    private void CheckStirringMotion()
    {
        float speed = panVelocity.magnitude;

        // Check if moving fast enough to count as stirring
        if (speed > stirSpeedThreshold)
        {
            isStirring = true;
            
            // Notify tutorial manager that cooking has started
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnPanToss(this);
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Stirring! Speed: {speed:F1}");
            }
        }
        else
        {
            isStirring = false;
        }
    }

    private void ApplyPanForceToIngredient()
    {
        if (ingredientRb == null) return;

        // Apply force to ingredient based on pan movement
        // This simulates the pan "pushing" the ingredient around
        Vector2 force = new Vector2(panVelocity.x, panVelocity.y) * panPushForce;
        ingredientRb.AddForce(force, ForceMode2D.Force);

        // Keep ingredient within pan boundaries (circular constraint)
        Vector2 ingredientPos = ingredientRb.position;
        Vector2 panCenter = new Vector2(transform.position.x, transform.position.y);
        float distanceFromCenter = Vector2.Distance(ingredientPos, panCenter);

        // If ingredient is near the edge, push it back slightly
        if (distanceFromCenter > boundaryRadius * 0.8f)
        {
            Vector2 directionToCenter = (panCenter - ingredientPos).normalized;
            ingredientRb.AddForce(directionToCenter * panPushForce * 2f, ForceMode2D.Force);
        }

        // Add some rotation for visual effect
        if (isStirring)
        {
            ingredientRb.AddTorque(panVelocity.magnitude * 5f, ForceMode2D.Force);
        }

        EnforcePanBoundary();
    }

    private void EnforcePanBoundary()
    {
        if (ingredientRb == null) return;

        Vector2 panCenter = transform.position;
        Vector2 ingredientPos = ingredientRb.position;
        float dist = Vector2.Distance(ingredientPos, panCenter);

        if (dist > boundaryRadius)
        {
            Vector2 clamped = panCenter + (ingredientPos - panCenter).normalized * (boundaryRadius - 0.05f);
            ingredientRb.position = clamped;
            ingredientRb.linearVelocity = Vector2.zero;
        }
    }


    public override void StartCooking()
    {
        ingredientInside.transform.position = transform.position;
        base.StartCooking();

        totalStirTime = 0f;

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Start stir-frying! Move pan around!");
        }
    }

    private void CenterIngredient()
    {
        if (ingredientRb == null) return;

        ingredientRb.bodyType = RigidbodyType2D.Kinematic;
        ingredientRb.linearVelocity = Vector2.zero;
        ingredientRb.angularVelocity = 0f;
        ingredientInside.transform.position = transform.position;
    }

    public void ToggleFire(bool on)
    {
        fireOn = on;

        if (!fireOn)
        {
            StopCooking();
            CenterIngredient();
        }

        if (fireOn && ingredientInside != null)
        {
            StartCooking();
        }
    }


    public override void StopCooking()
    {
        base.StopCooking();

        totalStirTime = 0f;
        isStirring = false;

        // Restore ingredient physics
        if (ingredientInside != null && ingredientRb != null)
        {
            ingredientRb.bodyType = RigidbodyType2D.Kinematic;
            ingredientRb.linearVelocity = Vector2.zero;
            ingredientRb.angularVelocity = 0f;

            Collider2D ingredientCollider = ingredientInside.GetComponent<Collider2D>();
            if (ingredientCollider != null && originalPhysicsMaterial != null)
            {
                ingredientCollider.sharedMaterial = originalPhysicsMaterial;
            }
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }
    }

    protected override void FinishCooking()
    {
        base.FinishCooking();

        totalStirTime = 0f;
        isStirring = false;

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        fireOn = false;
        CenterIngredient();

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] ✅ Stir-fry complete!");
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
                Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} Overcooked - took too long!");
            }
        }
    }

    // Visualize pan boundary in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius * 0.8f);
    }

    public bool IsBeingHeld() => isBeingHeld;
    public bool IsStirring() => isStirring;
    public float GetTotalStirTime() => totalStirTime;
    public float GetProperCookingTime() => properCookingTime;
    public float GetMaxCookingTime() => maxCookingTime;
}