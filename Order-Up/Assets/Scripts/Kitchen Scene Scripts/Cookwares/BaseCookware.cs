using UnityEngine;
using TMPro;

/// <summary>
/// Base class for all cookware. Handles drop zone logic and single ingredient restriction.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseCookware : MonoBehaviour, IDropZone
{
    [Header("Cookware Settings")]
    [SerializeField] protected string cookwareName;
    [SerializeField] protected CookwareType cookwareType;

    [Header("UI References")]
    [SerializeField] protected TextMeshProUGUI timerDisplayText;

    [Header("Cooking State")]
    protected GameObject ingredientInside;
    protected bool isCooking = false;
    protected float currentCookingTime = 0f;

    [Header("Debug Settings")]
    public bool enableDebugLogs = false;

    // Components
    protected Collider2D cookwareCollider;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb2D;
    protected Transform cookwareParent;

    protected virtual void Awake()
    {
        cookwareCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        cookwareParent = transform.parent;

        // Configure Rigidbody2D to be static
        rb2D.bodyType = RigidbodyType2D.Static;

        // Set as trigger for drag-and-drop detection
        cookwareCollider.isTrigger = true;
    }

    protected virtual void Start()
    {
        InitializeUI();
    }

    protected virtual void Update()
    {
        if (isCooking)
        {
            UpdateCookingLogic();
        }
    }

    // IDropZone implementation
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // set Cookware the parent of the ingredient
    public void SetIngredientParent(GameObject ingredient)
    {
        ingredient.transform.SetParent(cookwareParent);
    }

    /// <summary>
    /// Check if cookware can accept an ingredient (only one at a time)
    /// </summary>
    public virtual bool CanAcceptIngredient(GameObject ingredientObj = null)
    {
        // First check if cookware is available
        if (ingredientInside != null || isCooking)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Cannot accept - already has ingredient or is cooking");
            }
            PopupManager.Instance.ShowPopup("Already Cooking", this.transform);
            return false;
        }

        // If no ingredient provided, just check availability
        if (ingredientObj == null)
        {
            return true;
        }

        // Check if ingredient's cookware type matches this cookware
        Ingredient ingredient = ingredientObj.GetComponent<Ingredient>();
        if (ingredient == null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Object has no Ingredient component");
            }
            return false;
        }

        bool cookwareMatches = ingredient.CheckAllowedCookware(cookwareType);

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] CanAcceptIngredient: {cookwareMatches} " +
                      $"(Ingredient: {ingredient.IngredientName}, " +
                      $"Ingredient's Cookware: {ingredient.currentCookware}, " +
                      $"This Cookware: {cookwareType})");
        }

        return cookwareMatches;
    }

    // Trigger detection for ingredients
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Trigger Entered by: {other.gameObject.name}");
        }

        //    // Check for DraggableIngredient component
        DraggableIngredient draggable = other.GetComponent<DraggableIngredient>();

        if (draggable == null) return;

        if (draggable.IsDragging) return;


        // Only accept if we can
        if (!CanAcceptIngredient(other.gameObject))
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Cannot accept ingredient - already full or cooking");
            }
            return;
        }

        // Accept the ingredient
        ingredientInside = other.gameObject;
        OnIngredientEntered(other.gameObject);
    }

    public void ManuallyAcceptIngredient(GameObject ingredient)
    {
        ingredientInside = ingredient;
        OnIngredientEntered(ingredient);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        DraggableIngredient draggable = other.GetComponent<DraggableIngredient>();
        if (draggable == null) return;
        // Don't allow ingredients to leave while cooking
        if (isCooking)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] Ingredient cannot leave while cooking");
            }
            return;
        }

        if (ingredientInside == other.gameObject)
        {
            ingredientInside = null;
            OnIngredientExited(other.gameObject);
        }
    }

    // Abstract methods - must be implemented by children
    protected abstract void InitializeUI();
    protected abstract void UpdateCookingLogic();
    protected abstract void OnIngredientEntered(GameObject ingredient);
    protected abstract void OnIngredientExited(GameObject ingredient);

    // Virtual methods - can be overridden
    public virtual void StartCooking()
    {
        if (ingredientInside == null || isCooking) return;

        isCooking = true;
        currentCookingTime = 0f;
        
        // Notify tutorial manager that cooking has started
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnCookwareStarted(this);
        }

        // Make ingredient semi-transparent while cooking
        SetIngredientOpacity(0.5f);

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Started cooking");
        }
    }

    public virtual void StopCooking()
    {
        if (!isCooking) return;

        isCooking = false;
        currentCookingTime = 0f;

        // Restore ingredient opacity
        SetIngredientOpacity(1f);

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Stopped cooking");
        }
    }

    protected virtual void FinishCooking()
    {
        if (ingredientInside == null) return;

        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Cooking finished!");
        }
        
        // Get the ingredient component before processing
        Ingredient ing = ingredientInside.GetComponent<Ingredient>();
        
        // Notify the TutorialManager that cooking is finished
        if (TutorialManager.Instance != null)
        {
            // We pass the cookware and the ingredient that just finished
            TutorialManager.Instance.OnCookingFinished(this, ing);
        }
        
        if (ing != null)
        {
            ProcessIngredient(ing);
        }

        // Restore opacity
        SetIngredientOpacity(1f);

        // Clear the ingredient
        ingredientInside = null;
        isCooking = false;
    }

    protected virtual void ProcessIngredient(Ingredient ing)
    {
        ing.currentCookware = cookwareType;

        if (ing.currentState == IngredientState.Raw)
        {
            ing.CookIngredient();
            if (ing.ingredientData.cookedResult != null)
            {
                ing.ingredientData = ing.ingredientData.cookedResult;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} is now Cooked!");
            }
        }
        else if (ing.currentState == IngredientState.Cooked)
        {
            ing.OvercookIngredient();
            if (ing.ingredientData.overcookedResult != null)
            {
                ing.ingredientData = ing.ingredientData.overcookedResult;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} got Overcooked!");
            }
        }
    }

    protected void SetIngredientOpacity(float alpha)
    {
        if (ingredientInside != null)
        {
            SpriteRenderer sr = ingredientInside.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
        }
    }

    // Public getters
    public bool IsCooking() => isCooking;
    public float GetCurrentCookingTime() => currentCookingTime;
    public GameObject GetIngredientInside() => ingredientInside;
    public CookwareType GetCookwareType() => cookwareType;
}