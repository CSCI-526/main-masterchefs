using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;

public class AnalyticsManager : MonoBehaviour
{   
    public static AnalyticsManager Instance { get; private set; }
    [SerializeField] private string timeToCompleteURL;
    [SerializeField] private string levelStartURL;
    [SerializeField] private string levelCompleteURL;
    [SerializeField] private string earnURL;
    [SerializeField] private string spendURL;
 
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
    
    public void SendLevelTimeData(long sessionID, int level, int round, float timeToComplete)
    {
        Debug.Log($"Sending Session: {sessionID}, Level: {level}, Round: {round}, Time(s): {timeToComplete}");
        StartCoroutine(PostLevelTimeData(sessionID.ToString(), level.ToString(), round.ToString(), timeToComplete.ToString()));
    }

    private IEnumerator PostLevelTimeData(string sessionID, string level, string round, string timeToComplete)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.592413526", sessionID);
        form.AddField("entry.442855023", level);
        form.AddField("entry.1689713574", timeToComplete);

        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(timeToCompleteURL, form))
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
    
    public void SendLevelStart(long sessionID, int level, int round)
    {
        Debug.Log($"Sending Level Start: Session: {sessionID}, Level: {level}, Round: {round}");
        StartCoroutine(PostLevelStart(sessionID.ToString(), level.ToString(), round.ToString()));
    }
    private IEnumerator PostLevelStart(string sessionID, string level, string round)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.2120502415", sessionID);
        form.AddField("entry.1172461924", level);

        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(levelStartURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Level Start event upload complete!");
            }
        }
    }
    public void SendLevelComplete(long sessionID, int level, int round, string completionStatus, int totalAttempts, int finalRating, string reason)
    {
        Debug.Log($"Sending Level Complete: Session: {sessionID}, Level: {level}, Round: {round}, Status: {completionStatus}, Attempts: {totalAttempts}, Rating: {finalRating}, Reason: {reason}");
        StartCoroutine(PostLevelComplete(sessionID.ToString(), level.ToString(), round.ToString(),completionStatus, totalAttempts.ToString(), finalRating.ToString(), reason));
    }
    private IEnumerator PostLevelComplete(string sessionID, string level, string round, string completionStatus, string totalAttempts, string finalRating, string reason)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.1744163410", sessionID);
        form.AddField("entry.1328051060", level);
        form.AddField("entry.902022430", completionStatus);
        form.AddField("entry.1550860745", totalAttempts);
        form.AddField("entry.2014713914", finalRating);
        form.AddField("entry.1257113581", reason);

        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(levelCompleteURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Level Complete event upload complete!");
            }
        }
    }
    
    
}