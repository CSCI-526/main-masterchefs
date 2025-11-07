//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using TMPro;

//[RequireComponent(typeof(SpriteRenderer))]
//[RequireComponent(typeof(PolygonCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
//public class Cookwares : MonoBehaviour, IDropZone
//{
//    [Header("Cookware Settings")]
//    [SerializeField] private string cookwareName;
//    public CookwareType cookwareType;

//    [Header("UI References")]
//    [SerializeField] private GameObject sliderPanel;
//    [SerializeField] private Slider cookingTimeSlider;
//    [SerializeField] private TextMeshProUGUI timerDisplayText;

//    [Header("Cooking Time Settings")]
//    [SerializeField] private float minCookingTime = 1f;
//    [SerializeField] private float maxCookingTime = 20f;

//    [Header("Cooking State")]
//    private List<GameObject> ingredientsInside = new List<GameObject>();
//    private bool isCooking = false;
//    private float currentCookingTime = 0f;
//    private float selectedCookingTime = 10f;

//    [Header("Debug Settings")]
//    public bool enableDebugLogs = false;

//    private PolygonCollider2D polygonCollider;
//    private SpriteRenderer spriteRenderer;
//    private Rigidbody2D rb2D;

//    void Start()
//    {
//        polygonCollider = GetComponent<PolygonCollider2D>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        rb2D = GetComponent<Rigidbody2D>();

//        // Configure Rigidbody2D to be static (doesn't move or rotate)
//        rb2D.bodyType = RigidbodyType2D.Static;

//        // Set as trigger for drag-and-drop detection
//        polygonCollider.isTrigger = true;

//        if (cookingTimeSlider != null)
//        {
//            cookingTimeSlider.minValue = minCookingTime;
//            cookingTimeSlider.maxValue = maxCookingTime;
//            cookingTimeSlider.value = selectedCookingTime;
//            cookingTimeSlider.interactable = false; // Start disabled, will enable when ingredients added

//            cookingTimeSlider.onValueChanged.AddListener(OnSliderValueChanged);

//            if (enableDebugLogs)
//            {
//                Debug.Log($"[{cookwareName}] Slider initialized: min={minCookingTime}, max={maxCookingTime}, value={selectedCookingTime}");
//            }
//        }

//        if (sliderPanel != null)
//        {
//            sliderPanel.SetActive(false);
//        }

//        UpdateSliderState();
//    }

//    // IDropZone implementation
//    public GameObject GetGameObject()
//    {
//        return gameObject;
//    }

//    void Update()
//    {
//        if (isCooking)
//        {
//            currentCookingTime += Time.deltaTime;

//            if (timerDisplayText != null)
//            {
//                timerDisplayText.text = $"{currentCookingTime:F1}s / {selectedCookingTime:F1}s";
//            }

//            if (currentCookingTime >= selectedCookingTime)
//            {
//                FinishCooking();
//            }
//        }

//        UpdateSliderState();
//    }

//    void OnMouseDown()
//    {
//        if (sliderPanel != null)
//        {
//            sliderPanel.SetActive(!sliderPanel.activeSelf);

//            if (sliderPanel.activeSelf)
//            {
//                UpdateSliderState();
//            }
//        }
//    }

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (enableDebugLogs)
//        {
//            Debug.Log($"[{cookwareName}] Trigger Entered by: {other.gameObject.name}");
//        }

//        // Check for DraggableIngredient component instead of tag
//        DraggableIngredient draggable = other.GetComponent<DraggableIngredient>();
//        if (draggable != null)
//        {
//            if (!ingredientsInside.Contains(other.gameObject))
//            {
//                ingredientsInside.Add(other.gameObject);

//                // Keep ingredient draggable when it enters (unless cooking)
//                if (!isCooking)
//                {
//                    draggable.enabled = true;
//                    SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
//                    if (sr != null)
//                    {
//                        sr.color = Color.white; // Full opacity
//                    }

//                    if (enableDebugLogs)
//                    {
//                        Debug.Log($"[{cookwareName}] ✅ Ingredient entered while NOT cooking - dragging ENABLED");
//                        Debug.Log($"  - DraggableIngredient.enabled = {draggable.enabled}");
//                    }
//                }
//                else
//                {
//                    if (enableDebugLogs)
//                    {
//                        Debug.Log($"[{cookwareName}] ⚠️ Ingredient entered while cooking - dragging DISABLED");
//                    }
//                }

//                Debug.Log($"Ingredient {other.gameObject.name} entered {cookwareName}");

//                UpdateSliderState();
//            }
//        }
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        // Don't allow ingredients to leave while cooking
//        if (isCooking)
//        {
//            if (enableDebugLogs)
//            {
//                Debug.Log($"[{cookwareName}] Ignoring exit during cooking: {other.gameObject.name}");
//            }
//            return;
//        }

//        if (ingredientsInside.Contains(other.gameObject))
//        {
//            ingredientsInside.Remove(other.gameObject);
//            Debug.Log($"Ingredient {other.gameObject.name} left {cookwareName}");

//            UpdateSliderState();
//        }
//    }

//    private void UpdateSliderState()
//    {
//        if (cookingTimeSlider != null)
//        {
//            // Clean up any destroyed ingredients
//            ingredientsInside.RemoveAll(item => item == null);

//            bool hasIngredients = ingredientsInside.Count > 0;

//            // Slider is interactive when there are ingredients AND not cooking
//            cookingTimeSlider.interactable = hasIngredients && !isCooking;
//            // Visual feedback: change slider color based on state
//            ColorBlock colors = cookingTimeSlider.colors;
//            if (!hasIngredients)
//            {
//                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray when no ingredients
//            }
//            cookingTimeSlider.colors = colors;
//        }
//    }

//    private void OnSliderValueChanged(float value)
//    {
//        selectedCookingTime = value;

//        if (timerDisplayText != null && !isCooking)
//        {
//            timerDisplayText.text = $"Cook Time: {selectedCookingTime:F1}s";
//            cookingTimeSlider.value = selectedCookingTime;

//        }
//    }

//    public void StartCooking()
//    {
//        if (ingredientsInside.Count > 0 && !isCooking)
//        {
//            isCooking = true;
//            currentCookingTime = 0f;
//            cookingTimeSlider.interactable = false;

//            // Disable all ingredients so they can't be dragged during cooking
//            DisableIngredients();

//            Debug.Log($"Started cooking in {cookwareName} for {selectedCookingTime} seconds");
//        }
//    }

//    private void FinishCooking()
//    {
//        isCooking = false;
//        currentCookingTime = 0f;

//        Debug.Log($"[{cookwareName}] ✅ Cooking finished! Ingredient count: {ingredientsInside.Count}");

//        // ADD DIRT TO COOKWARE AFTER COOKING
//        CookwareMaintenance maintenance = GetComponent<CookwareMaintenance>();
//        if (maintenance != null)
//        {
//            // Add 1 dirt level for each ingredient cooked
//            maintenance.AddDirt(ingredientsInside.Count);
//            Debug.Log($"[{cookwareName}] Added {ingredientsInside.Count} dirt levels");
//        }

//        // Re-enable all ingredients after cooking
//        EnableIngredients();

//        foreach (GameObject ingredient in ingredientsInside)
//        {
//            if (ingredient == null) continue;

//            Ingredient ing = ingredient.GetComponent<Ingredient>();

//            if (ing != null)
//            {
//                ing.currentCookware = cookwareType;
//                //change ingredient data
//                if (ing.currentState == IngredientState.Raw)
//                {
//                    ing.CookIngredient();
//                    if (ing.ingredientData.cookedResult != null)
//                    {
//                        ing.ingredientData = ing.ingredientData.cookedResult;
//                    }
//                    Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} is now Cooked!");
//                }
//                else if (ing.currentState == IngredientState.Cooked)
//                {
//                    ing.OvercookIngredient();

//                    if (ing.ingredientData.overcookedResult != null)
//                    {
//                        ing.ingredientData = ing.ingredientData.overcookedResult;
//                    }

//                    Debug.Log($"[{cookwareType}] {ing.ingredientData.ingredientName} got Overcooked!");
//                }
//            }
//        }

//        UpdateSliderState();
//    }

//    public void StopCooking()
//    {
//        if (isCooking)
//        {
//            isCooking = false;
//            currentCookingTime = 0f;

//            // Re-enable ingredients if cooking was stopped early
//            EnableIngredients();

//            UpdateSliderState();
//            Debug.Log($"Cooking stopped in {cookwareName}");
//        }
//    }

//    // Disable ingredients during cooking
//    private void DisableIngredients()
//    {
//        foreach (GameObject ingredient in ingredientsInside)
//        {
//            if (ingredient != null)
//            {
//                // Instead, disable only the DraggableIngredient component
//                DraggableIngredient draggable = ingredient.GetComponent<DraggableIngredient>();
//                if (draggable != null)
//                {
//                    draggable.enabled = false;
//                }

//                // Optionally make it semi-transparent to show it's "cooking"
//                SpriteRenderer sr = ingredient.GetComponent<SpriteRenderer>();
//                if (sr != null)
//                {
//                    sr.color = new Color(1f, 1f, 1f, 0.5f); // 50% transparent
//                }

//                if (enableDebugLogs)
//                {
//                    Debug.Log($"[{cookwareName}] Disabled dragging for ingredient: {ingredient.name}");
//                }
//            }
//        }
//    }

//    // Re-enable ingredients after cooking
//    private void EnableIngredients()
//    {
//        if (enableDebugLogs)
//        {
//            Debug.Log($"[{cookwareName}] 🔄 EnableIngredients called. Current count: {ingredientsInside.Count}");
//        }

//        // Clean up null references first
//        int beforeCleanup = ingredientsInside.Count;
//        ingredientsInside.RemoveAll(item => item == null);
//        int afterCleanup = ingredientsInside.Count;

//        if (enableDebugLogs && beforeCleanup != afterCleanup)
//        {
//            Debug.LogWarning($"[{cookwareName}] ⚠️ Removed {beforeCleanup - afterCleanup} null ingredients during cleanup!");
//        }

//        foreach (GameObject ingredient in ingredientsInside)
//        {
//            if (ingredient != null)
//            {
//                // Re-enable the DraggableIngredient component
//                DraggableIngredient draggable = ingredient.GetComponent<DraggableIngredient>();
//                if (draggable != null)
//                {
//                    draggable.enabled = true;

//                    if (enableDebugLogs)
//                    {
//                        Debug.Log($"[{cookwareName}] ✅ Enabled DraggableIngredient for: {ingredient.name}");
//                    }
//                }
//                else
//                {
//                    Debug.LogWarning($"[{cookwareName}] ⚠️ No DraggableIngredient component found on: {ingredient.name}");
//                }

//                // Restore full opacity
//                SpriteRenderer sr = ingredient.GetComponent<SpriteRenderer>();
//                if (sr != null)
//                {
//                    sr.color = Color.white; // Full opacity

//                    if (enableDebugLogs)
//                    {
//                        Debug.Log($"[{cookwareName}] 🎨 Restored opacity for: {ingredient.name}");
//                    }
//                }
//            }
//        }

//        if (enableDebugLogs)
//        {
//            Debug.Log($"[{cookwareName}] ✅ EnableIngredients completed. Final count: {ingredientsInside.Count}");
//        }
//    }

//    public bool IsCooking() => isCooking;
//    public int GetIngredientCount() => ingredientsInside.Count;
//    public float GetCurrentCookingTime() => currentCookingTime;
//    public float GetSelectedCookingTime() => selectedCookingTime;
//}





















using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Cookwares : MonoBehaviour, IDropZone
{
    [Header("Cookware Settings")]
    [SerializeField] private string cookwareName;
    public CookwareType cookwareType;

    [Header("UI References")]
    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private Slider cookingTimeSlider;
    [SerializeField] private TextMeshProUGUI timerDisplayText;

    [Header("Cooking Time Settings")]
    [SerializeField] private float minCookingTime = 1f;
    [SerializeField] private float maxCookingTime = 20f;

    [Header("Cooking State")]
    private List<GameObject> ingredientsInside = new List<GameObject>();
    private bool isCooking = false;
    private float currentCookingTime = 0f;
    private float selectedCookingTime = 10f;

    [Header("Debug Settings")]
    public bool enableDebugLogs = false;

    private PolygonCollider2D polygonCollider;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2D;

    void Start()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();

        rb2D.bodyType = RigidbodyType2D.Static;
        polygonCollider.isTrigger = true;

        if (cookingTimeSlider != null)
        {
            cookingTimeSlider.minValue = minCookingTime;
            cookingTimeSlider.maxValue = maxCookingTime;
            cookingTimeSlider.value = selectedCookingTime;
            cookingTimeSlider.interactable = false;
            cookingTimeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (sliderPanel != null)
        {
            sliderPanel.SetActive(true);
        }
        
        UpdateSliderState();
    }

    public GameObject GetGameObject() => gameObject;

    void Update()
    {
        if (isCooking)
        {
            currentCookingTime += Time.deltaTime;
            if (timerDisplayText != null)
                timerDisplayText.text = $"{currentCookingTime:F1}s / {selectedCookingTime:F1}s";

            if (currentCookingTime >= selectedCookingTime) FinishCooking();
        }
        UpdateSliderState();
    }

    void OnMouseDown()
    {
        if (sliderPanel != null)
        {
            sliderPanel.SetActive(!sliderPanel.activeSelf);
            if (sliderPanel.activeSelf) UpdateSliderState();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var draggable = other.GetComponent<DraggableIngredient>();
        if (draggable != null)
        {
            if (!ingredientsInside.Contains(other.gameObject))
            {
                ingredientsInside.Add(other.gameObject);

                if (!isCooking)
                {
                    draggable.enabled = true;
                    var sr = other.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.color = Color.white;
                }
                UpdateSliderState();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isCooking) return;
        if (ingredientsInside.Contains(other.gameObject))
        {
            ingredientsInside.Remove(other.gameObject);
            UpdateSliderState();
        }
    }

    private void UpdateSliderState()
    {
        if (cookingTimeSlider == null) return;

        ingredientsInside.RemoveAll(item => item == null);
        bool hasIngredients = ingredientsInside.Count > 0;

        cookingTimeSlider.interactable = hasIngredients && !isCooking;
        ColorBlock colors = cookingTimeSlider.colors;
        if (!hasIngredients) colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        cookingTimeSlider.colors = colors;
    }

    private void OnSliderValueChanged(float value)
    {
        selectedCookingTime = value;
        if (timerDisplayText != null && !isCooking)
        {
            timerDisplayText.text = $"Cook Time: {selectedCookingTime:F1}s";
            cookingTimeSlider.value = selectedCookingTime;
        }
    }

    public void StartCooking()
    {
        if (ingredientsInside.Count > 0 && !isCooking)
        {
            isCooking = true;
            currentCookingTime = 0f;
            cookingTimeSlider.interactable = false;
            DisableIngredients();
        }
    }

    private void FinishCooking()
    {
        isCooking = false;
        currentCookingTime = 0f;

        // ⛔️ Removed dirt-add here to avoid double counts;
        // dirt is added on drop in DraggableIngredient.StopDragging().

        EnableIngredients();

        foreach (GameObject ingredient in ingredientsInside)
        {
            if (ingredient == null) continue;
            var ing = ingredient.GetComponent<Ingredient>();
            if (ing == null) continue;

            ing.currentCookware = cookwareType;

            if (ing.currentState == IngredientState.Raw)
            {
                ing.CookIngredient();
                if (ing.ingredientData.cookedResult != null)
                    ing.ingredientData = ing.ingredientData.cookedResult;
            }
            else if (ing.currentState == IngredientState.Cooked)
            {
                ing.OvercookIngredient();
                if (ing.ingredientData.overcookedResult != null)
                    ing.ingredientData = ing.ingredientData.overcookedResult;
            }
        }

        UpdateSliderState();
    }

    public void StopCooking()
    {
        if (!isCooking) return;
        isCooking = false;
        currentCookingTime = 0f;
        EnableIngredients();
        UpdateSliderState();
    }

    private void DisableIngredients()
    {
        foreach (GameObject ingredient in ingredientsInside)
        {
            if (ingredient == null) continue;
            var draggable = ingredient.GetComponent<DraggableIngredient>();
            if (draggable != null) draggable.enabled = false;

            var sr = ingredient.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    private void EnableIngredients()
    {
        ingredientsInside.RemoveAll(item => item == null);
        foreach (GameObject ingredient in ingredientsInside)
        {
            var draggable = ingredient.GetComponent<DraggableIngredient>();
            if (draggable != null) draggable.enabled = true;

            var sr = ingredient.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;
        }
    }

    /// <summary>
    /// Check if there is already ingredient inside, if yes, return false, if not true
    /// </summary>
    /// <returns></returns>
    public bool CanAcceptIngredient()
    {
        return true;
    }


    public bool IsCooking() => isCooking;
    public int GetIngredientCount() => ingredientsInside.Count;
    public float GetCurrentCookingTime() => currentCookingTime;
    public float GetSelectedCookingTime() => selectedCookingTime;
}
