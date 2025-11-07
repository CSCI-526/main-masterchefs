//using UnityEngine;
//using UnityEngine.UI;
//using TMPro; // If using TextMeshPro, otherwise use UnityEngine.UI for Text

//public class IngredientHeartbeatHover : MonoBehaviour
//{
//    [Header("Animation Settings")]
//    [Tooltip("How much bigger the ingredient gets (like a heart pumping)")]
//    public float scaleMultiplier = 1.3f; // The ingredient "inhales" to 130% size

//    [Tooltip("How fast the heartbeat happens (lower = slower, like a calm heartbeat)")]
//    public float animationSpeed = 8f;

//    [Header("Text Settings")]
//    [Tooltip("The text that shows the ingredient name")]
//    public TextMeshProUGUI ingredientText; // Or use 'Text' if not using TextMeshPro

//    [Tooltip("Text will fade in/out smoothly")]
//    public float textFadeSpeed = 5f;

//    // Private variables - like the ingredient's memory
//    private Vector3 originalScale; // Remember the ingredient's normal size
//    private Vector3 targetScale; // Where the ingredient wants to be (like a destination)
//    private bool isHovering = false; // If the mouse is nearby
//    private CanvasGroup textCanvasGroup; // Controls text visibility (like dimming lights)

//    void Start()
//    {
//        // Remember the original size - like taking a photo before the magic happens
//        originalScale = transform.localScale;
//        targetScale = originalScale;

//        // Set up the text to be invisible at first
//        if (ingredientText != null)
//        {
//            // Add CanvasGroup if it doesn't exist (like installing a dimmer switch)
//            textCanvasGroup = ingredientText.GetComponent<CanvasGroup>();
//            if (textCanvasGroup == null)
//            {
//                textCanvasGroup = ingredientText.gameObject.AddComponent<CanvasGroup>();
//            }
//            textCanvasGroup.alpha = 0f; // Start invisible
//        }
//    }

//    void Update()
//    {
//        // Think of this like breathing - smooth, natural movement
//        // Lerp is like gradually walking from point A to point B, not teleporting
//        transform.localScale = Vector3.Lerp(
//            transform.localScale,
//            targetScale,
//            Time.deltaTime * animationSpeed
//        );

//        // Fade the text in or out smoothly (like sunrise/sunset)
//        if (textCanvasGroup != null)
//        {
//            float targetAlpha = isHovering ? 1f : 0f; // 1 = fully visible, 0 = invisible
//            textCanvasGroup.alpha = Mathf.Lerp(
//                textCanvasGroup.alpha,
//                targetAlpha,
//                Time.deltaTime * textFadeSpeed
//            );
//        }
//    }

//    // When mouse enters - like the ingredient gets excited to see you!
//    void OnMouseEnter()
//    {
//        isHovering = true;
//        // The ingredient "pumps up" like an excited heartbeat
//        targetScale = originalScale * scaleMultiplier;
//    }

//    // When mouse leaves - the ingredient calms down and relaxes
//    void OnMouseExit()
//    {
//        isHovering = false;
//        // Return to normal size, like exhaling after holding your breath
//        targetScale = originalScale;
//    }
//}




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
        targetScale = originalScale;

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
}