using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;
    private Text tooltipText;
    private RectTransform backgroundRectTransform;

    private void Awake() {
        instance = this;
        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.Find("text").GetComponent<Text>();
        HideToolTip(); 

    }

    private void Update() {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out localPoint);
        transform.localPosition = localPoint;
    }


    private void ShowToolTip(string tooltipString) {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);

        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void HideToolTip() {
        gameObject.SetActive(false);
    }

    public static void ShowToolTip_Static(string tooltipString) {
        if (instance != null) 
        {
            instance.ShowToolTip(tooltipString);
        } 
        else 
        {
            Debug.LogError("Tooltip instance is NULL");
        }
    }

    public static void HideToolTip_Static() {
        if (instance != null) 
        {
            instance.HideToolTip();
        }
    }
}
