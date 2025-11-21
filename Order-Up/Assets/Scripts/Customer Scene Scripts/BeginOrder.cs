using UnityEngine;

public class BeginOrder : MonoBehaviour
{
    public void onOrderStart()
    {
        long sessionID = GameManager.Instance.SessionID;
        int level = GameData.CurrentLevel;
        int round = GameData.CurrentRound;
        
        AnalyticsManager.Instance.SendLevelStart(sessionID, level, round);
        // Debug.Log($"sent sessionID:{sessionID} level:{level} round:{round} to analytics manager");
    }
}