using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;

public class AnalyticsManager : MonoBehaviour
{   
    public static AnalyticsManager Instance { get; private set; }
    [SerializeField] private string URL;
    private long sessionID;
    private int level;
    private float timeToComplete;
    public Button orderCompleteButton;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // private void Start()
    // {   
    //     orderCompleteButton.onClick.AddListener(Send);
    // }

    // private void OnDestroy()
    // {
    //     orderCompleteButton.onClick.RemoveListener(Send);
    // }
    public void Send(long sessionID, int level, float timeToComplete)
    {
        Debug.Log($"Sending Session: {sessionID}, Level: {level}, Time(s): {timeToComplete}");
        StartCoroutine(Post(sessionID.ToString(), level.ToString(), timeToComplete.ToString()));
    }

    private IEnumerator Post(string sessionID, string level, string timeToComplete)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.592413526", sessionID);
        form.AddField("entry.442855023", level);
        form.AddField("entry.1689713574", timeToComplete);

        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }

    }

}
