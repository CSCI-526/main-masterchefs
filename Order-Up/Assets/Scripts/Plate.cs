//using UnityEngine;
//using System.Collections.Generic;

//public class Plate : MonoBehaviour, IDropZone
//{
//    [Header("Plate Settings")]
//    public Transform ingredientParent;
//    public float maxIngredients = 100f;
//    public bool allowDuplicates = true;
//    public float ingredientSpacing = 0.2f;
//    public CombinationSystem comboSystem;

//    [Header("Visual Feedback")]
//    public GameObject highlightEffect;
//    public Color highlightColor;

//    private List<DraggableIngredient> ingredientsOnPlate; 
//    private SpriteRenderer plateRenderer;
//    private Color originalColor;

//    // Events
//    public System.Action<DraggableIngredient> OnIngredientAdded;
//    public System.Action<DraggableIngredient> OnIngredientRemoved;
//    public System.Action OnPlateFull;
//    public System.Action OnPlateEmpty;

//    void Start()
//    {
//        ingredientsOnPlate = new List<DraggableIngredient>();
//        highlightColor = Color.yellow;

//        plateRenderer = GetComponent<SpriteRenderer>();
//        if (plateRenderer != null)
//            originalColor = plateRenderer.color;

//        if (ingredientParent == null)
//            ingredientParent = transform;

//        if (highlightEffect != null)
//            highlightEffect.SetActive(false);
//    }

//    // IDropZone implementation
//    public GameObject GetGameObject()
//    {
//        return gameObject;
//    }

//    public bool AddIngredient(DraggableIngredient ingredient)
//    {
//        if (!CanAddIngredient(ingredient))
//            return false;

//        ingredientsOnPlate.Add(ingredient);
//        PositionIngredientOnPlate(ingredient);

//        if (ingredientParent != null)
//            ingredient.transform.SetParent(ingredientParent);

//        OnIngredientAdded?.Invoke(ingredient);

//        if (IsFull())
//            OnPlateFull?.Invoke();

//        if (ingredientsOnPlate.Count > 1)
//            comboSystem.CheckForCombinations();

//        Debug.Log($"Added {ingredient.name} to plate. Total ingredients: {ingredientsOnPlate.Count}");
//        return true;
//    }

//    public bool RemoveIngredient(DraggableIngredient ingredient)
//    {
//        if (!ingredientsOnPlate.Contains(ingredient))
//            return false;

//        bool wasEmpty = IsEmpty();

//        ingredientsOnPlate.Remove(ingredient);

//        ingredient.transform.SetParent(null);

//        Destroy(ingredient.gameObject);
//        RepositionIngredients();

//        OnIngredientRemoved?.Invoke(ingredient);

//        if (!wasEmpty && IsEmpty())
//            OnPlateEmpty?.Invoke();

//        Debug.Log($"Removed {ingredient.name} from plate. Total ingredients: {ingredientsOnPlate.Count}");
//        return true;
//    }

//    bool CanAddIngredient(DraggableIngredient ingredient)
//    {
//        if (IsFull())
//        {
//            Debug.Log("Plate is full!");
//            return false;
//        }

//        if (!allowDuplicates)
//        {
//            foreach (DraggableIngredient existing in ingredientsOnPlate)
//            {
//                if (existing.name == ingredient.name ||
//                    existing.gameObject.name == ingredient.gameObject.name)
//                {
//                    Debug.Log("Duplicate ingredient not allowed!");
//                    return false;
//                }
//            }
//        }

//        return true;
//    }

//    void PositionIngredientOnPlate(DraggableIngredient ingredient)
//    {
//        Vector3 plateCenter = transform.position;
//        int index = ingredientsOnPlate.Count - 1;

//        float angle = index * 60f * Mathf.Deg2Rad;
//        float radius = 0.3f + (index / 6f) * 0.2f;

//        Vector3 offset = new Vector3(
//            Mathf.Cos(angle) * radius,
//            Mathf.Sin(angle) * radius,
//            -0.1f
//        );

//        ingredient.transform.position = plateCenter + offset;
//    }

//    void RepositionIngredients()
//    {
//        for (int i = 0; i < ingredientsOnPlate.Count; i++)
//        {
//            if (ingredientsOnPlate[i] != null)
//            {
//                var temp = ingredientsOnPlate[i];
//                ingredientsOnPlate.RemoveAt(i);
//                ingredientsOnPlate.Insert(i, temp);

//                Vector3 plateCenter = transform.position;
//                float angle = i * 60f * Mathf.Deg2Rad;
//                float radius = 0.3f + (i / 6f) * 0.2f;

//                Vector3 offset = new Vector3(
//                    Mathf.Cos(angle) * radius,
//                    Mathf.Sin(angle) * radius,
//                    -0.1f
//                );

//                ingredientsOnPlate[i].transform.position = plateCenter + offset;
//                ingredientsOnPlate[i].SetNewOriginalPosition();
//            }
//        }
//    }

//    void OnMouseEnter()
//    {
//        ShowHighlight();
//    }

//    void OnMouseExit()
//    {
//        HideHighlight();
//    }

//    void ShowHighlight()
//    {
//        if (highlightEffect != null)
//            highlightEffect.SetActive(true);
//        else if (plateRenderer != null)
//            plateRenderer.color = highlightColor;
//    }

//    void HideHighlight()
//    {
//        if (highlightEffect != null)
//            highlightEffect.SetActive(false);
//        else if (plateRenderer != null)
//            plateRenderer.color = originalColor;
//    }

//    public bool IsFull() => ingredientsOnPlate.Count >= maxIngredients;
//    public bool IsEmpty() => ingredientsOnPlate.Count == 0;
//    public int GetIngredientCount() => ingredientsOnPlate.Count;
//    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsOnPlate);

//    public void ClearPlate()
//    {
//        while (ingredientsOnPlate.Count > 0)
//        {
//            RemoveIngredient(ingredientsOnPlate[0]);
//        }

//        // remove every child under plate, even if they aren't ingredients
//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }
//    }

//    public List<string> GetIngredientNames()
//    {
//        List<string> names = new List<string>();
//        foreach (DraggableIngredient ingredient in ingredientsOnPlate)
//        {
//            names.Add(ingredient.name);
//        }
//        return names;
//    }

//    public string getDishName()
//    {
//        if (IsEmpty())
//            return "Empty Plate";

//        foreach (Transform child in transform)
//        {
//            GameObject childObject = child.gameObject;
//            Debug.Log("Child object under plate: " + childObject.name);
//            return childObject.name;
//        }

//        return "";
//    }
//}



















//using UnityEngine;
//using System.Collections.Generic;

//public class Plate : MonoBehaviour, IDropZone
//{
//    [Header("Plate Settings")]
//    public Transform ingredientParent;
//    public float maxIngredients = 100f;
//    public bool allowDuplicates = true;
//    public float ingredientSpacing = 0.2f;
//    public CombinationSystem comboSystem;

//    [Header("Visual Feedback")]
//    public GameObject highlightEffect;
//    public Color highlightColor;

//    private List<DraggableIngredient> ingredientsOnPlate;
//    private SpriteRenderer plateRenderer;
//    private Color originalColor;

//    // Events
//    public System.Action<DraggableIngredient> OnIngredientAdded;
//    public System.Action<DraggableIngredient> OnIngredientRemoved;
//    public System.Action OnPlateFull;
//    public System.Action OnPlateEmpty;

//    void Start()
//    {
//        ingredientsOnPlate = new List<DraggableIngredient>();
//        highlightColor = Color.yellow;

//        plateRenderer = GetComponent<SpriteRenderer>();
//        if (plateRenderer != null) originalColor = plateRenderer.color;

//        if (ingredientParent == null) ingredientParent = transform;
//        if (highlightEffect != null) highlightEffect.SetActive(false);
//    }

//    public GameObject GetGameObject() => gameObject;

//    public bool AddIngredient(DraggableIngredient ingredient)
//    {
//        if (!CanAddIngredient(ingredient)) return false;

//        ingredientsOnPlate.Add(ingredient);
//        PositionIngredientOnPlate(ingredient);

//        if (ingredientParent != null) ingredient.transform.SetParent(ingredientParent);

//        OnIngredientAdded?.Invoke(ingredient);
//        if (IsFull()) OnPlateFull?.Invoke();
//        if (ingredientsOnPlate.Count > 1 && comboSystem != null) comboSystem.CheckForCombinations();

//        return true;
//    }

//    public bool RemoveIngredient(DraggableIngredient ingredient)
//    {
//        if (!ingredientsOnPlate.Contains(ingredient)) return false;
//        bool wasEmpty = IsEmpty();

//        ingredientsOnPlate.Remove(ingredient);
//        ingredient.transform.SetParent(null);
//        Destroy(ingredient.gameObject);
//        RepositionIngredients();

//        OnIngredientRemoved?.Invoke(ingredient);
//        if (!wasEmpty && IsEmpty()) OnPlateEmpty?.Invoke();
//        return true;
//    }

//    bool CanAddIngredient(DraggableIngredient ingredient)
//    {
//        if (IsFull()) return false;

//        if (!allowDuplicates)
//        {
//            foreach (var existing in ingredientsOnPlate)
//            {
//                if (existing.name == ingredient.name ||
//                    existing.gameObject.name == ingredient.gameObject.name)
//                    return false;
//            }
//        }
//        return true;
//    }

//    void PositionIngredientOnPlate(DraggableIngredient ingredient)
//    {
//        Vector3 plateCenter = transform.position;
//        int index = ingredientsOnPlate.Count - 1;

//        float angle = index * 60f * Mathf.Deg2Rad;
//        float radius = 0.3f + (index / 6f) * 0.2f;

//        Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);
//        ingredient.transform.position = plateCenter + offset;
//    }

//    void RepositionIngredients()
//    {
//        for (int i = 0; i < ingredientsOnPlate.Count; i++)
//        {
//            if (ingredientsOnPlate[i] == null) continue;
//            var temp = ingredientsOnPlate[i];
//            ingredientsOnPlate[i] = temp;

//            Vector3 plateCenter = transform.position;
//            float angle = i * 60f * Mathf.Deg2Rad;
//            float radius = 0.3f + (i / 6f) * 0.2f;
//            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);

//            ingredientsOnPlate[i].transform.position = plateCenter + offset;
//            ingredientsOnPlate[i].SetNewOriginalPosition();
//        }
//    }

//    void OnMouseEnter() => ShowHighlight();
//    void OnMouseExit() => HideHighlight();

//    void ShowHighlight()
//    {
//        if (highlightEffect != null) highlightEffect.SetActive(true);
//        else if (plateRenderer != null) plateRenderer.color = highlightColor;
//    }

//    void HideHighlight()
//    {
//        if (highlightEffect != null) highlightEffect.SetActive(false);
//        else if (plateRenderer != null) plateRenderer.color = originalColor;
//    }

//    public bool IsFull() => ingredientsOnPlate.Count >= maxIngredients;
//    public bool IsEmpty() => ingredientsOnPlate.Count == 0;
//    public int GetIngredientCount() => ingredientsOnPlate.Count;
//    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsOnPlate);

//    public void ClearPlate()
//    {
//        while (ingredientsOnPlate.Count > 0) RemoveIngredient(ingredientsOnPlate[0]);
//        foreach (Transform child in transform) Destroy(child.gameObject);
//    }

//    public List<string> GetIngredientNames()
//    {
//        List<string> names = new List<string>();
//        foreach (var ingredient in ingredientsOnPlate) names.Add(ingredient.name);
//        return names;
//    }

//    public string getDishName()
//    {
//        if (IsEmpty()) return "Empty Plate";
//        foreach (Transform child in transform) return child.gameObject.name;
//        return "";
//    }
//}




















//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI; // ✅ Add this for UI Image support

//public class Plate : MonoBehaviour, IDropZone
//{
//    [Header("Plate Settings")]
//    public Transform ingredientParent;
//    public float maxIngredients = 100f;
//    public bool allowDuplicates = true;
//    public float ingredientSpacing = 0.2f;
//    public CombinationSystem comboSystem;

//    [Header("Visual Feedback")]
//    public GameObject highlightEffect;
//    public Color highlightColor;

//    [Header("Color Change Settings")]
//    public Color emptyPlateColor = Color.white;
//    public Color fullPlateColor = new Color(0.6f, 0.8f, 1f); // Light blue when full
//    public bool useGradualColorChange = true;

//    private List<DraggableIngredient> ingredientsOnPlate;
//    private SpriteRenderer plateRenderer;
//    private Image plateImage; // ✅ For UI plates
//    private Color originalColor;
//    private bool isUIPlate = false; // ✅ Track if this is a UI element

//    // Events
//    public System.Action<DraggableIngredient> OnIngredientAdded;
//    public System.Action<DraggableIngredient> OnIngredientRemoved;
//    public System.Action OnPlateFull;
//    public System.Action OnPlateEmpty;

//    void Start()
//    {
//        ingredientsOnPlate = new List<DraggableIngredient>();
//        highlightColor = Color.yellow;

//        // ✅ Check if this is a SpriteRenderer or UI Image
//        plateRenderer = GetComponent<SpriteRenderer>();
//        plateImage = GetComponent<Image>();

//        if (plateRenderer != null)
//        {
//            originalColor = plateRenderer.color;
//            emptyPlateColor = originalColor;
//            isUIPlate = false;
//            Debug.Log($"[Plate] Found SpriteRenderer, original color: {originalColor}");
//        }
//        else if (plateImage != null)
//        {
//            originalColor = plateImage.color;
//            emptyPlateColor = originalColor;
//            isUIPlate = true;
//            Debug.Log($"[Plate] Found UI Image, original color: {originalColor}");
//        }
//        else
//        {
//            Debug.LogError($"[Plate] No SpriteRenderer or Image component found on {gameObject.name}!");
//        }

//        if (ingredientParent == null) ingredientParent = transform;
//        if (highlightEffect != null) highlightEffect.SetActive(false);

//        Debug.Log($"[Plate] Initialized. Empty color: {emptyPlateColor}, Full color: {fullPlateColor}");
//    }

//    public GameObject GetGameObject() => gameObject;

//    public bool AddIngredient(DraggableIngredient ingredient)
//    {
//        if (!CanAddIngredient(ingredient)) return false;

//        ingredientsOnPlate.Add(ingredient);
//        PositionIngredientOnPlate(ingredient);

//        if (ingredientParent != null) ingredient.transform.SetParent(ingredientParent);

//        // ✅ Change plate color when ingredient is added
//        Debug.Log($"[Plate] Adding ingredient. Count: {ingredientsOnPlate.Count}");
//        UpdatePlateColor();

//        OnIngredientAdded?.Invoke(ingredient);
//        if (IsFull()) OnPlateFull?.Invoke();
//        if (ingredientsOnPlate.Count > 1 && comboSystem != null) comboSystem.CheckForCombinations();

//        return true;
//    }

//    public bool RemoveIngredient(DraggableIngredient ingredient)
//    {
//        if (!ingredientsOnPlate.Contains(ingredient)) return false;
//        bool wasEmpty = IsEmpty();

//        ingredientsOnPlate.Remove(ingredient);
//        ingredient.transform.SetParent(null);
//        Destroy(ingredient.gameObject);
//        RepositionIngredients();

//        // ✅ Update plate color after removal
//        Debug.Log($"[Plate] Removing ingredient. Count: {ingredientsOnPlate.Count}");
//        UpdatePlateColor();

//        OnIngredientRemoved?.Invoke(ingredient);
//        if (!wasEmpty && IsEmpty()) OnPlateEmpty?.Invoke();
//        return true;
//    }

//    // ✅ UPDATED METHOD: Works with both SpriteRenderer and UI Image
//    private void UpdatePlateColor()
//    {
//        Color targetColor;

//        if (ingredientsOnPlate.Count == 0)
//        {
//            targetColor = emptyPlateColor;
//            Debug.Log($"[Plate] Setting to empty color: {targetColor}");
//        }
//        else
//        {
//            if (useGradualColorChange)
//            {
//                float fillRatio = Mathf.Clamp01(ingredientsOnPlate.Count / maxIngredients);
//                targetColor = Color.Lerp(emptyPlateColor, fullPlateColor, fillRatio);
//                Debug.Log($"[Plate] Gradual color. Count: {ingredientsOnPlate.Count}, Ratio: {fillRatio}, Color: {targetColor}");
//            }
//            else
//            {
//                targetColor = fullPlateColor;
//                Debug.Log($"[Plate] Setting to full color: {targetColor}");
//            }
//        }

//        // ✅ Apply color to the correct component
//        if (plateRenderer != null)
//        {
//            plateRenderer.color = targetColor;
//        }
//        else if (plateImage != null)
//        {
//            plateImage.color = targetColor;
//        }
//    }

//    bool CanAddIngredient(DraggableIngredient ingredient)
//    {
//        if (IsFull()) return false;

//        if (!allowDuplicates)
//        {
//            foreach (var existing in ingredientsOnPlate)
//            {
//                if (existing.name == ingredient.name ||
//                    existing.gameObject.name == ingredient.gameObject.name)
//                    return false;
//            }
//        }
//        return true;
//    }

//    void PositionIngredientOnPlate(DraggableIngredient ingredient)
//    {
//        Vector3 plateCenter = transform.position;
//        int index = ingredientsOnPlate.Count - 1;

//        float angle = index * 60f * Mathf.Deg2Rad;
//        float radius = 0.3f + (index / 6f) * 0.2f;

//        Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);
//        ingredient.transform.position = plateCenter + offset;
//    }

//    void RepositionIngredients()
//    {
//        for (int i = 0; i < ingredientsOnPlate.Count; i++)
//        {
//            if (ingredientsOnPlate[i] == null) continue;
//            var temp = ingredientsOnPlate[i];
//            ingredientsOnPlate[i] = temp;

//            Vector3 plateCenter = transform.position;
//            float angle = i * 60f * Mathf.Deg2Rad;
//            float radius = 0.3f + (i / 6f) * 0.2f;
//            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);

//            ingredientsOnPlate[i].transform.position = plateCenter + offset;
//            ingredientsOnPlate[i].SetNewOriginalPosition();
//        }
//    }

//    void OnMouseEnter() => ShowHighlight();
//    void OnMouseExit() => HideHighlight();

//    void ShowHighlight()
//    {
//        if (highlightEffect != null)
//        {
//            highlightEffect.SetActive(true);
//        }
//        else if (plateRenderer != null)
//        {
//            plateRenderer.color = highlightColor;
//        }
//        else if (plateImage != null)
//        {
//            plateImage.color = highlightColor;
//        }
//    }

//    void HideHighlight()
//    {
//        if (highlightEffect != null)
//        {
//            highlightEffect.SetActive(false);
//        }
//        else
//        {
//            UpdatePlateColor();
//        }
//    }

//    public bool IsFull() => ingredientsOnPlate.Count >= maxIngredients;
//    public bool IsEmpty() => ingredientsOnPlate.Count == 0;
//    public int GetIngredientCount() => ingredientsOnPlate.Count;
//    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsOnPlate);

//    public void ClearPlate()
//    {
//        while (ingredientsOnPlate.Count > 0)
//        {
//            RemoveIngredient(ingredientsOnPlate[0]);
//        }

//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }

//        UpdatePlateColor();
//    }

//    public List<string> GetIngredientNames()
//    {
//        List<string> names = new List<string>();
//        foreach (var ingredient in ingredientsOnPlate)
//        {
//            names.Add(ingredient.name);
//        }
//        return names;
//    }

//    public string getDishName()
//    {
//        if (IsEmpty()) return "Empty Plate";
//        foreach (Transform child in transform)
//        {
//            return child.gameObject.name;
//        }
//        return "";
//    }
//}























//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;

//public class Plate : MonoBehaviour, IDropZone
//{
//    [Header("Plate Settings")]
//    public Transform ingredientParent;
//    [Tooltip("Maximum number of ingredients before plate is full")]
//    public float maxIngredients = 5f; // ✅ Changed from 100 to 5 for visible color changes
//    public bool allowDuplicates = true;
//    public float ingredientSpacing = 0.2f;
//    public CombinationSystem comboSystem;

//    [Header("Visual Feedback")]
//    public GameObject highlightEffect;
//    public Color highlightColor = Color.yellow;

//    [Header("Color Change Settings")]
//    [Tooltip("Color when plate is empty")]
//    public Color emptyPlateColor = Color.white;
//    [Tooltip("Color when plate is full")]
//    public Color fullPlateColor = new Color(0.3f, 0.6f, 1f); // ✅ More visible blue color
//    [Tooltip("If true, color gradually changes. If false, changes instantly when food is added")]
//    public bool useGradualColorChange = true;

//    private List<DraggableIngredient> ingredientsOnPlate;
//    private SpriteRenderer plateRenderer;
//    private Image plateImage;
//    private Color originalColor;
//    private bool isUIPlate = false;

//    // Events
//    public System.Action<DraggableIngredient> OnIngredientAdded;
//    public System.Action<DraggableIngredient> OnIngredientRemoved;
//    public System.Action OnPlateFull;
//    public System.Action OnPlateEmpty;

//    void Start()
//    {
//        ingredientsOnPlate = new List<DraggableIngredient>();

//        // ✅ Check if this is a SpriteRenderer or UI Image
//        plateRenderer = GetComponent<SpriteRenderer>();
//        plateImage = GetComponent<Image>();

//        if (plateRenderer != null)
//        {
//            originalColor = plateRenderer.color;
//            emptyPlateColor = originalColor;
//            isUIPlate = false;
//            Debug.Log($"[Plate] Found SpriteRenderer, original color: {originalColor}");
//        }
//        else if (plateImage != null)
//        {
//            originalColor = plateImage.color;
//            emptyPlateColor = originalColor;
//            isUIPlate = true;
//            Debug.Log($"[Plate] Found UI Image, original color: {originalColor}");
//        }
//        else
//        {
//            Debug.LogWarning($"[Plate] No SpriteRenderer or Image component found on {gameObject.name}! Color changes won't work.");
//        }

//        if (ingredientParent == null) ingredientParent = transform;
//        if (highlightEffect != null) highlightEffect.SetActive(false);

//        Debug.Log($"[Plate] Initialized. Empty color: {emptyPlateColor}, Full color: {fullPlateColor}, Max ingredients: {maxIngredients}");
//    }

//    public GameObject GetGameObject() => gameObject;

//    public bool AddIngredient(DraggableIngredient ingredient)
//    {
//        if (!CanAddIngredient(ingredient))
//        {
//            Debug.Log($"[Plate] Cannot add ingredient - plate full or duplicate");
//            return false;
//        }

//        ingredientsOnPlate.Add(ingredient);
//        PositionIngredientOnPlate(ingredient);

//        if (ingredientParent != null) ingredient.transform.SetParent(ingredientParent);

//        // ✅ Change plate color when ingredient is added
//        UpdatePlateColor();

//        OnIngredientAdded?.Invoke(ingredient);
//        if (IsFull()) OnPlateFull?.Invoke();
//        if (ingredientsOnPlate.Count > 1 && comboSystem != null) comboSystem.CheckForCombinations();

//        return true;
//    }

//    public bool RemoveIngredient(DraggableIngredient ingredient)
//    {
//        if (!ingredientsOnPlate.Contains(ingredient)) return false;
//        bool wasEmpty = IsEmpty();

//        ingredientsOnPlate.Remove(ingredient);
//        ingredient.transform.SetParent(null);
//        Destroy(ingredient.gameObject);
//        RepositionIngredients();

//        // ✅ Update plate color after removal
//        UpdatePlateColor();

//        OnIngredientRemoved?.Invoke(ingredient);
//        if (!wasEmpty && IsEmpty()) OnPlateEmpty?.Invoke();
//        return true;
//    }

//    // ✅ UPDATED METHOD: More visible color changes with better ratio calculation
//    private void UpdatePlateColor()
//    {
//        // Don't change color if we're currently showing highlight
//        if (highlightEffect != null && highlightEffect.activeSelf) return;

//        Color targetColor;

//        if (ingredientsOnPlate.Count == 0)
//        {
//            targetColor = emptyPlateColor;
//            Debug.Log($"[Plate] Plate empty - setting to empty color: {targetColor}");
//        }
//        else
//        {
//            if (useGradualColorChange)
//            {
//                // Calculate fill ratio - capped at 1.0 even if over max
//                float fillRatio = Mathf.Clamp01(ingredientsOnPlate.Count / maxIngredients);
//                targetColor = Color.Lerp(emptyPlateColor, fullPlateColor, fillRatio);
//                Debug.Log($"[Plate] Gradual color change. Ingredients: {ingredientsOnPlate.Count}/{maxIngredients}, Ratio: {fillRatio:F2}, Color: {targetColor}");
//            }
//            else
//            {
//                // Instant color change - full color as soon as any food is added
//                targetColor = fullPlateColor;
//                Debug.Log($"[Plate] Instant color change. Ingredients: {ingredientsOnPlate.Count}, Color: {targetColor}");
//            }
//        }

//        // ✅ Apply color to the correct component
//        if (plateRenderer != null)
//        {
//            plateRenderer.color = targetColor;
//            Debug.Log($"[Plate] Applied color to SpriteRenderer: {targetColor}");
//        }
//        else if (plateImage != null)
//        {
//            plateImage.color = targetColor;
//            Debug.Log($"[Plate] Applied color to UI Image: {targetColor}");
//        }
//        else
//        {
//            Debug.LogWarning($"[Plate] Cannot apply color - no renderer found!");
//        }
//    }

//    bool CanAddIngredient(DraggableIngredient ingredient)
//    {
//        if (IsFull())
//        {
//            Debug.Log($"[Plate] Plate is full ({ingredientsOnPlate.Count}/{maxIngredients})");
//            return false;
//        }

//        if (!allowDuplicates)
//        {
//            foreach (var existing in ingredientsOnPlate)
//            {
//                if (existing.name == ingredient.name ||
//                    existing.gameObject.name == ingredient.gameObject.name)
//                {
//                    Debug.Log($"[Plate] Duplicate ingredient rejected: {ingredient.name}");
//                    return false;
//                }
//            }
//        }
//        return true;
//    }

//    void PositionIngredientOnPlate(DraggableIngredient ingredient)
//    {
//        Vector3 plateCenter = transform.position;
//        int index = ingredientsOnPlate.Count - 1;

//        float angle = index * 60f * Mathf.Deg2Rad;
//        float radius = 0.3f + (index / 6f) * 0.2f;

//        Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);
//        ingredient.transform.position = plateCenter + offset;
//    }

//    void RepositionIngredients()
//    {
//        for (int i = 0; i < ingredientsOnPlate.Count; i++)
//        {
//            if (ingredientsOnPlate[i] == null) continue;

//            Vector3 plateCenter = transform.position;
//            float angle = i * 60f * Mathf.Deg2Rad;
//            float radius = 0.3f + (i / 6f) * 0.2f;
//            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);

//            ingredientsOnPlate[i].transform.position = plateCenter + offset;
//            ingredientsOnPlate[i].SetNewOriginalPosition();
//        }
//    }

//    void OnMouseEnter() => ShowHighlight();
//    void OnMouseExit() => HideHighlight();

//    void ShowHighlight()
//    {
//        if (highlightEffect != null)
//        {
//            highlightEffect.SetActive(true);
//        }
//        else if (plateRenderer != null)
//        {
//            plateRenderer.color = highlightColor;
//        }
//        else if (plateImage != null)
//        {
//            plateImage.color = highlightColor;
//        }
//    }

//    void HideHighlight()
//    {
//        if (highlightEffect != null)
//        {
//            highlightEffect.SetActive(false);
//        }
//        else
//        {
//            // ✅ Restore appropriate color based on current state
//            UpdatePlateColor();
//        }
//    }

//    public bool IsFull() => ingredientsOnPlate.Count >= maxIngredients;
//    public bool IsEmpty() => ingredientsOnPlate.Count == 0;
//    public int GetIngredientCount() => ingredientsOnPlate.Count;
//    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsOnPlate);

//    public void ClearPlate()
//    {
//        while (ingredientsOnPlate.Count > 0)
//        {
//            RemoveIngredient(ingredientsOnPlate[0]);
//        }

//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }

//        UpdatePlateColor();
//        Debug.Log($"[Plate] Plate cleared");
//    }

//    public List<string> GetIngredientNames()
//    {
//        List<string> names = new List<string>();
//        foreach (var ingredient in ingredientsOnPlate)
//        {
//            names.Add(ingredient.name);
//        }
//        return names;
//    }

//    public string getDishName()
//    {
//        if (IsEmpty()) return "Empty Plate";
//        foreach (Transform child in transform)
//        {
//            return child.gameObject.name;
//        }
//        return "";
//    }
//}
































using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Plate : MonoBehaviour, IDropZone
{
    [Header("Plate Settings")]
    public Transform ingredientParent;
    [Tooltip("Maximum number of ingredients before plate is full")]
    public float maxIngredients = 5f;
    public bool allowDuplicates = true;
    public float ingredientSpacing = 0.2f;
    public CombinationSystem comboSystem;

    [Header("Visual Feedback")]
    public GameObject highlightEffect;
    public Color highlightColor;
    [Header("Dish Display Settings")]
    public TextMeshProUGUI dishIngredientsText; // Text component to show dish ingredients
    public float maxTextWidth = 200f; // Maximum width for text box
    public float minFontSize = 12f;
    public float maxFontSize = 24f;
    private List<DraggableIngredient> ingredientsOnPlate; // make sure to take it out if the ingredient is removed
    private SpriteRenderer plateRenderer;
    private Image plateImage;
    private Color originalColor;
    private bool isUIPlate = false;

    // Events
    public System.Action<DraggableIngredient> OnIngredientAdded;
    public System.Action<DraggableIngredient> OnIngredientRemoved;
    public System.Action OnPlateFull;
    public System.Action OnPlateEmpty;

    void Start()
    {
        ingredientsOnPlate = new List<DraggableIngredient>();

        // ✅ Check for UI Image FIRST (prioritize UI)
        plateImage = GetComponent<Image>();

        // Only use SpriteRenderer if Image doesn't exist AND sprite is assigned
        if (plateImage == null)
        {
            plateRenderer = GetComponent<SpriteRenderer>();
            if (plateRenderer != null && plateRenderer.sprite == null)
            {
                plateRenderer = null; // Ignore empty SpriteRenderer
                Debug.Log("[Plate] Found SpriteRenderer but no sprite assigned, ignoring it");
            }
        }

        if (plateImage != null)
        {
            originalColor = plateImage.color;
            emptyPlateColor = originalColor;
            isUIPlate = true;
            Debug.Log($"[Plate] Using UI Image, original color: {originalColor}");
        }
        else if (plateRenderer != null)
        {
            originalColor = plateRenderer.color;

        if (ingredientParent == null)
            ingredientParent = transform;

        if (highlightEffect != null)
            highlightEffect.SetActive(false);

        UpdateDishDisplay(); // Initialize display
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool AddIngredient(DraggableIngredient ingredient)
    {
        if (!CanAddIngredient(ingredient))
        {
            Debug.Log($"[Plate] Cannot add ingredient - plate full or duplicate");
            return false;
        }

        ingredientsOnPlate.Add(ingredient);
        //PositionIngredientOnPlate(ingredient);

        if (ingredientParent != null)
            ingredient.transform.SetParent(ingredientParent);

        OnIngredientAdded?.Invoke(ingredient);
        if (IsFull()) OnPlateFull?.Invoke();
        if (ingredientsOnPlate.Count > 1 && comboSystem != null) comboSystem.CheckForCombinations();

        UpdateDishDisplay(); // Update display after adding ingredient

        Debug.Log($"Added {ingredient.name} to plate. Total ingredients: {ingredientsOnPlate.Count}");
        return true;
    }

    public bool RemoveIngredient(DraggableIngredient ingredient)
    {
        if (!ingredientsOnPlate.Contains(ingredient)) return false;
        bool wasEmpty = IsEmpty();

        ingredientsOnPlate.Remove(ingredient);
        ingredient.transform.SetParent(null);
        Destroy(ingredient.gameObject);
        //RepositionIngredients();

        OnIngredientRemoved?.Invoke(ingredient);

        if (!wasEmpty && IsEmpty())
            OnPlateEmpty?.Invoke();

        UpdateDishDisplay(); // Update display after removing ingredient

        Debug.Log($"Removed {ingredient.name} from plate. Total ingredients: {ingredientsOnPlate.Count}");
        return true;
    }

    // ✅ UPDATED METHOD: More visible color changes with better ratio calculation
    private void UpdatePlateColor()
    {
        // Don't change color if we're currently showing highlight
        if (highlightEffect != null && highlightEffect.activeSelf) return;

        Color targetColor;

        if (ingredientsOnPlate.Count == 0)
        {
            targetColor = emptyPlateColor;
            Debug.Log($"[Plate] Plate empty - setting to empty color: {targetColor}");
        }
        else
        {
            if (useGradualColorChange)
            {
                // Calculate fill ratio - capped at 1.0 even if over max
                float fillRatio = Mathf.Clamp01(ingredientsOnPlate.Count / maxIngredients);
                targetColor = Color.Lerp(emptyPlateColor, fullPlateColor, fillRatio);
                Debug.Log($"[Plate] Gradual color change. Ingredients: {ingredientsOnPlate.Count}/{maxIngredients}, Ratio: {fillRatio:F2}, Color: {targetColor}");
            }
            else
            {
                // Instant color change - full color as soon as any food is added
                targetColor = fullPlateColor;
                Debug.Log($"[Plate] Instant color change. Ingredients: {ingredientsOnPlate.Count}, Color: {targetColor}");
            }
        }

        // ✅ Apply color to the correct component
        if (plateImage != null)
        {
            plateImage.color = targetColor;
            Debug.Log($"[Plate] Applied color to UI Image: {targetColor}");
        }
        else if (plateRenderer != null)
        {
            plateRenderer.color = targetColor;
            Debug.Log($"[Plate] Applied color to SpriteRenderer: {targetColor}");
        }
        else
        {
            Debug.LogWarning($"[Plate] Cannot apply color - no renderer found!");
        }
    }

    bool CanAddIngredient(DraggableIngredient ingredient)
    {
        if (IsFull())
        {
            Debug.Log($"[Plate] Plate is full ({ingredientsOnPlate.Count}/{maxIngredients})");
            return false;
        }

        if (!allowDuplicates)
        {
            foreach (var existing in ingredientsOnPlate)
            {
                if (existing.name == ingredient.name ||
                    existing.gameObject.name == ingredient.gameObject.name)
                {
                    Debug.Log($"[Plate] Duplicate ingredient rejected: {ingredient.name}");
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Should rearrange the ingredients from top down left right in a circular pattern
    /// no more than 4 ingredients on the plate, so the position shouldn't be overlapping
    /// </summary>
    void PositionIngredientOnPlate(DraggableIngredient ingredient)
    {
        Vector3 plateCenter = transform.position;
        int index = ingredientsOnPlate.Count - 1;

        float angle = index * 60f * Mathf.Deg2Rad;
        float radius = 0.3f + (index / 6f) * 0.2f;

        Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);
        ingredient.transform.position = plateCenter + offset;
    }

    void RepositionIngredients()
    {
        for (int i = 0; i < ingredientsOnPlate.Count; i++)
        {
            if (ingredientsOnPlate[i] == null) continue;

            Vector3 plateCenter = transform.position;
            float angle = i * 60f * Mathf.Deg2Rad;
            float radius = 0.3f + (i / 6f) * 0.2f;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, -0.1f);

                ingredientsOnPlate[i].transform.position = plateCenter + offset;
            }
        }
    }

    void OnMouseEnter() => ShowHighlight();
    void OnMouseExit() => HideHighlight();

    void ShowHighlight()
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
        }
        else if (plateImage != null)
        {
            plateImage.color = highlightColor;
        }
        else if (plateRenderer != null)
        {
            plateRenderer.color = highlightColor;
        }
    }

    void HideHighlight()
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
        else
        {
            // ✅ Restore appropriate color based on current state
            UpdatePlateColor();
        }
    }

    public bool IsFull() => ingredientsOnPlate.Count >= maxIngredients;
    public bool IsEmpty() => ingredientsOnPlate.Count == 0;
    public int GetIngredientCount() => ingredientsOnPlate.Count;
    public List<DraggableIngredient> GetIngredients() => new List<DraggableIngredient>(ingredientsOnPlate);

    public void ClearPlate()
    {
        while (ingredientsOnPlate.Count > 0)
        {
            RemoveIngredient(ingredientsOnPlate[0]);
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        UpdateDishDisplay(); // Update display after clearing
    }

    public void UpdateDishDisplay()
    {
        if (dishIngredientsText == null)
            return;

        // Check if a recipe is formed
        Recipe matchedRecipe = GetMatchingRecipe();

        if (matchedRecipe != null)
        {
            // Show recipe name if found
            dishIngredientsText.text = matchedRecipe.dishName;
            AdjustTextSize(matchedRecipe.dishName);
        }
        else if (IsEmpty())
        {
            // Show "Dish Empty" when no ingredients
            dishIngredientsText.text = "Dish Empty";
            dishIngredientsText.fontSize = maxFontSize;
        }
        else
        {
            // Show ingredient names separated by " + "
            string displayText = GetIngredientsDisplayText();
            dishIngredientsText.text = displayText;
            AdjustTextSize(displayText);
        }
    }
    
    private string GetIngredientsDisplayText()
    {
        if (IsEmpty())
            return "Dish Empty";

        List<string> ingredientNames = new List<string>();
        
        // Get ingredient names from the plate
        foreach (Transform child in ingredientParent)
        {
            Ingredient ingredient = child.GetComponent<Ingredient>();
            if (ingredient != null && ingredient.ingredientData != null)
            {
                ingredientNames.Add(ingredient.ingredientData.ingredientName);
            }
            else
            {
                // Fallback to object name if no Ingredient component
                ingredientNames.Add(child.gameObject.name);
            }
        }

        return string.Join(" + ", ingredientNames);
    }

    /// <summary>
    /// Adjusts text size to fit within the specified width
    /// </summary>
    private void AdjustTextSize(string text)
    {
        if (dishIngredientsText == null)
            return;

        // Start with max font size
        dishIngredientsText.fontSize = maxFontSize;
        dishIngredientsText.text = text;

        // Force update to calculate size
        dishIngredientsText.ForceMeshUpdate();

        // Get the preferred width of the text
        float textWidth = dishIngredientsText.preferredWidth;

        // If text is too wide, reduce font size
        if (textWidth > maxTextWidth)
        {
            float scaleFactor = maxTextWidth / textWidth;
            float newFontSize = Mathf.Max(minFontSize, maxFontSize * scaleFactor);
            dishIngredientsText.fontSize = newFontSize;
        }
    }

    /// <summary>
    /// Checks if current ingredients match any recipe
    /// </summary>
    private Recipe GetMatchingRecipe()
    {
        if (comboSystem == null || ingredientParent == null)
            return null;

        List<Ingredient> ingredients = new List<Ingredient>();

        // Get all ingredients from the plate
        foreach (Transform child in ingredientParent)
        {
            Ingredient ing = child.GetComponent<Ingredient>();
            if (ing != null)
            {
                ingredients.Add(ing);
            }
        }

        if (ingredients.Count == 0)
            return null;

        // Check all recipes to find a match
        foreach (Recipe recipe in comboSystem.allRecipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                return recipe;
            }
        }

        return null;
    }

    public List<string> GetIngredientNames()
    {
        List<string> names = new List<string>();
        foreach (var ingredient in ingredientsOnPlate)
        {
            names.Add(ingredient.name);
        }
        return names;
    }

    public string getDishName()
    {
        if (IsEmpty()) return "Empty Plate";
        foreach (Transform child in transform)
        {
            return child.gameObject.name;
        }
        return "";
    }
}