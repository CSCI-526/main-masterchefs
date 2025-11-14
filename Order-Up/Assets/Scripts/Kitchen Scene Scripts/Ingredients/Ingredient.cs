using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode] // Runs in editor without Play
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DraggableIngredient))]
public class Ingredient : MonoBehaviour
{
    [Header("Ingredient Settings")]
    public IngredientData ingredientData;
    public IngredientState currentState = IngredientState.Raw;
    public CookwareType currentCookware = CookwareType.None;

    private PolygonCollider2D polyCollider;
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
        if (polyCollider == null) polyCollider = GetComponent<PolygonCollider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ApplyData()
    {
        if (ingredientData != null && ingredientData.icon != null)
        {
            spriteRenderer.sprite = ingredientData.icon;
            gameObject.name = ingredientData.ingredientName;

            ResetCollider();

#if UNITY_EDITOR
            // Mark prefab dirty so Unity knows to save collider changes
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);
#endif
        }
    }

    private void ResetCollider()
    {
        if (polyCollider != null && spriteRenderer.sprite != null)
        {
            polyCollider.pathCount = 0; // clear old paths

            int shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
            if (shapeCount > 0)
            {
                polyCollider.pathCount = shapeCount;
                var path = new System.Collections.Generic.List<Vector2>();

                for (int i = 0; i < shapeCount; i++)
                {
                    path.Clear();
                    spriteRenderer.sprite.GetPhysicsShape(i, path);
                    polyCollider.SetPath(i, path.ToArray());
                }
            }
        }
    }

    // Public API for runtime sprite swaps
    public void SetSprite(Sprite newSprite, Color color, string newName = null)
    {
        spriteRenderer.sprite = newSprite;
        if (!string.IsNullOrEmpty(newName))
            gameObject.name = newName;
        spriteRenderer.color = color;
        ResetCollider();
    }

    // Public API for cookwares interact with ingredients
    public void CookIngredient()
    {
        if(ingredientData.cookedResult != null)
        {
            currentState = IngredientState.Cooked;
            Color newColor;
            ColorUtility.TryParseHtmlString("#00FF00", out newColor);
            SetSprite(ingredientData.cookedResult.icon, newColor, ingredientData.cookedResult.ingredientName);
        }
    }


        public void OvercookIngredient()
    {
        if(ingredientData.overcookedResult != null)
        {
            currentState = IngredientState.Overcooked;
            Color newColor;
            ColorUtility.TryParseHtmlString("#000000", out newColor);
            SetSprite(ingredientData.overcookedResult.icon, newColor, ingredientData.overcookedResult.ingredientName);
        }
    }

    public string IngredientName => ingredientData != null ? ingredientData.ingredientName : name;
}
