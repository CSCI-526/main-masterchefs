using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    // [Tooltip("The parent CanvasGroup for all tutorial UI (text, highlighters)")]
    public CanvasGroup tutorialCanvasGroup;
    // [Tooltip("The text field to display tutorial instructions")]
    public TextMeshProUGUI instructionText;
    // [Tooltip("A UI Image used to 'spotlight' or 'highlight' the target button")]
    public RectTransform highlighter; // An image with a hole, or an outline

    [Header("Step 1: Select Potato")]
    public PantryIngredient potatoIngredient; // Your existing script

    [Header("Step 2: Drag to Fryer")]
    public Cookwares airFryer; // Your existing script


    [Header("Step 4: Cook Item")]
    // public CookingPopup cookingPopup; // Your existing script
    public Button startCookButton; // The "Start" button in the popup 

    [Header("Step 5: Drag to Dish")]
    public PantryIngredient cookedPotato; // The item that comes from the fryer
    //public IDropZone dishZone; // Your existing script

    [Header("Step 6: Submit Order")]
    public Button submitOrderButton;
    public Plate plate;
    public Button trashButton;

    private int currentStep = 0;

    private bool potatoCooked = false;
    private bool friesDone = false;

    void Start()
    {
        // Check the session manager
        if (GameManager.Instance.CurrentLevel != 1)
        {
            gameObject.SetActive(false);
            return;
        }

        // Show the tutorial UI
        tutorialCanvasGroup.alpha = 1;
        tutorialCanvasGroup.interactable = false; // Makes it so the UI doesn't block clicks
        tutorialCanvasGroup.blocksRaycasts = false;

        GoToStep(1);
    }

    private void GoToStep(int step)
    {
        currentStep = step;
        Debug.Log($"current step: {currentStep}");

        RemoveAllListeners();

        switch (currentStep)
        {
            case 1:
                instructionText.text = "Grab a potato and toss it into the air fryer.";
                // Highlight the pantry ingredient
                // HighlightUI(potatoIngredient.GetComponent<RectTransform>());
                // Subscribe to the new event on the PantryIngredient script
                // potatoIngredient.OnPantryIngredientDragged += HandleItemBeginDrag;
                potatoIngredient.OnIngredientDroppedToCookware += HandlePotatoDroppedToFryer;
                break;

            case 2:
                instructionText.text = "Start Cooking and be careful with the time.";
                //     HighlightUI(airFryer.GetComponent<RectTransform>());
                //     // Assumes your DropZone has an event. See guide below.
               // airFryer.OnItemDropped += OnStepCompleted;
                break;

            case 3:
                instructionText.text = "Step 3: Select the air fryer to start cooking.";
            //     HighlightUI(airFryerButton.GetComponent<RectTransform>());
                airFryer.OnCookwareClicked += OnStepCompleted;
                break;

            case 4:
                instructionText.text = "Step 4: Start cooking and stop it after 3 seconds.";
            //     // Highlight the whole popup or just the start button
            //     HighlightUI(startCookButton.GetComponent<RectTransform>());
            //     // We listen for the cooking to FINISH, not start
            //     // Assumes your CookingPopup has an event. See guide below.
                // cookingPopup.OnCookingStopped.AddListener(OnStepCompleted);
                break;

            case 5:
                instructionText.text = "Step 5: Drag the cooked fries to the dish.";
            //     HighlightUI(dishZone.GetComponent<RectTransform>());
            //     // We need to wait for the *cooked* potato to appear
            //     // This is a special case. Let's assume the cooking popup
            //     // enables the cookedPotato item. We'll listen for its drag.
                // cookedPotato.OnPantryIngredientDragged.+= OnStepCompleted;
                break;
                
            case 6:
                instructionText.text = "Step 6: Click the submit button to complete the level!";
            //     HighlightUI(submitOrderButton.GetComponent<RectTransform>());
                submitOrderButton.onClick.AddListener(OnTutorialFinished);
                break;

            default:
                Debug.Log($"Error: step {currentStep} does not exist");
                break;
        }
    }

    // private void HandleItemBeginDrag(DraggableIngredient item)
    // {
    //     Debug.Log($"ingredient name:{item.name}");
    //     if (currentStep == 1 && item.name == "Potato_Raw")
    //     {
    //         OnStepCompleted(); // Go to next step
    //     }
    //     else if (currentStep == 5 && item == cookedPotato)
    //     {
    //         OnStepCompleted(); // Go to next step
    //     }
    // }

    private void HandlePotatoDroppedToFryer(DraggableIngredient ingredient, Cookwares cookware)
    {
        if (currentStep == 1 && ingredient.name.Contains("Potato"))
        {
            Debug.Log("[Tutorial] Potato dropped into fryer!");
            GoToStep(2);
        }
    }
    private void OnStepCompleted()
    {
        // Un-register all listeners to prevent double-firing
        RemoveAllListeners();

        // Go to the next step
        GoToStep(currentStep + 1);
    }

    /// Special version for DropZones, as its event might pass data.
    /// We just route it to the main OnStepCompleted function.
    private void OnStepCompleted(DraggableIngredient ingredient)
    {
        Debug.Log($"ingredient name: {ingredient.name}");
        if (currentStep == 2 && ingredient.name == "Potato_Raw")
        {
            OnStepCompleted();
        }
        else if (currentStep == 5 && ingredient == cookedPotato)
        {
            OnStepCompleted();
        }
    }


    private void OnTutorialFinished()
    {
        RemoveAllListeners();
        // Hide the tutorial UI
        tutorialCanvasGroup.alpha = 0;
        Debug.Log("Tutorial Complete!");
    }

    /// Moves the highlighter UI element to focus on the target UI.
    private void HighlightUI(RectTransform target)
    {
        highlighter.position = target.position;
        Debug.Log($"target position: {target.position}"); //testing
        Debug.Log($"highlighter position: {highlighter.position}"); //testing 
        // You might need to adjust size as well:
        // highlighter.sizeDelta = target.sizeDelta + new Vector2(20, 20); // Add padding
    }

    /// Helper function to clean up listeners so we don't call a step twice.
    private void RemoveAllListeners()
    {
        // Use -= to unsubscribe from System.Actions
        // if (potatoIngredient != null)
        //     potatoIngredient.OnPantryIngredientDragged -= HandleItemBeginDrag;
        if (potatoIngredient != null)
            potatoIngredient.OnIngredientDroppedToCookware -= HandlePotatoDroppedToFryer;


        // if (airFryer != null)
            airFryer.OnItemDropped -= OnStepCompleted;
        // if (cookingPopup != null)
        // cookingPopup.OnCookingStopped -= OnStepCompleted;
        // if (cookedPotato != null)
        //     cookedPotato.OnItemBeginDrag -= HandleItemBeginDrag;
        if (airFryer != null)
        {
            airFryer.OnCookwareClicked -= OnStepCompleted;
            airFryer.OnItemDropped -= OnStepCompleted;
        }
        // if (submitOrderButton != null)
        //     submitOrderButton.onClick.RemoveListener(OnTutorialFinished);
    }
}
