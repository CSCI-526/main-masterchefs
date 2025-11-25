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
    [SerializeField] private string failureURL;
 
    private long sessionID;
    private int level;
    private float timeToComplete;

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
    
    // public void SendLevelTimeData(long sessionID, int level, int round, float timeToComplete)
    // {
    //     Debug.Log($"Sending Session: {sessionID}, Level: {level}, Round: {round}, Time(s): {timeToComplete}");
    //     StartCoroutine(PostLevelTimeData(sessionID.ToString(), level.ToString(), round.ToString(), timeToComplete.ToString()));
    // }
    //
    // private IEnumerator PostLevelTimeData(string sessionID, string level, string round, string timeToComplete)
    // {
    //     // Create the form and enter responses
    //     WWWForm form = new WWWForm();
    //     form.AddField("entry.592413526", sessionID);
    //     form.AddField("entry.442855023", level);
    //     form.AddField("entry.1689713574", timeToComplete);
    //
    //     // Send responses and verify result
    //     using (UnityWebRequest www = UnityWebRequest.Post(timeToCompleteURL, form))
    //     {
    //         yield return www.SendWebRequest();
    //
    //         if (www.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.Log(www.error);
    //         }
    //         else
    //         {
    //             Debug.Log("Form upload complete!");
    //         }
    //     }
    //
    // }
    //
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
        form.AddField("entry.1153827513", round);

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
    public void SendLevelComplete(long sessionID, int level, int round, string completionStatus, float completionTime, int totalAttempts, int finalRating)
    {
        Debug.Log($"Sending Level Complete: Session: {sessionID}, Level: {level}, Round: {round}, Status: {completionStatus}, Attempts: {totalAttempts}, Rating: {finalRating}");
        StartCoroutine(PostLevelComplete(sessionID.ToString(), level.ToString(), round.ToString(),completionStatus, completionTime.ToString(), totalAttempts.ToString(), finalRating.ToString()));
    }
    private IEnumerator PostLevelComplete(string sessionID, string level, string round, string completionStatus, string completionTime, string totalAttempts, string finalRating)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.1744163410", sessionID);
        form.AddField("entry.1328051060", level);
        form.AddField("entry.19381181", round);
        form.AddField("entry.902022430", completionStatus);
        form.AddField("entry.1023065139", completionTime);
        form.AddField("entry.1550860745", totalAttempts);
        form.AddField("entry.2014713914", finalRating);

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
    
    public void SendEarn(long sessionID, int level, int round, int amount, string dishName, int rating)
    {
        Debug.Log($"Sending Transaction Data: Session: {sessionID}, Level: {level}, Round: {round}, Amount: {amount}, Dish: {dishName}. rating: {rating}");
        StartCoroutine(PostEarn(sessionID.ToString(), level.ToString(), round.ToString(),amount.ToString(), dishName, rating.ToString()));
    }

    private IEnumerator PostEarn(string sessionID, string level, string round, string amount, string dishName,
        string rating)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.1570518334", sessionID);
        form.AddField("entry.351929486", level);
        form.AddField("entry.1474034226", round);
        form.AddField("entry.1284491017", amount);
        form.AddField("entry.69673593", dishName);
        form.AddField("entry.349720690", rating);



        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(earnURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Money spent event upload complete!");
            }
        }
    }

    public void SendSpend(long sessionID, int level, int round, int amount, string purchaseType, string cookware = "", string dishName="", int hintIndex = 0)
    {
        Debug.Log($"Sending Transaction Data: Session: {sessionID}, Level: {level}, Round: {round}, Amount: {amount}, PurchaseType: {purchaseType}, Cookware: {cookware}, Dish: {dishName}. HintIndex: {hintIndex}");
        StartCoroutine(PostSpend(sessionID.ToString(), level.ToString(), round.ToString(),amount.ToString(), purchaseType, cookware, dishName, hintIndex.ToString()));
    }

    private IEnumerator PostSpend(string sessionID, string level, string round, string amount, string purchaseType, string cookware, string dishName, string hintIndex)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.611321465", sessionID);
        form.AddField("entry.1010897800", level);
        form.AddField("entry.1929398080", round);
        form.AddField("entry.2063797328", amount);
        form.AddField("entry.809055744", purchaseType);
        form.AddField("entry.226808417", cookware);
        form.AddField("entry.331723635", dishName);
        form.AddField("entry.826326779", hintIndex);
        
        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(spendURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Money spent event upload complete!");
            }
        }
        
      
    }
    public void SendFailureData(long sessionID, int level, int round, int attempt, string dishName, string reason, string missingIngredients="", string wrongIngredients="", string overcookedIngredients="", string rawIngredients="", string wrongCookware="")
    {
        Debug.Log($"[Analytics Manager] Sending Failure Log: {sessionID}, Level: {level}, Round: {round}, attempt: {attempt}, Dish:{dishName}, Reason: {reason}, Missing Ingredients: {missingIngredients}, Wrong Ingredients:{wrongIngredients}, Overcooked Ingredients:{overcookedIngredients}, Raw Ingredients:{rawIngredients}, Wrong Cookware: {wrongCookware}");
        StartCoroutine(PostFailureData(sessionID.ToString(), level.ToString(), round.ToString(), attempt.ToString(), dishName, reason, missingIngredients, wrongIngredients, overcookedIngredients, rawIngredients, wrongCookware));
    }
    
    private IEnumerator PostFailureData(string sessionID, string level, string round, string attempt, string dishName, string reason, string missingIngredients="", string wrongIngredients="", string overcookedIngredients="", string rawIngredients="", string wrongCookware="")
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.694689951", sessionID);
        form.AddField("entry.1280208837", level);
        form.AddField("entry.39041271", round);
        form.AddField("entry.1663960025", attempt);
        form.AddField("entry.206919008", dishName);
        form.AddField("entry.892720662", reason);
        form.AddField("entry.1063221403", missingIngredients);
        form.AddField("entry.1568725325", wrongIngredients);
        form.AddField("entry.1437370441", overcookedIngredients);
        form.AddField("entry.935375524", rawIngredients);
        form.AddField("entry.1807950957", wrongCookware);
        
    
        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(failureURL, form))
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