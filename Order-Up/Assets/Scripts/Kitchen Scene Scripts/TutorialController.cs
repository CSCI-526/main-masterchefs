using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;
    public Image highlightFrame;
    public Image arrow;

    [Header("Targets")]
    public GameObject potato;
    public GameObject fryer;
    public GameObject plate;
    public GameObject trash;
    public GameObject orderUpButton;

    private int step = 0;
    private bool tutorialEnabled = true;
    private bool advanceAfterOrder = false;
    private const string TutorialFinishedKey = "TutorialFinished";

    void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // check if the tutorial is finished already
        if (PlayerPrefs.GetInt(TutorialFinishedKey, 0) == 1)
        {
            tutorialEnabled = false;
            tutorialPanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void HandleOrderAccepted()
    {
        advanceAfterOrder = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!tutorialEnabled)
        {
            tutorialPanel.SetActive(false);
            return;
        }

        if (scene.name == "KitchenScene")
        {
            tutorialPanel.SetActive(true);
            StartCoroutine(DelayedSetup()); // delay executing the tutorial
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(0.2f); // waiting for the scene to finish being loaded
        AutoBindTargets();
        if (advanceAfterOrder)
        {
            step = Mathf.Max(step, 1);
            advanceAfterOrder = false;
        }
        StartCoroutine(ShowStep(step));
    }
 

    private void AutoBindTargets()
    {
          
        potato = GameObject.Find("Potato") ?? GameObject.FindWithTag("Potato");
        fryer = GameObject.Find("Fryer") ?? GameObject.FindWithTag("Fryer");
        plate = GameObject.Find("Plate") ?? GameObject.FindWithTag("Plate");
        trash = GameObject.Find("Trash") ?? GameObject.FindWithTag("Trash");
        orderUpButton = GameObject.Find("SubmitButton") ?? GameObject.FindWithTag("SubmitButton");

          
        if (!potato) Debug.LogWarning("Potato not found in scene.");
        if (!fryer) Debug.LogWarning("Fryer not found in scene.");
        if (!plate) Debug.LogWarning("Plate not found in scene.");
        if (!trash) Debug.LogWarning("Trash not found in scene.");
        if (!submitButton) Debug.LogWarning("SubmitButton not found in scene.");
    }

    private void AddClickListener(GameObject target)
    {
        if (target == null) return;

        // for UI Button
        Button btn = target.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(NextStep);
            return;
        }

        // for other game objects
        var trigger = target.GetComponent<TutorialClickTrigger>();
        if (trigger == null)
            trigger = target.AddComponent<TutorialClickTrigger>();

        
        trigger.tutorial = this;
    }

    IEnumerator ShowStep(int index)
    {
        Debug.Log($"[Tutorial] ShowStep {index}");

        if (!tutorialEnabled || !tutorialPanel.activeInHierarchy) yield break;

        step = index;
        highlightFrame.gameObject.SetActive(true);
        arrow.gameObject.SetActive(true);

        switch (index)
        {
            case 0:
                yield return SetInstruction("Grab the potato! Our star ingredient today!", potato);
                break;
            case 1:
                yield return SetInstruction("Drag it into the fryer, and keep an eye on the timer!", fryer);
                break;
            case 2:
                yield return SetInstruction("Now move it to the plate. This is where we combine the ingredients.", plate);
                break;
            case 3:
                yield return SetInstruction("Put the wrong ingredient? Tap Trash to clear.", trash);
                break;
            case 4:
                yield return SetInstruction("All set? Hit Order Up to serve your masterpiece!", orderUpButton);
                break;
            case 5:
                tutorialPanel.SetActive(false);
                tutorialEnabled = false;

                // save the tutorial finish state 
                PlayerPrefs.SetInt(TutorialFinishedKey, 1);
                PlayerPrefs.Save();

                Debug.Log("Tutorial finished!");
                yield break;
        }
    }

    IEnumerator SetInstruction(string text, GameObject target)
    {
        yield return FadeText(text);
        if (target != null)
        {
            Vector3 pos = target.transform.position;
            highlightFrame.transform.position = pos;
            arrow.transform.position = pos + new Vector3(0, -100, 0);
        }
    }

    IEnumerator FadeText(string newText)
    {
        tutorialText.CrossFadeAlpha(0, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        tutorialText.text = newText;
        tutorialText.CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(0.4f);
    }

    public void NextStep()
    {
        step++;
        StartCoroutine(ShowStep(step));
    }
}
