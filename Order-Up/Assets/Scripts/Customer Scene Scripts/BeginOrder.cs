using UnityEngine;

public class BeginOrder : MonoBehaviour
{
    public void onOrderStart()
    {
        long sessionID = GameManager.Instance.SessionID;
        int level = GameManager.Instance.CurrentLevel;
        AnalyticsManager.Instance.SendLevelStart(sessionID, level);
        Debug.Log($"sent sessionID:{sessionID} level:{level} to analytics manager"); // testing
    }
}
