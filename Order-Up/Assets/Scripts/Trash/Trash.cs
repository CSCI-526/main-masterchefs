using UnityEngine;

public class Trash : MonoBehaviour
{
    [Header("Target Dish")]
    public GameObject dish; // Assign the Dish GameObject in Inspector

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private void OnMouseDown()
    {
        if (dish == null)
        {
            if (enableDebugLogs) Debug.LogWarning("[Trash] No Dish assigned!");
            return;
        }

        if (dish.transform.childCount == 0)
        {
            if (enableDebugLogs) Debug.Log("[Trash] Dish is already empty.");
            return;
        }

        // Loop through all children and destroy them
        for (int i = dish.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = dish.transform.GetChild(i);
            Destroy(child.gameObject);

            if (enableDebugLogs)
                Debug.Log($"[Trash] Deleted {child.name} from Dish.");
        }
    }
}
