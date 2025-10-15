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
        if (!isDragging) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        // Apply offset so spoon doesn’t jump when clicked
        transform.position = mouseWorld + mouseOffset;

        // Calculate movement distance to determine stirring intensity
        float distanceMoved = Vector3.Distance(mouseWorld, lastMousePosition);
        stirIntensity = Mathf.Clamp(distanceMoved * stirSensitivity, 0f, maxStirMultiplier);

        if (potBelow != null)
            potBelow.ApplyStirring(stirIntensity);

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
        stirIntensity = 0f;

        if (potBelow != null)
            potBelow.StopStirring();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Pot cookware = other.GetComponent<Pot>();
        if (cookware != null)
        {
            potBelow = cookware;
            Debug.Log($"[{gameObject.name}] Entered cookware {cookware.name}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Pot cookware = other.GetComponent<Pot>();
        if (cookware != null && cookware == potBelow)
        {
            potBelow.StopStirring();
            potBelow = null;
            Debug.Log($"[{gameObject.name}] Left cookware {cookware.name}");
        }
    }
}
