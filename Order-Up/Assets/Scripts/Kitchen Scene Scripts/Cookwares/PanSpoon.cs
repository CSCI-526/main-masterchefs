using UnityEngine;

public class PanSpoon : MonoBehaviour
{
    public LayerMask cookwareLayer;
    public float stirSensitivity = 1.5f;

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 mouseOffset;
    private Pan panBelow;
    private float stirIntensity = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindAnyObjectByType<Camera>();
        }
    }
    void Update()
    {
        if (!isDragging) return; // If player isn't moving or dragging spoon
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;
        // Apply offset so spoon doesn’t jump when clicked
        transform.position = mouseWorld + mouseOffset;
        // Calculate movement distance to determine stirring intensity
        float distanceMoved = Vector3.Distance(mouseWorld, lastMousePosition);
        stirIntensity = distanceMoved * stirSensitivity;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, cookwareLayer); // Check if spoon is moving over object 
        if (hit != null)
        {
            Pan cookware = hit.GetComponent<Pan>();
            if (cookware != null)
            {
                panBelow = cookware;

                if (stirIntensity > 0.05f)
                {
                    panBelow.StartStirring();
                }

            }
        }
        else
        {
            if (panBelow != null)
            {
                panBelow.StopStirring();
            }
            panBelow = null;

        }
        lastMousePosition = mouseWorld;
    }

    void OnMouseDown()
    {
        // Begin dragging only if clicking directly on spoon collider
        isDragging = true;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;
        mouseOffset = transform.position - mouseWorld;
        lastMousePosition = mouseWorld;
    }
    void OnMouseUp()
    {
        // Stop dragging when mouse released
        isDragging = false;
        stirIntensity = 0f; // Reset stirring intensity if spoon stops moving 

        if (panBelow != null)
        {
            panBelow.StopStirring();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Pan cookware = other.GetComponent<Pan>();  // To check if spoon is colliding with pan
        if (cookware != null)
        {
            panBelow = cookware;
            Debug.Log($"[{gameObject.name}] Entered cookware {cookware.name}");
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Pan cookware = other.GetComponent<Pan>(); // To check if spoon leaves pan
        if (cookware != null && cookware == panBelow)
        {
            panBelow = null;
            Debug.Log($"[{gameObject.name}] Left cookware {cookware.name}");


        }
    }
}

