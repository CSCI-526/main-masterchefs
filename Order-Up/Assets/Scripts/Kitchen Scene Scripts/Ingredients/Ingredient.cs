using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode] // Runs in editor without Play
[RequireComponent(typeof(DraggableIngredient))]
public class Ingredient : MonoBehaviour
{
    [Header("Ingredient Settings")]
    public IngredientData ingredientData;
    public IngredientState currentState = IngredientState.Raw;
    public CookwareType currentCookware = CookwareType.None;

    private CircleCollider2D Collider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        Init();
        ApplyData(); // Runtime initialization
    }

    void OnValidate()
    {
        Init();
        ApplyData(); // Runs whenever values change in Inspector (edit or play)
    }

    private void Init()
    {
        if (Collider == null) Collider = GetComponent<CircleCollider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ApplyData()
    {
        if (ingredientData != null && ingredientData.icon != null)
        {
            spriteRenderer.sprite = ingredientData.icon;
            gameObject.name = ingredientData.ingredientName;

#if UNITY_EDITOR
            // Mark prefab dirty so Unity knows to save collider changes
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);
#endif
        }
    }

    private void ResetCollider()
    {
        if (Collider != null && spriteRenderer.sprite != null)
        {
            // Disable and re-enable the collider to force Unity to refresh it
            Collider.enabled = false;
            Collider.enabled = true;
        }
    }

    // public method to declare the only cookware type this ingredient can interact with
    public bool CheckAllowedCookware(CookwareType cookwareType)
    {
        if (currentCookware == cookwareType)
            return true;
        else
            return false;
    }


    // Public API for runtime sprite swaps
    public void SetSprite(Sprite newSprite, string newName = null)
    {
        spriteRenderer.sprite = newSprite;
        if (!string.IsNullOrEmpty(newName))
            gameObject.name = newName;
        //ResetCollider();
    }

    // Public API for cookwares interact with ingredients
    public void CookIngredient()
    {
        if(ingredientData.cookedResult != null)
        {
            currentState = IngredientState.Cooked;
            SetSprite(ingredientData.cookedResult.icon, ingredientData.cookedResult.ingredientName);
        }
    }


        public void OvercookIngredient()
    {
        if(ingredientData.overcookedResult != null)
        {
            currentState = IngredientState.Overcooked;
            SetSprite(ingredientData.overcookedResult.icon, ingredientData.overcookedResult.ingredientName);
        }
    }

    public string IngredientName => ingredientData != null ? ingredientData.ingredientName : name;
}
