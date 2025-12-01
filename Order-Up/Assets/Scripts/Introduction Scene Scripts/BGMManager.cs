using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio Source")]
    private AudioSource audioSource;

    [Header("BGM Tracks")]
    public AudioClip introBGM;
    public AudioClip customerBGM;
    public AudioClip kitchenBGM;

    [Header("Volume Settings")]
    public float maxVolume = 0.8f;   
    public float fadeDuration = 0.8f;  

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // play different BGM based on scene
        if (scene.name == "IntroScene")
        {
            PlayBGM(introBGM);
        }
        else if (scene.name == "CustomerScene")
        {
            PlayBGM(customerBGM);
        }
        else if (scene.name == "KitchenScene")
        {
            PlayBGM(kitchenBGM);
        }
    }

 

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("BGMManager: Missing AudioClip.");
            return;
        }

        // if already playing this clip, do nothing
        if (audioSource.clip == clip) return;

        StartCoroutine(FadeToNewBGM(clip));
    }

    //  BGM fade

    private IEnumerator FadeToNewBGM(AudioClip newClip)
    { 
        
        // fade out current music
        while (audioSource.volume > 0.01f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime  * 3f);
            yield return null;
        }
        audioSource.volume = 0f; // ensure volume is zero

        // change clip
        audioSource.clip = newClip;
        audioSource.Play();

        // fade in new music
        float velocity = 0f;
        while (audioSource.volume < maxVolume - 0.02f)
        {
            audioSource.volume = Mathf.SmoothDamp(audioSource.volume, maxVolume, ref velocity, fadeDuration);
            yield return null;
        }

        audioSource.volume = maxVolume; // ensure max volume
    }

 
}
