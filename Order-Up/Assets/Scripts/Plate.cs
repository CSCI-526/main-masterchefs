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




using UnityEngine;
using System.Collections.Generic;

public class Plate : MonoBehaviour, IDropZone
{
    [Header("Plate Settings")]
    public Transform ingredientParent;
    public float maxIngredients = 100f;
    public bool allowDuplicates = true;
    public float ingredientSpacing = 0.2f;
    public CombinationSystem comboSystem;

    [Header("Visual Feedback")]
    public GameObject highlightEffect;
    public Color highlightColor;

    private List<DraggableIngredient> ingredientsOnPlate;
    private SpriteRenderer plateRenderer;
    private Color originalColor;
    private CookwareMaintenance maintenance; // Add this

    // Events
    public System.Action<DraggableIngredient> OnIngredientAdded;
    public System.Action<DraggableIngredient> OnIngredientRemoved;
    public System.Action OnPlateFull;
    public System.Action OnPlateEmpty;

    void Start()
    {
        ingredientsOnPlate = new List<DraggableIngredient>();
        highlightColor = Color.yellow;

        plateRenderer = GetComponent<SpriteRenderer>();
        if (plateRenderer != null)
            originalColor = plateRenderer.color;

        if (ingredientParent == null)
            ingredientParent = transform;

        if (highlightEffect != null)
            highlightEffect.SetActive(false);

        // Get the maintenance component
        maintenance = GetComponent<CookwareMaintenance>();
    }

    // IDropZone implementation
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool AddIngredient(DraggableIngredient ingredient)
    {
        if (!CanAddIngredient(ingredient))
            return false;

        ingredientsOnPlate.Add(ingredient);
        PositionIngredientOnPlate(ingredient);

        if (ingredientParent != null)
            ingredient.transform.SetParent(ingredientParent);

        // ADD THIS LINE: Make the dish dirty when ingredient is added
        if (maintenance != null)
        {
            maintenance.AddDirt(1);
        }

        OnIngredientAdded?.Invoke(ingredient);

        if (IsFull())
            OnPlateFull?.Invoke();

        if (ingredientsOnPlate.Count > 1)
            comboSystem.CheckForCombinations();

        Debug.Log($"Added {ingredient.name} to plate. Total ingredients: {ingredientsOnPlate.Count}");
        return true;
    }

    public bool RemoveIngredient(DraggableIngredient ingredient)
    {
        if (!ingredientsOnPlate.Contains(ingredient))
            return false;

        bool wasEmpty = IsEmpty();

        ingredientsOnPlate.Remove(ingredient);

        ingredient.transform.SetParent(null);

        Destroy(ingredient.gameObject);
        RepositionIngredients();

        // Optional: Call RemoveItem() to log that item was removed (color stays dirty)
        if (maintenance != null)
        {
            maintenance.RemoveItem();
        }

        OnIngredientRemoved?.Invoke(ingredient);

        if (!wasEmpty && IsEmpty())
            OnPlateEmpty?.Invoke();

        Debug.Log($"Removed {ingredient.name} from plate. Total ingredients: {ingredientsOnPlate.Count}");
        return true;
    }

    bool CanAddIngredient(DraggableIngredient ingredient)
    {
        if (IsFull())
        {
            Debug.Log("Plate is full!");
            return false;
        }

        if (!allowDuplicates)
        {
            foreach (DraggableIngredient existing in ingredientsOnPlate)
            {
                if (existing.name == ingredient.name ||
                    existing.gameObject.name == ingredient.gameObject.name)
                {
                    Debug.Log("Duplicate ingredient not allowed!");
                    return false;
                }
            }
        }

        return true;
    }

    void PositionIngredientOnPlate(DraggableIngredient ingredient)
    {
        Vector3 plateCenter = transform.position;
        int index = ingredientsOnPlate.Count - 1;

        float angle = index * 60f * Mathf.Deg2Rad;
        float radius = 0.3f + (index / 6f) * 0.2f;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            -0.1f
        );

        ingredient.transform.position = plateCenter + offset;
    }

    void RepositionIngredients()
    {
        for (int i = 0; i < ingredientsOnPlate.Count; i++)
        {
            if (ingredientsOnPlate[i] != null)
            {
                var temp = ingredientsOnPlate[i];
                ingredientsOnPlate.RemoveAt(i);
                ingredientsOnPlate.Insert(i, temp);

                Vector3 plateCenter = transform.position;
                float angle = i * 60f * Mathf.Deg2Rad;
                float radius = 0.3f + (i / 6f) * 0.2f;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    -0.1f
                );

                ingredientsOnPlate[i].transform.position = plateCenter + offset;
                ingredientsOnPlate[i].SetNewOriginalPosition();
            }
        }
    }

    void OnMouseEnter()
    {
        ShowHighlight();
    }

    void OnMouseExit()
    {
        HideHighlight();
    }

    void ShowHighlight()
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(true);
        else if (plateRenderer != null)
            plateRenderer.color = highlightColor;
    }

    void HideHighlight()
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
        else if (plateRenderer != null)
            plateRenderer.color = originalColor;
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
    }

    public List<string> GetIngredientNames()
    {
        List<string> names = new List<string>();
        foreach (DraggableIngredient ingredient in ingredientsOnPlate)
        {
            names.Add(ingredient.name);
        }
        return names;
    }

    public string getDishName()
    {
        if (IsEmpty())
            return "Empty Plate";

        foreach (Transform child in transform)
        {
            GameObject childObject = child.gameObject;
            Debug.Log("Child object under plate: " + childObject.name);
            return childObject.name;
        }

        return "";
    }
}