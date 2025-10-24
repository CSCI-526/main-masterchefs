using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialClickTrigger : MonoBehaviour, IPointerClickHandler
{
    public TutorialController tutorial;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tutorial != null)
        {
            tutorial.NextStep();
        }
    }
}
