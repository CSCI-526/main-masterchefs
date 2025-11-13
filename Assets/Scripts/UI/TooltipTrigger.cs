using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string tooltipText = "Default tooltip text.";

    // void Start()
    // {
    //     // tooltipSystem = FindObjectOfType<TooltipSystem>(); 
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("in!");
        Tooltip.ShowToolTip_Static(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("out!");
        Tooltip.HideToolTip_Static();
    }
}