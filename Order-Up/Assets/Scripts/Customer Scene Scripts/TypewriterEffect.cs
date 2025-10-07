using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; 

    float typeSpeed = 0.05f;

    public GameObject foodImage;
    public GameObject takeOrderBtn;

    private bool skip = false;

    private DialogueManager dialogueManager;
    private DialogueData currentDialogue;

    private void Start()
    { 
    // Hide the image and button in the beginning
    foodImage.SetActive(false);
    takeOrderBtn.SetActive(false);

    // Get the random dialogue data from the DialogueManager
    dialogueManager = FindObjectOfType<DialogueManager>();

    if(dialogueManager == null) Debug.Log("No DialogueManager Found.");
    currentDialogue = dialogueManager.GetRandomDialogue();
    
    string fullText = currentDialogue.sentenceTemplate.Replace("{dish}", $"<color={currentDialogue.color}>{currentDialogue.dishName}</color>");
    dialogueText.text = fullText;
 
    dialogueText.ForceMeshUpdate();
      
    StartCoroutine(ShowText()); 
    }

    // Show text with Typewriter effect, then show the food image and button
    IEnumerator ShowText()
    {
        TMP_TextInfo info = dialogueText.textInfo;
        int total = info.characterCount;
        int i = 0;
        StartCoroutine(DetectSkip());

        while (i <= total)
        {
            if (skip)
            {
                dialogueText.maxVisibleCharacters = total;
                break;
            }
            dialogueText.maxVisibleCharacters = i++;
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(0.5f);
 
        
        // load food image
        Sprite sprite = Resources.Load<Sprite>("FoodImages/" + currentDialogue.image);
        if (sprite != null)
        {
            foodImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            foodImage.SetActive(true);
        } else{
            Debug.Log("Sprite not found:" + currentDialogue.image);
        }

        yield return new WaitForSeconds(0.5f);
        takeOrderBtn.SetActive(true);
    }

    // If the player clicks, skip the typerwriter effect
    IEnumerator DetectSkip()
    {
        while (!skip)
        {
            if (Input.GetMouseButtonDown(0))
                skip = true;
            yield return null;
        }
    }

    // Jump to kitchen scene
    public void GoToKitchen()
    {
        GameData.currentDishId = currentDialogue.dishId;
        SceneManager.LoadScene("KitchenScene");
        Debug.Log("dishid:" + GameData.currentDishId);
    }

}
