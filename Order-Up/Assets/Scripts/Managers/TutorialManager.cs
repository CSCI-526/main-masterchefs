using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public GameObject[] Steps;
    private int currentStep = 0;
    private int previousStep = -1;
    
    private bool isWaitingForCookingToEnd = false;
    
    public Button orderUpButton;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        // Debug.Log($"Current Level: {GameData.CurrentLevel}, Current Round: {GameData.CurrentRound}");
        if (GameData.CurrentLevel == 1 && GameData.CurrentRound == 0)
        {
            // Run the tutorial on Level 1 Round 0
            InitializeTutorial();
        }
        else
        {
            // It's not Level 1 Round 0. Disable tutorial GameObject.
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (previousStep != currentStep)
        {
            // Run the setup logic for the new step
            EnterStep(currentStep);
            
            // Update previousStep so this block doesn't run again
            // until currentStep changes.
            previousStep = currentStep;
        }
        
        if (currentStep == 0)
        {
            // Debug.Log($"Step {currentStep + 1}: show player dish window (and level, currency, hint when they're implemented), tell player click anywhere to continue");
            // increment step when player clicks anywhere on canvas
            if (Input.GetMouseButtonDown(0))
            {
                currentStep++;
            }
            
        }
        // else if (currentStep == 1)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: show player first ingredient(potato) and tell them to drag and drop on cookware");
        //     // increment step when player drags and drops potato on fryer (see OnIngredientDroppedOnCookware)
        //     
        // }
        // else if (currentStep == 2)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: show player how to use the cookware(press start button and wait for progress bar to finish");
        //     // increment step when player presses start button on cookware and progress bar finishes (see OnCookwareStarted and OnCookingFinished)
        //     
        // }
        // else if (currentStep == 3)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: plate the ingredient");
        //     // increment step when plate receives the Potato_Cooked ingredient (see OnIngredientDroppedOnPlate)
        // }

        // else if (currentStep == 4)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: show player second ingredient and drag and drop onto cookware");
        //     // increment step when player drags and drops cheese on fryer (see OnIngredientDroppedOnCookware)
        // }
        // else if (currentStep == 5)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: show player how to use cookware");
        //     // increment step when player presses start button on cookware and progress bar finishes (see OnCookwareStarted and OnCookingFinished)
        // }
        // else if (currentStep == 6)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: drag second ingredient onto plate to combine");
        //     // increment step when plate receives the Cheese_Cooked ingredient (see OnIngredientDroppedOnPlate)
        // }
        // else if (currentStep == 7)
        // {
        //     // Debug.Log($"Step {currentStep + 1}: submit dish by clicking order up btn");
        //     // increment step when Order Up button is clicked  (see OnOrderUpClicked)
        // }
        else if (currentStep == 8)
        {
            // Debug.Log($"Step {currentStep + 1}: tell player the goal of the game and click anywhere to end tutorial");
            // end tutorial when player clicks anywhere
            if (Input.GetMouseButtonDown(0))
            {
                EndTutorial();
            }
        }
        
    }
    
    void InitializeTutorial()
    {
        // Ensure all tutorial steps are hidden when we begin
        if (Steps != null)
        {
            foreach (GameObject stepPopup in Steps)
            {
                if (stepPopup != null)
                {
                    stepPopup.SetActive(false);
                }
            }
        }

        // Disable the Order Up button at the start
        if (orderUpButton != null)
        {
            orderUpButton.interactable = false;
        }
    }
    void EnterStep(int step)
    {
        // Hide the last step's popup
        if (previousStep >= 0 && previousStep < Steps.Length && Steps[previousStep] != null)
        {
            Steps[previousStep].SetActive(false);
        }
        
        // Show the new step's popup
        if (step >= 0 && step < Steps.Length && Steps[step] != null)
        {
            Steps[step].SetActive(true);
        }
        else
        {
            // Log a warning if the popup for this step is missing
            Debug.LogWarning($"Tutorial: No popup found for step {step}.");
            return;
        }
        switch (step)
        {
            case 0:
                // Debug.Log($"Step {step}: show player dish window...");
                break;
            case 1:
                // Debug.Log($"Step {step}: show player first ingredient and tell them to drag and drop onto cookware");
                break;
            case 2:
                // Debug.Log($"Step {step}: show player how to use the cookware (press start button on fryer and wait for progress bar to finish)");
                isWaitingForCookingToEnd = false; // reset the flag
                break;
            case 3:
                // Debug.Log($"Step {step}: plate the ingredient");
                break;
            case 4:
                // Debug.Log($"Step {step}: show player second ingredient...");
                break;
            case 5:
                // Debug.Log($"Step {step}: show player how to use cookware");
                isWaitingForCookingToEnd = false; // reset the flag
                break;
            case 6:
                // Debug.Log($"Step {step}: drag second ingredient onto plate to combine with first");
                break;
            case 7:
                // Debug.Log($"Step {step}: submit dish by clicking order up btn");
                // Enable Order Up button
                if (orderUpButton != null)
                {
                    orderUpButton.interactable = true;
                }
                break;
            case 8:
                // Debug.Log($"Step {step}: tell player the goal of the game");
                break;
            
            default:
                Debug.LogWarning($"Entered an unknown tutorial step: {step}");
                break;
        }
    }
        
    public void OnIngredientDroppedOnCookware(DraggableIngredient ingredient, BaseCookware cookware)
    {
        // Debug.Log($"OnIngredientDroppedOnCookware called with cookware: {cookware.GetCookwareType()} and  ingredient: {ingredient}"); 
        // Check if we're currently on step 1 on step 5
        if (currentStep == 1)
        { 
            // Check if it's the correct cookware
            bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Fryer;

            // Check if it's the correct ingredient (potato)
            Ingredient ing = ingredient.GetComponent<Ingredient>();
            bool isCorrectIngredient = false;
            if (ing != null)
            {
                isCorrectIngredient = ing.ingredientData.ingredientName == "Potato_Raw";
            }
        
            if (isCorrectCookware && isCorrectIngredient)
            {
                currentStep++; // Advance to step 2
            }
        }
        else if (currentStep == 4)
        {
            // Check if it's the correct cookware
            bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Fryer;

            // Check if it's the correct ingredient (cheese)
            Ingredient ing = ingredient.GetComponent<Ingredient>();
            bool isCorrectIngredient = false;
            if (ing != null)
            {
                isCorrectIngredient = ing.ingredientData.ingredientName == "Cheese_Raw";
            }
        
            if (isCorrectCookware && isCorrectIngredient)
            {
                currentStep++; // Advance to step 5
            }
        }
        else
        {
            return;
        }

        
    }
    
    public void OnCookwareStarted(BaseCookware cookware)
    {
        // Check if we're currently on step 2 or 6
        if (currentStep == 2)
        {
            // Check if it's the correct cookware (the fryer)
            if (cookware.GetCookwareType() == CookwareType.Fryer)
            {
                Debug.Log("OnCookwareStarted: Fryer started cooking!");
                isWaitingForCookingToEnd = true;
            }
        }
        else if (currentStep == 5)
        {
            // Check if it's the correct cookware (the fryer)
            if (cookware.GetCookwareType() == CookwareType.Fryer)
            {
                Debug.Log("OnCookwareStarted: Fryer started cooking!");
                isWaitingForCookingToEnd = true;
            }
        }
        else
        {
            return;
        }

        
    }
    
    public void OnCookingFinished(BaseCookware cookware, Ingredient ingredient)
    {
        // Debug.Log($"OnCookingFinished called with cookware: {cookware.GetCookwareType()} and ingredient: {ingredient.ingredientData.ingredientName}");
        
        // Check if we were waiting for cooking to end
        if (!isWaitingForCookingToEnd)
        {
            Debug.Log($"leaving OnCookingFinished, isWaitingForCookingToEnd: {isWaitingForCookingToEnd}");
            return;
        }
        
        // Check if we are on Step 2 or Step 6 
        if (currentStep == 2)
        {
            bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Fryer;
        
            bool isCorrectIngredient = false;
            if (ingredient != null)
            {
                // Check for the Cooked ingredient name
                isCorrectIngredient = ingredient.ingredientData.ingredientName == "Potato_Raw";
            }
        
            if (isCorrectCookware && isCorrectIngredient)
            {
                Debug.Log("OnCookingFinished: Cooking finished!");
                currentStep++; // Advance to Step 3
                isWaitingForCookingToEnd = false;
            }
            else
            {
                // Another item finished, but not the one we're waiting for.
                Debug.Log($"OnCookingFinished: Ignored {ingredient.ingredientData.ingredientName} finishing in {cookware.name}. Waiting for Potato_Raw.");
            }
        }
        else if (currentStep == 5)
        {
            bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Fryer;
        
            bool isCorrectIngredient = false;
            if (ingredient != null)
            {
                // Check for the Cooked ingredient name
                isCorrectIngredient = ingredient.ingredientData.ingredientName == "Cheese_Raw";
            }
        
            if (isCorrectCookware && isCorrectIngredient)
            {
                Debug.Log("OnCookingFinished: Cooking finished!");
                currentStep++; // Advance to Step 6
                isWaitingForCookingToEnd = false;
            }
            else
            {
                // Another item finished, but not the one we're waiting for.
                Debug.Log($"OnCookingFinished: Ignored {ingredient.ingredientData.ingredientName} finishing in {cookware.name}. Waiting for Cheese_Raw.");
            }
        }
        else
        {
            return;
        }
        
        
    }
    
    public void OnIngredientDroppedOnPlate(DraggableIngredient draggable, Plate plate)
    {
        // Check if we are on Step 3 or Step 6
        if (currentStep == 3)
        {
            // Check if it's the correct ingredient (Cooked Potato)
            Ingredient ing = draggable.GetComponent<Ingredient>();
            if (ing != null)
            {
                bool isCorrectIngredient = ing.ingredientData.ingredientName == "Potato_Cooked";
                bool isCooked = ing.currentState == IngredientState.Cooked;

                if (isCorrectIngredient && isCooked)
                {
                    Debug.Log("OnIngredientDroppedOnPlate: Cooked ingredient plated!");
                    currentStep++; // Advance to Step 4
                }
            }
        }
        else if (currentStep == 6)
        {
            // Check if it's the correct ingredient (Cooked Cheese)
            Ingredient ing = draggable.GetComponent<Ingredient>();
            if (ing != null)
            {
                bool isCorrectIngredient = ing.ingredientData.ingredientName == "Cheese_Cooked";
                bool isCooked = ing.currentState == IngredientState.Cooked;

                if (isCorrectIngredient && isCooked)
                {
                    Debug.Log("OnIngredientDroppedOnPlate: Cooked ingredient plated!");
                    currentStep++; // Advance to Step 7
                }
            }
        }
        else
        {
            return;
        }

        
    }
    
    public void OnOrderUpClicked()
    {
        // Check if we are on Step 7
        if (currentStep != 7)
        {
            return;
        }

        Debug.Log("OnOrderUpClicked: Dish submitted!");
        currentStep++; // Advance to the step 8
    }
    
    void EndTutorial()
    {
        Debug.Log("Tutorial Finished!");
        if (currentStep >= 0 && currentStep < Steps.Length && Steps[currentStep] != null)
        {
            Steps[currentStep].SetActive(false);
        }

        // this.enabled = false;
        gameObject.SetActive(false);
    }
    

}
