using UnityEngine;

public class Stirring : MonoBehaviour
{
    public Animator plateAnimator;
    public Transform spoon;
    public float stirSpeed = 100f;

    private bool isStirring = false;

    void OnMouseDown()
    {
        isStirring = true;
        if (plateAnimator != null)
            plateAnimator.SetBool("isStirring", true);
    }

    void OnMouseUp()
    {
        isStirring = false;
        if (plateAnimator != null)
            plateAnimator.SetBool("isStirring", false);
    }

    void Update()
    {
        if (isStirring && spoon != null)
        {
            // rotate spoon based on mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // make it feel like circular stirring
            Vector2 rotation = new Vector3(-mouseY, mouseX, 0f);
            spoon.Rotate(rotation * stirSpeed * Time.deltaTime, Space.Self);
        }
    }
}
