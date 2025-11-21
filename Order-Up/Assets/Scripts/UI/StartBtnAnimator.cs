using UnityEngine;

public class StartBtnAnimator : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void PressDown()
    {
        transform.localScale = originalScale * 0.9f;   
    }

    public void Release()
    {
        transform.localScale = originalScale;       
    }
}
