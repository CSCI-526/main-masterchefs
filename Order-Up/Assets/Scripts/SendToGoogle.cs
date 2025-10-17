using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;

public class SendToGoogle : MonoBehaviour
{
    [SerializeField] private string URL;
    private long sessionID;
    private int level;
    private float timeToComplete;
    public Button orderCompleteButton;


    private void Awake()
    {
        sessionID = DateTime.Now.Ticks;
    }

    private void Start()
    {
        orderCompleteButton.onClick.AddListener(Send);
    }

    private void OnDestroy()
    {
        orderCompleteButton.onClick.RemoveListener(Send);
    }
    public void Send()
    {
        timeToComplete = Timer.Instance.ElapsedSeconds;
        level = 1; //todo: retrieve current level
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
