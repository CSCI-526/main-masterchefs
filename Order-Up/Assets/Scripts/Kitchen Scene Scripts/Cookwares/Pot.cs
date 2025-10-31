using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Pot : MonoBehaviour, IDropZone
{
    [Header("Cookware Settings")]
    [SerializeField] private string cookwareName;
    public CookwareType cookwareType;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerDisplayText;
    [SerializeField] private Slider progress;
    
    [Header("Cooking Time Settings")]
    [SerializeField] private float properCookingTime = 5f;
    [SerializeField] private float maxCookingTime = 20f;
    
    [Header("Cooking State")]
    private GameObject ingredientInside;
    private bool isCooking = false;
    private float currentCookingTime = 0f;

    [Header("Debug Settings")]
    public bool enableDebugLogs = true;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2D;
    private bool isStirring = false;

    public void StartStirring()
    {
        if (!isCooking) return;
        isStirring = true;
    }
    
    public void StopStirring()
    {
        isStirring=false;
    }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        
        // Configure Rigidbody2D to be static (doesn't move or rotate)
        rb2D.bodyType = RigidbodyType2D.Static;

        // Set as trigger for drag-and-drop detection
        boxCollider.isTrigger = true;
        
        if (timerDisplayText != null && !isCooking)
        {
            timerDisplayText.text = $"Cook Time: {currentCookingTime:F1}s";
        }
        if (progress != null)
        {
            progress.minValue = 0f;
            progress.maxValue = properCookingTime;
            progress.value = 0f;
        }
    }

    // IDropZone implementation
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    void Update()
    {
        if (isCooking)
        {
            if (isStirring)
            {
                currentCookingTime += Time.deltaTime;
                if (progress != null)
                {
                    progress.value = Mathf.Clamp(currentCookingTime, 0f, properCookingTime);
                }
            }

            if (timerDisplayText != null)
            {
                timerDisplayText.text = $"{currentCookingTime:F1}s";
            }

            if (currentCookingTime >= maxCookingTime)
            {
                Ingredient ing = null;
                if (ingredientInside != null)
                {
                    ing = ingredientInside.GetComponent<Ingredient>();
                }
                if (ing != null && ing.currentState == IngredientState.Cooked)
                {
                    ing.OvercookIngredient();
                    if (ing.ingredientData.overcookedResult != null)
                    {
                        ing.ingredientData = ing.ingredientData.overcookedResult;
                    }
                    Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} Overcooked");
                }
                StopCooking();
            }
            else if (currentCookingTime >= properCookingTime)
            {
                FinishCooking();
            }
        }
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] Trigger Entered by: {other.gameObject.name}");
        }

        // Check for DraggableIngredient component instead of tag
        DraggableIngredient draggable = other.GetComponent<DraggableIngredient>();
        if (draggable == null) return;
        
        if (ingredientInside != other.gameObject)
        {
            ingredientInside = other.gameObject;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] ⚠️ Ingredient entered - start cooking");
            }
        }
        
        StartCooking();
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        // Allow ingredients to leave while cooking
        if (ingredientInside == other.gameObject) {
            StopCooking();
            if (enableDebugLogs)
            {
                Debug.Log($"[{cookwareName}] exited during cooking: {other.gameObject.name}");
            }
        }
    }
    
    public void StartCooking()
    {
        if (ingredientInside == null || isCooking) return;
        
        isCooking = true;
        currentCookingTime = 0f;

        SpriteRenderer sr = ingredientInside.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 1f, 1f, 0.5f); // 50% transparent
        }
        
        Debug.Log($"Started cooking in {cookwareName}");
    }
    
    private void FinishCooking()
    {
        Debug.Log($"[{cookwareName}] ✅ Cooking finished!");

        if (ingredientInside == null) 
        {
            return; 
        }

        // Re-enable all ingredients after cooking
        EnableIngredients();

        Ingredient ing = ingredientInside.GetComponent<Ingredient>();

        if (ing != null)
        {
            ing.currentCookware = cookwareType;
            //change ingredient data
            if(ing.currentState == IngredientState.Raw)
            {
                ing.CookIngredient();
                if (ing.ingredientData.cookedResult != null)
                {
                    ing.ingredientData = ing.ingredientData.cookedResult;
                }
                Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} is now Cooked!");
            }
            else if (ing.currentState == IngredientState.Cooked)
            { 
                ing.OvercookIngredient();

                if (ing.ingredientData.overcookedResult != null)
                {
                    ing.ingredientData = ing.ingredientData.overcookedResult;
                }

                Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} got Overcooked!");
            }


        }
        ingredientInside = null;
        isStirring = false;
        if (progress != null)
        {
            progress.value = 0f;
        }
    }
    
    public void StopCooking()
    {
        isStirring = false;
        if (isCooking)
        {
            isCooking = false;
            currentCookingTime = 0f;

            // Re-enable ingredients if cooking was stopped early
            EnableIngredients();

            Debug.Log($"Cooking stopped in {cookwareName}");
        }
        if (progress != null)
        {
            progress.value = 0f;
        }
    }

    // Re-enable ingredients after cooking
    private void EnableIngredients()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] 🔄 EnableIngredients called. ");
        }
        
        if (ingredientInside != null)
        {
            // Restore full opacity
            SpriteRenderer sr = ingredientInside.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.white; // Full opacity
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[{cookwareName}] 🎨 Restored opacity for: {ingredientInside.name}");
                }
            }
        }
        
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{cookwareName}] ✅ EnableIngredients completed.");
        }
    }
    
    public bool IsCooking() => isCooking;
    public float GetCurrentCookingTime() => currentCookingTime;
}