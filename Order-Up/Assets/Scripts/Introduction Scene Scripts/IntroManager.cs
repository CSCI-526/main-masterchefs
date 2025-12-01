using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{

    [Header("Existing")]
    public RectTransform background;
    public Image introPanel;       
    public CanvasGroup buttonGroup;
    public TextMeshProUGUI introText;
    public Button button; 
    public RectTransform startBtnArtwork;

    [Header("Story Intro")]
    public GameObject storyLayer;
    public CanvasGroup storyCanvasGroup;
    public Image backgroundStoryImage;
    public TextMeshProUGUI storyText;
    public Sprite storyGrama;
    public Sprite storyLoanShark;
    public Sprite storyCafe;

    float charSpeed = 0.03f;
    bool skipTyping = false;
    bool isTyping = false;



    private void Awake()
    {
        button.onClick.AddListener(PlayIntroSequence);
    }

IEnumerator ShowLine(TextMeshProUGUI label, string line)
{
    skipTyping = false;
    isTyping = true;

    yield return null; //wait a frame 
    label.text = "";
    int index = 0;

    while (index < line.Length)
    {
        // if player clicks, show full line immediately
        if (Input.GetMouseButtonDown(0))
        {
            label.text = line;
            index = line.Length;
            break;
        }

        label.text += line[index];
        index++;

        float t = 0f;
        while (t < charSpeed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                label.text = line;
                index = line.Length;
                break;
            }

            t += Time.deltaTime;
            yield return null;
        }
    }
 
    label.text = line;

    // Wait for player to click to proceed
    yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
    yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

    isTyping = false;
}
 
    IEnumerator WaitForPlayerInput()
    {  
        skipTyping = false;
        while (!Input.GetMouseButtonDown(0))
            yield return null;
        if(isTyping)
        {
            skipTyping = true;
            yield break;
        }
    }

    
    public void PlayIntroSequence()
    { 

        button.interactable = false;  // Disable button to prevent multiple clicks


        StartCoroutine(StorySequence());

    }

    IEnumerator StorySequence()
    { 
        // Hide button group
        buttonGroup.alpha = 0f;  
        buttonGroup.interactable = false;
        buttonGroup.blocksRaycasts = false;
        // Show story layer
        storyLayer.SetActive(true);
        storyCanvasGroup.alpha = 0f; 
 


        // Hide story image instantly to avoid flash
        backgroundStoryImage.color = new Color(1, 1, 1, 0);

        // Fade-in story canvas
        float time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            storyCanvasGroup.alpha = Mathf.Lerp(0, 1, time / 0.5f);
            yield return null;
        }
        // Make story image visible
        backgroundStoryImage.color = Color.white;


        // Slide 1: Show Grama
        backgroundStoryImage.sprite = storyGrama;
        string[] slide1 = new string[]
        {
            "One day, Little Bear received a letter from Grandma, that's her last gift.",
            "Grandma left her the tiny café she had poured her whole life into.",
            "It used to be the coziest spot in town, but it had closed when Grandma’s health began to fail."
        };

        foreach (string line in slide1)
        {
            yield return StartCoroutine(ShowLine(storyText, line));
        } 

        // Show Loan Shark
        backgroundStoryImage.sprite = storyLoanShark;
        string[] slide2 = {
            "Reopening the café wasn’t easy.",
            "The place needed far more money than Little Bear had... which was none.",
            "Desperate, Little Bear went to the one creature everyone in town avoided, the loan shark.",
            "Loan Shark:\nPay on time, kid. I'd hate to nibble those fluffy ears."
        };
        foreach (string line in slide2)
        {
            yield return StartCoroutine(ShowLine(storyText, line));

        }

        // Show Café
        backgroundStoryImage.sprite = storyCafe;
        string[] slide3 = {
            "With the loan in paw and Grandma’s dream in her heart...",
            "Little Bear rebuilt the café from scratch.",
            "Now it's your turn to help her serve her very first customer!"
        };
        foreach (string line in slide3)
        {
            yield return StartCoroutine(ShowLine(storyText, line));

        }

        // Fade-out story canvas
        time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            storyCanvasGroup.alpha = Mathf.Lerp(1, 0, time / 0.5f);
            yield return null;
        }

        // Hide story layer
        storyLayer.SetActive(false);


        // Start intro routine
        StartCoroutine(IntroRoutine());
    }



    IEnumerator IntroRoutine()
    { 

        // Background zoom-in 
        Vector3 startScale = background.localScale;
        Vector3 endScale = startScale * 1.08f;
        float time = 0f;
 

        // // Button zoom-in
        // Vector3 startScaleBtn = startBtnArtwork.localScale;
        // Vector3 endScaleBtn = startScaleBtn * 1.08f;

        // // Button move downward to avoid overlap with background
        // Vector3 startPosBtn = startBtnArtwork.localPosition;
        // Vector3 endPosBtn = startPosBtn + new Vector3(0, -18f, 0);  


        while (time < 0.6f)
        {
            time += Time.deltaTime;
            float t = time / 0.6f;

            // // zoom in
            // background.localScale = Vector3.Lerp(startScale, endScale, t);
            // startBtnArtwork.localScale = Vector3.Lerp(startScaleBtn, endScaleBtn, t);

            // // move downward
            // startBtnArtwork.localPosition = Vector3.Lerp(startPosBtn, endPosBtn, t);



            yield return null;
        }

        // Fade-in panel
        Color p = introPanel.color;

        time = 0f;
        while (time < 0.4f)   
        {
            time += Time.deltaTime;
            p.a = Mathf.Lerp(0, 1, time / 0.4f);
            introPanel.color = p;
            yield return null;
}


        // Fade-in text 
        // introText.text = "Welcome to Master Chef Café.\nA mysterious order is coming..."; 
        // Color c = introText.color;

        // time = 0f;
        // while (time < 0.8f)
        // {
        //     time += Time.deltaTime;
        //     c.a = Mathf.Lerp(0, 1, time / 0.8f);
        //     introText.color = c;
        //     yield return null;
        // }


        string[] intoText = new string[]
        {
            "Welcome to Master Chef Café.\nA mysterious order is coming..."
        };
        
        introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 1);
    
        foreach (string line in intoText)
        {
            yield return StartCoroutine(ShowLine(introText, line));
        } 
  
        GameManager.Instance.StartGame();  
    }
}
