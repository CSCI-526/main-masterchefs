using UnityEngine;

public class PopupTextUI : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ShowPopup()
    {
        anim.SetTrigger("ShowPopup");
    }
}