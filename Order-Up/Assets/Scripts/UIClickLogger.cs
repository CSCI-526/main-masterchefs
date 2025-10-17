using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickLogger : MonoBehaviour
{
    [SerializeField] private bool enableDebugLogs = false;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                string log = "UI Raycast Results:";
                foreach (var result in results)
                {
                    log += "\n" + result.gameObject.name;
                }
                if (enableDebugLogs) Debug.Log(log);
            }
            else
            {
                if (enableDebugLogs) Debug.Log("UI Raycast Results: None");
            }
        }
    }
}
