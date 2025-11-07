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
    public Button airFryerStartCookingButton; // The "Start" button in the popup 

    [Header("Step 5: Drag to Dish")]
    public DraggableIngredient cookedPotato; // The runtime cooked item (store the Draggable instance)
    //public IDropZone dishZone; // Your existing script

    [Header("Step 6: Submit Order")]
    public Button submitOrderButton;
    public Plate plate;
    public Button trashButton;

    private int currentStep = 0;

    private bool potatoCooked = false;
    private bool friesDone = false;

    // store the runtime delegate so we can unsubscribe cleanly
    private System.Action<DraggableIngredient, Plate> cookedDropHandler;

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
                potatoIngredient.IngredientDroppedOnCookware += HandlePotatoDroppedOnFryer;
                break;

            case 2:
                instructionText.text = "Nice toss! Now start cooking and let the magic happen.";
                //     HighlightUI(airFryer.GetComponent<RectTransform>());
                //     // Assumes your DropZone has an event. See guide below.
                // airFryer.OnItemDropped += OnStepCompleted; 
                airFryerStartCookingButton.onClick.AddListener(HandleStartCooking);
                break;

            case 3:
                instructionText.text = "Don't walk away too far. Burnt fries are no fun!";
                //     HighlightUI(airFryerButton.GetComponent<RectTransform>());
                airFryer.OnCooked += HandlePotatoCooked;
                break;

            case 4:
                instructionText.text = "Now drag the cooked potato onto the plate. Remember, always make the combination on the plate.";
                if (cookedPotato != null)
                {
                    cookedDropHandler = HandlePotatoDroppedOnPlate;
                    cookedPotato.OnDroppedOnPlate += cookedDropHandler;
                    Debug.Log($"[Tutorial] The cookedPotato instance is dropped on the plate");
                }
                break;

            case 5:
                instructionText.text = "If you feel like a reset, click Trash to clean that plate(Left Click to continue).";

                //     HighlightUI(dishZone.GetComponent<RectTransform>());
                //     // We need to wait for the *cooked* potato to appear
                //     // This is a special case. Let's assume the cooking popup
                //     // enables the cookedPotato item. We'll listen for its drag.
                // cookedPotato.OnPantryIngredientDragged.+= OnStepCompleted;
                break;

            case 6:
                instructionText.text = "BOOM! You've mastered your first dish! Hit Order Up and serve your hungry customers!";
                //     HighlightUI(submitOrderButton.GetComponent<RectTransform>());
                submitOrderButton.onClick.AddListener(OnTutorialFinished);
                break;

            default:
                Debug.Log($"Error: step {currentStep} does not exist");
                break;
        }
    }

    void Update()
    {
        // Advance from step 5 to step 6 on left mouse click
        if (currentStep == 5 && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[Tutorial] Step 5: left click detected, advancing to step 6");
            GoToStep(6);
        }
    }


    private void HandlePotatoDroppedOnPlate(DraggableIngredient ingredient)
    {
        if (currentStep == 4 && ingredient.name.Contains("Potato"))
        {
            Debug.Log("[Tutorial] Potato dropped into plate!");
            GoToStep(5);
        }
    }

    // Overload matching the OnDroppedOnPlate signature so we can subscribe directly.
    private void HandlePotatoDroppedOnPlate(DraggableIngredient ingredient, Plate plate)
    {
        // Delegate to the single-argument handler to keep behavior centralised.
        HandlePotatoDroppedOnPlate(ingredient);
    }

    private void HandlePotatoCooked(DraggableIngredient ingredient)
    {
        if (currentStep == 3 && ingredient.name.Contains("Potato") && ingredient.GetComponent<Ingredient>().currentState == IngredientState.Cooked)
        {
            Debug.Log("[Tutorial] Potato cooked!");
            // Store the actual runtime Draggable instance so we can listen to its drop event
            cookedPotato = ingredient;
            Debug.Log($"[Tutorial] cached cookedPotato = {cookedPotato.name}");
            GoToStep(4);
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

    //case 1
    private void HandlePotatoDroppedOnFryer(DraggableIngredient ingredient, Cookwares cookware)
    {
        if (currentStep == 1 && ingredient.name.Contains("Potato") && cookware.cookwareType == CookwareType.Fryer)
        {
            Debug.Log("[Tutorial] Potato dropped into fryer!");
            GoToStep(2);
        }
    }

    //case 2
    public void HandleStartCooking()
    {
        if (currentStep == 2)
        {
            Debug.Log("[Tutorial] Started cooking!");
            GoToStep(3);
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
            potatoIngredient.IngredientDroppedOnCookware -= HandlePotatoDroppedOnFryer;


        if (airFryer != null)
        {
            airFryer.OnItemDropped -= OnStepCompleted;
            airFryer.OnCookwareClicked -= OnStepCompleted;
            airFryer.OnItemDropped -= OnStepCompleted;
            airFryer.OnCooked -= HandlePotatoCooked;
        }
        // if (cookingPopup != null)
        // cookingPopup.OnCookingStopped -= OnStepCompleted;
        // if (cookedPotato != null)
        //     cookedPotato.OnItemBeginDrag -= HandleItemBeginDrag;
        if (airFryerStartCookingButton != null)
            airFryerStartCookingButton.onClick.RemoveListener(HandleStartCooking);
        if (cookedPotato != null && cookedDropHandler != null)
        {
            Debug.Log($"[Tutorial] Unsubscribing cookedPotato drop handler from instance: {cookedPotato.name}");
            cookedPotato.OnDroppedOnPlate -= cookedDropHandler;
            cookedDropHandler = null;
        }
        // if (submitOrderButton != null)
        //     submitOrderButton.onClick.RemoveListener(OnTutorialFinished);
    }
}
