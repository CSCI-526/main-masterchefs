using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Stirring : MonoBehaviour
{
    [Header("Stirring Settings")]
    public LayerMask cookwareLayer;
    public float stirSensitivity = 1.5f; // How fast the player must move to stir more
    public float maxStirMultiplier = 2.5f; 

    private Vector3 lastMousePosition;
    private float stirIntensity = 0f;
    private Pot potBelow;

    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 mouseOffset;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();
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
        stirIntensity = Mathf.Clamp(distanceMoved * stirSensitivity, 0f, maxStirMultiplier);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, cookwareLayer); // Check if spoon is moving over object 
        if (hit != null)
        {
            Pot cookware = hit.GetComponent<Pot>();
            if (cookware != null)
            {
                potBelow = cookware;
                potBelow.ApplyStirring(stirIntensity); // Register that pot is cooking
            }
        }
        else
        {
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

        if (potBelow != null)
            potBelow.StopStirring();
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
            potBelow.StopStirring();
            potBelow = null;
            Debug.Log($"[{gameObject.name}] Left cookware {cookware.name}");
        }
    }
}
