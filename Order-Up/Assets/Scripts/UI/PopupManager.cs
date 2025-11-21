using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public GameObject popupPrefab;    
    public Canvas worldCanvas;        

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(string message, Transform target)
    {
        // Instantiate popup under canvas
        GameObject popup = Instantiate(popupPrefab, worldCanvas.transform);

        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        float spriteHeight = (sr != null) ? sr.bounds.size.y : 1f;

        Vector3 offset = new Vector3(0, spriteHeight * 1.4f, 0);    
        popup.transform.position = target.position + offset;
 


        Debug.Log("POPUP SPAWNED: " + message);

        // Set the text
        TMP_Text text = popup.transform.Find("Text").GetComponent<TMP_Text>();
        text.text = message;

        // Auto-destroy after animation 
        Destroy(popup, 2.7f);
    }
}
