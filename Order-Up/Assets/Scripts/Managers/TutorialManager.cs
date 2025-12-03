using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public enum TutorialType
    {
        None,
        Level1_Basic,   // Cheesy Fries (Fryer)
        Level3_Pot,     // Mac n Cheese (Pot)
        Level4_Pan      // Chicken Cheesy Melt (Pan)
    }
    [Header("Current Status")]
    public TutorialType activeTutorial = TutorialType.None;
    private int currentStep = 0;
    
    [Header("Tutorial UI Sets")]
    public GameObject[] FryerSteps; // Fryer tutorial popups
    public GameObject[] PotSteps;    // Pot tutorial popups
    public GameObject[] PanSteps;    // Pan tutorial popups
    
    private GameObject[] currentActivePopups;
    
    // private int previousStep = -1;
    
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
        // Hide everything to start
        HideAllPanels();
        
        if (GameData.CurrentLevel == 1 && GameData.CurrentRound == 0)
        {
            // Run the fryer tutorial on Level 1 Round 0
            Debug.Log("Starting fryer tutorial");
            StartTutorial(TutorialType.Level1_Basic, FryerSteps);
        }
        else if (GameData.CurrentLevel == 3 && GameData.CurrentRound == 0)
        { 
            // Run the pot tutorial on Level 3 Round 0
            Debug.Log("Starting pot tutorial");
            StartTutorial(TutorialType.Level3_Pot, PotSteps);
        }
        // else if (GameData.CurrentLevel == 4 && GameData.CurrentRound == 2)
        // {
        //     // Run the pan tutorial on Level 4 Round 2
        //     Debug.Log("Starting pan tutorial");
        //     StartTutorial(TutorialType.Level4_Pan, PanSteps);
        // }
        else
        {
            // It's not a tutorial level. Disable tutorial GameObject.
            gameObject.SetActive(false);
        }
    }
    
    void HideAllPanels()
    {
        // Hide all step popup
        if (FryerSteps != null) foreach (var p in FryerSteps) if(p) p.SetActive(false);
        if (PotSteps != null) foreach (var p in PotSteps) if(p) p.SetActive(false);
        if (PanSteps != null) foreach (var p in PanSteps) if(p) p.SetActive(false);
    }
    
    void StartTutorial(TutorialType type, GameObject[] popups)
    {
        activeTutorial = type;
        currentActivePopups = popups;
        currentStep = 0;
        
        // Disable the Order Up button at the start of every tutorial to prevent player from ending level prematurely
        if (orderUpButton != null)
        {
            orderUpButton.interactable = false;
        }
        // Start the first step
        EnterStep(0);
    }

    void Update()
    {
        // TODO
        // if (previousStep != currentStep)
        // {
        //     // Run the setup logic for the new step
        //     EnterStep(currentStep);
        //     
        //     // Update previousStep so this block doesn't run again
        //     // until currentStep changes.
        //     previousStep = currentStep;
        // }
        switch (activeTutorial)
        {
            case TutorialType.Level1_Basic:
                UpdateLevel1();
                break;
            case TutorialType.Level3_Pot:
                UpdateLevel3();
                break;
            case TutorialType.Level4_Pan:
                UpdateLevel4();
                break;
        }
        
    }

    void UpdateLevel1()
    {
        if (currentStep == 0)
        {
            // increment step when player clicks anywhere on canvas
            if (Input.GetMouseButtonDown(0))
            {
                AdvanceStep();
            }
            
        }
        else if (currentStep == 9)
        {
            // end tutorial when player clicks anywhere
            if (Input.GetMouseButtonDown(0))
            {
                EndTutorial();
            }
        }
    }

    void UpdateLevel3()
    {
        if (currentStep == 0)
        {
            // increment step when player clicks anywhere on canvas
            if (Input.GetMouseButtonDown(0))
            {
                AdvanceStep();
            }
            
        }

        if (currentStep == 5)
        {
            // end tutorial when player clicks anywhere
            if (Input.GetMouseButtonDown(0))
            {
                EndTutorial();
            }
        }
    }

    void UpdateLevel4()
    {
        // TODO
    }
    
    void AdvanceStep()
    {
        // Hide current
        if (currentActivePopups != null && currentStep < currentActivePopups.Length)
        {
            if (currentActivePopups[currentStep] != null)
                currentActivePopups[currentStep].SetActive(false);
        }

        currentStep++;
        EnterStep(currentStep);
    }
    
    void EnterStep(int step)
    {
        
        // Show the new popup
        if (currentActivePopups != null && step < currentActivePopups.Length)
        {
            if (currentActivePopups[step] != null)
                currentActivePopups[step].SetActive(true);
        }
        else
        {
            // Log a warning if the popup for this step is missing
            Debug.LogWarning($"Tutorial: No popup found for step {step}.");
            return;
        }
        
        // Run logic based on Tutorial Type (current level)
        switch (activeTutorial)
        {
            case TutorialType.Level1_Basic:
                EnterLevel1Step(step);
                break;
            case TutorialType.Level3_Pot:
                EnterLevel3Step(step);
                break;
            case TutorialType.Level4_Pan:
                EnterLevel4Step(step);
                break;
        }
        
        
    }
    
    void EnterLevel1Step(int step)
    {
        Debug.Log($"[Fryer Tutorial] Entered Step {step}");
        // Logic for fryer tutorial
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
                // Debug.Log($"Step {step}: drag second ingredient onto plate");
                break;
            case 7:
                // Debug.Log($"Step {step}: click combine btn to combine ingredients");
                break;
            case 8:
                // Debug.Log($"Step {step}: submit dish by clicking order up btn");
                // Enable Order Up button
                if (orderUpButton != null)
                {
                    orderUpButton.interactable = true;
                }
                break;
            case 9:
                // Debug.Log($"Step {step}: tell player the goal of the game");
                break;
            
            default:
                Debug.LogWarning($"Entered an unknown tutorial step: {step}");
                break;
        }
    }
    
    void EnterLevel3Step(int step)
    {
        // Logic for Pot tutorial
        Debug.Log($"[Pot Tutorial] Entered Step {step}");
        switch (step)
        {
            case 0:
                // Display text "you have unlocked new cookware: pot!"
                break;
            case 1:
                // Instruct player to turn on burner
                break;
            case 2:
                // Instruct player drag pasta to pot
                break;
            case 3:
                // Instruct player to stir pot with ladle
                isWaitingForCookingToEnd = false; // reset the flag
                break;
            case 4:
                // Instruct player to drag pasta to plate
                break;
            case 5:
                // Congratulate player on learning how to use pot
                if (orderUpButton != null)
                {
                    orderUpButton.interactable = true;
                }
                break;
            default:
                Debug.LogWarning($"Entered an unknown tutorial step: {step}");
                break;
        }
            
    }
    
    void EnterLevel4Step(int step)
    {
        // Logic for Pan tutorial
        Debug.Log($"[Pan Tutorial] Entered Step {step}");
    }
        
    public void OnIngredientDroppedOnCookware(DraggableIngredient ingredient, BaseCookware cookware)
    {
        if (activeTutorial == TutorialType.Level1_Basic)
        {
            // Check for Potato + Fryer
            // if (cookware.GetCookwareType() == CookwareType.Fryer) { AdvanceStep(); }
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
                    isCorrectIngredient = ing.ingredientData.ingredientName.Contains("Potato");
                }
        
                if (isCorrectCookware && isCorrectIngredient)
                {
                    AdvanceStep();; // Advance to step 2
                }
            }
            else if (currentStep == 4)
            {
                // Check if it's the correct cookware
                bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Oven;

                // Check if it's the correct ingredient (cheese)
                Ingredient ing = ingredient.GetComponent<Ingredient>();
                bool isCorrectIngredient = false;
                if (ing != null)
                {
                    isCorrectIngredient = ing.ingredientData.ingredientName.Contains("Cheese");
                }
        
                if (isCorrectCookware && isCorrectIngredient)
                {
                    AdvanceStep(); // Advance to step 5
                }
            }
        }
        if (activeTutorial == TutorialType.Level3_Pot)
        {
            // Check if we're on step 2
            if (currentStep == 2)
            {
                // Check if it's the correct cookware
                bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Pot;
                
                // Check if it's the correct ingredient (pasta)
                Ingredient ing = ingredient.GetComponent<Ingredient>();
                bool isCorrectIngredient = false;
                if (ing != null)
                {
                    isCorrectIngredient = ing.ingredientData.ingredientName.Contains("Pasta");
                }
        
                if (isCorrectCookware && isCorrectIngredient)
                {
                    AdvanceStep(); // Advance to step 3
                }
            }
        }
        
        
    }

    public void OnCookwareStarted(BaseCookware cookware)
    {
        Debug.Log($"[Tutorial] Cookware Started: {cookware.GetCookwareType()}");
        if (activeTutorial == TutorialType.Level1_Basic)
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
                if (cookware.GetCookwareType() == CookwareType.Oven)
                {
                    Debug.Log("OnCookwareStarted: Oven started cooking!");
                    isWaitingForCookingToEnd = true;
                }
            }
        }
        
    }

    public void OnStirring(StirBasedCookware cookware)
    {
        if (activeTutorial == TutorialType.Level3_Pot)
        {
            // Check if we're currently on step 3
            if (currentStep == 3)
            {
                // Check if it's the correct cookware (the pot)
                if (cookware.GetCookwareType() == CookwareType.Pot)
                {
                    Debug.Log("OnCookwareStarted: Pot started cooking!");
                    isWaitingForCookingToEnd = true;
                }
            }
        }
    }
    
    public void OnCookingFinished(BaseCookware cookware, Ingredient ingredient)
    {
        Debug.Log($"[Tutorial] OnCookingFinished cookware:{cookware.GetCookwareType()}");
        if (activeTutorial == TutorialType.Level1_Basic)
        {
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
                    isCorrectIngredient = ingredient.ingredientData.ingredientName.Contains("Potato");
                }

                if (isCorrectCookware && isCorrectIngredient)
                {
                    Debug.Log("OnCookingFinished: Cooking finished!");
                    AdvanceStep();
                    ; // Advance to Step 3
                    isWaitingForCookingToEnd = false;
                }
                else
                {
                    // Another item finished, but not the one we're waiting for.
                    Debug.Log(
                        $"OnCookingFinished: Ignored {ingredient.ingredientData.ingredientName} finishing in {cookware.name}. Waiting for Potato_Raw.");
                }
            }
            else if (currentStep == 5)
            {
                bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Oven;

                bool isCorrectIngredient = false;
                if (ingredient != null)
                {
                    // Check for the Cooked ingredient name
                    isCorrectIngredient = ingredient.ingredientData.ingredientName.Contains("Cheese");
                }

                if (isCorrectCookware && isCorrectIngredient)
                {
                    Debug.Log("OnCookingFinished: Cooking finished!");
                    AdvanceStep();
                    ; // Advance to Step 6
                    isWaitingForCookingToEnd = false;
                }
                else
                {
                    // Another item finished, but not the one we're waiting for.
                    Debug.Log(
                        $"OnCookingFinished: Ignored {ingredient.ingredientData.ingredientName} finishing in {cookware.name}. Waiting for Cheese_Raw.");
                }
            }
        }

        if (activeTutorial == TutorialType.Level3_Pot)
        {
            // Check if we were waiting for cooking to end
            if (!isWaitingForCookingToEnd)
            {
                Debug.Log($"leaving OnCookingFinished, isWaitingForCookingToEnd: {isWaitingForCookingToEnd}");
                return;
            }

            if (currentStep == 3)
            {
                bool isCorrectCookware = cookware.GetCookwareType() == CookwareType.Pot;

                bool isCorrectIngredient = false;
                if (ingredient != null)
                {
                    // Check for the Cooked ingredient name (pasta)
                    isCorrectIngredient = ingredient.ingredientData.ingredientName.Contains("Pasta");
                }

                if (isCorrectCookware && isCorrectIngredient)
                {
                    Debug.Log("OnCookingFinished: Cooking finished!");
                    AdvanceStep(); // Advance to Step 4
                    isWaitingForCookingToEnd = false;
                }
                else
                {
                    // Another item finished, but not the one we're waiting for.
                    Debug.Log(
                        $"OnCookingFinished: Ignored {ingredient.ingredientData.ingredientName} finishing in {cookware.name}. Waiting for Pasta.");
                }
            }
        }
    }

    public void onPotFireButtonClicked()
    {
        if (activeTutorial == TutorialType.Level3_Pot)
        {
            // Check if we're currently on step 1
            if (currentStep == 1)
            {
                AdvanceStep(); // Advance to step 2
            }
        }
    }
    
    public void OnIngredientDroppedOnPlate(DraggableIngredient draggable, Plate plate)
    {
        if (activeTutorial == TutorialType.Level1_Basic)
        {
            // Check if we are on Step 3 or Step 6
            if (currentStep == 3)
            {
                // Check if it's the correct ingredient (Cooked Potato)
                Ingredient ing = draggable.GetComponent<Ingredient>();
                if (ing != null)
                {
                    bool isCorrectIngredient = ing.ingredientData.ingredientName.Contains("Potato");
                    bool isCooked = ing.currentState == IngredientState.Cooked;

                    if (isCorrectIngredient && isCooked)
                    {
                        Debug.Log("OnIngredientDroppedOnPlate: Cooked ingredient plated!");
                        AdvanceStep(); // Advance to Step 4
                    }
                }
            }
            else if (currentStep == 6)
            {
                // Check if it's the correct ingredient (Cooked Cheese)
                Ingredient ing = draggable.GetComponent<Ingredient>();
                if (ing != null)
                {
                    bool isCorrectIngredient = ing.ingredientData.ingredientName.Contains("Cheese");
                    bool isCooked = ing.currentState == IngredientState.Cooked;

                    if (isCorrectIngredient && isCooked)
                    {
                        Debug.Log("OnIngredientDroppedOnPlate: Cooked ingredient plated!");
                        AdvanceStep(); // Advance to Step 7
                    }
                }
            }
        }
        if (activeTutorial == TutorialType.Level3_Pot)
        {
            // Check if we are on Step 4
            if (currentStep == 4)
            {
                // Check if it's the correct ingredient (Pasta)
                Ingredient ing = draggable.GetComponent<Ingredient>();
                if (ing != null)
                {
                    bool isCorrectIngredient = ing.ingredientData.ingredientName.Contains("Pasta");
                    bool isCooked = ing.currentState == IngredientState.Cooked;

                    if (isCorrectIngredient && isCooked)
                    {
                        Debug.Log("OnIngredientDroppedOnPlate: Cooked ingredient plated!");
                        AdvanceStep(); // Advance to Step 5
                    }
                }
            }
        }
    }

    public void OnCombineClicked()
    {
        if (activeTutorial == TutorialType.Level1_Basic)
        {
            // Check if we are on Step 7
            if (currentStep != 7)
            {
                return;
            }
            AdvanceStep(); // Advance to Step 8
        }
    }
    public void OnOrderUpClicked()
    {
        if (activeTutorial == TutorialType.Level1_Basic)
        {
            // Check if we are on Step 8
            if (currentStep != 8)
            {
                return;
            }

            Debug.Log("OnOrderUpClicked: Dish submitted!");
            AdvanceStep(); // Advance to the step 9
        }
    }
    
    void EndTutorial()
    {
        Debug.Log("Tutorial Finished!");
        // if (currentStep >= 0 && currentStep < Steps.Length && Steps[currentStep] != null)
        // {
        //     Steps[currentStep].SetActive(false);
        // }
        if (currentActivePopups != null && currentStep < currentActivePopups.Length)
        {
            if(currentActivePopups[currentStep]) 
                currentActivePopups[currentStep].SetActive(false);
        }

        // this.enabled = false;
        gameObject.SetActive(false);
    }
    

}
