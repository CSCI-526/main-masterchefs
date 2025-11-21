using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    public RectTransform background;
    public TextMeshProUGUI introText;

    public void PlayIntro()
    {
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        // Step 1: Zoom in background
        Vector3 startScale = background.localScale;
        Vector3 endScale = startScale * 1.08f;
        float time = 0f;

        while (time < 0.6f)
        {
            time += Time.deltaTime;
            background.localScale = Vector3.Lerp(startScale, endScale, time / 0.6f);
            yield return null;
        }

        // Step 2: Fade in text
        introText.text = "Welcome to Café Bear.\nYour first customer is almost here...";
        Color c = introText.color;

        time = 0f;
        while (time < 0.8f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, time / 0.8f);
            introText.color = c;
            yield return null;
        }

        // Step 3: Hold for 1 second
        yield return new WaitForSeconds(1f);

        // Step 4: Load next scene
        SceneManager.LoadScene("Level1");
    }
}
