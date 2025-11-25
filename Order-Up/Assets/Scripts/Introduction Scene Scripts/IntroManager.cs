using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    public Button button;
    public RectTransform background;
    public TextMeshProUGUI introText;
    public Image introPanel; 

    private void Awake()
    {
        button.onClick.AddListener(PlayIntroSequence);
    }

    private void PlayIntroSequence()
    {
        button.interactable = false;  // Disable button to prevent multiple clicks
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        // Background zoom-in 
        Vector3 startScale = background.localScale;
        Vector3 endScale = startScale * 1.08f;
        float time = 0f;

        while (time < 0.6f)
        {
            time += Time.deltaTime;
            background.localScale = Vector3.Lerp(startScale, endScale, time / 0.6f);
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
        Color c = introText.color;

        time = 0f;
        while (time < 0.8f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, time / 0.8f);
            introText.color = c;
            yield return null;
        }

        //  Hold for 2.5 seconds
        yield return new WaitForSeconds(2.5f);

 
        GameManager.Instance.StartGame();
    }
}
