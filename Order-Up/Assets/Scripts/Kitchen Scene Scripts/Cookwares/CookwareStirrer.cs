using UnityEngine;

/// <summary>
/// Unified spoon/stirrer that works with any StirBasedCookware
/// </summary>
public class CookwareStirrer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask cookwareLayer;
    [SerializeField] private float stirSensitivity = 1.5f;
    [SerializeField] private float stirThreshold = 0.05f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 mouseOffset;
    private StirBasedCookware currentCookware;
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
        if (!isDragging) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        // Move spoon with mouse
        transform.position = mouseWorld + mouseOffset;

        // Calculate stirring intensity based on movement
        float distanceMoved = Vector3.Distance(mouseWorld, lastMousePosition);
        stirIntensity = distanceMoved * stirSensitivity;

        // Check if spoon is over cookware
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, cookwareLayer);

        if (hit != null)
        {
            StirBasedCookware cookware = hit.GetComponent<StirBasedCookware>();

            if (cookware != null)
            {
                // If we're over a different cookware, stop stirring the old one
                if (currentCookware != null && currentCookware != cookware)
                {
                    currentCookware.StopStirring();
                }

                currentCookware = cookware;

                // Start stirring if moving enough
                if (stirIntensity > stirThreshold)
                {
                    currentCookware.StartStirring();

                    if (enableDebugLogs)
                    {
                        Debug.Log($"[Stirrer] Stirring {cookware.name} with intensity {stirIntensity}");
                    }
                }
            }
        }
        else
        {
            // Not over any cookware
            if (currentCookware != null)
            {
                currentCookware.StopStirring();
                currentCookware = null;
            }
        }

        lastMousePosition = mouseWorld;
    }

    void OnMouseDown()
    {
        isDragging = true;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;
        mouseOffset = transform.position - mouseWorld;
        lastMousePosition = mouseWorld;

        if (enableDebugLogs)
        {
            Debug.Log($"[Stirrer] Started dragging");
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        stirIntensity = 0f;

        // Stop stirring when releasing spoon
        if (currentCookware != null)
        {
            currentCookware.StopStirring();
            currentCookware = null;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[Stirrer] Stopped dragging");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        StirBasedCookware cookware = other.GetComponent<StirBasedCookware>();
        if (cookware != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[Stirrer] Entered {cookware.name}");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        StirBasedCookware cookware = other.GetComponent<StirBasedCookware>();
        if (cookware != null && cookware == currentCookware)
        {
            currentCookware.StopStirring();
            currentCookware = null;

            if (enableDebugLogs)
            {
                Debug.Log($"[Stirrer] Left {cookware.name}");
            }
        }
    }
}