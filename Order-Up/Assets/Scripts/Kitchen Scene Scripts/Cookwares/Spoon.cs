using UnityEngine;

public class Spoon : MonoBehaviour
{
    public LayerMask cookwareLayer;
    public float stirSensitivity = 1.5f;

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 mouseOffset;
    private Pot potBelow;
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
            Pot cookware = hit.GetComponent<Pot>();
            if (cookware != null)
            {
                potBelow = cookware;
                
                if (stirIntensity > 0.01f)
                {
                    potBelow.StartStirring();
                }
          
            }
        }
        else
        {
            if (potBelow != null)
            {
                potBelow.StopStirring();
            }
            potBelow = null;
            
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

        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Pot cookware = other.GetComponent<Pot>();  // To check if spoon is colliding with pot
        if (cookware != null)
        {
            potBelow = cookware;
            Debug.Log($"[{gameObject.name}] Entered cookware {cookware.name}");
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Pot cookware = other.GetComponent<Pot>(); // To check if spoon leaves pot
        if (cookware != null && cookware == potBelow)
        {
            potBelow = null;
            Debug.Log($"[{gameObject.name}] Left cookware {cookware.name}");
            
                
        }
    }
}

