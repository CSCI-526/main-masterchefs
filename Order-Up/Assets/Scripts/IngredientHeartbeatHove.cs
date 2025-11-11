using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientHeartbeatHover : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How much bigger the ingredient gets (like a heart pumping)")]
    public float scaleMultiplier = 1.3f;

    [Tooltip("How fast the heartbeat happens (lower = slower, like a calm heartbeat)")]
    public float animationSpeed = 8f;

    [Header("Text Settings")]
    [Tooltip("The text that shows the ingredient name")]
    public TextMeshProUGUI ingredientText;

    [Tooltip("Text will fade in/out smoothly")]
    public float textFadeSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovering = false;
    private CanvasGroup textCanvasGroup;

    void Start()
    {
        originalScale = transform.localScale;

        if (ingredientText != null)
        {
            // Set up the CanvasGroup - like installing a complete invisibility cloak
            textCanvasGroup = ingredientText.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                textCanvasGroup = ingredientText.gameObject.AddComponent<CanvasGroup>();
            }

            // Start completely invisible
            textCanvasGroup.alpha = 0f;
            textCanvasGroup.blocksRaycasts = false; // Can't be clicked when invisible
            textCanvasGroup.interactable = false;   // Can't interact when invisible

            // ALSO set the text color alpha to 0 (belt and suspenders approach!)
            Color textColor = ingredientText.color;
            textColor.a = 0f;
            ingredientText.color = textColor;
        }
    }

    void Update()
    {
        // Smooth scale animation - like breathing
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        // Fade the text in or out
        if (textCanvasGroup != null)
        {
            float targetAlpha = isHovering ? 1f : 0f;
            textCanvasGroup.alpha = Mathf.Lerp(
                textCanvasGroup.alpha,
                targetAlpha,
                Time.deltaTime * textFadeSpeed
            );
        }
    }

    void OnMouseEnter()
    {
        isHovering = true;
        targetScale = originalScale * scaleMultiplier;
    }

    void OnMouseExit()
    {
        isHovering = false;
        targetScale = originalScale;
    }

    void OnTransformParentChanged()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }
}