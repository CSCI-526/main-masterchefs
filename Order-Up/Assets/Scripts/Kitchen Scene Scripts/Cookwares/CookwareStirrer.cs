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
    [SerializeField] private float followLerpSpeed = 12f;
    [SerializeField] private float returnLerpSpeed = 6f;
    private Vector3 startPosition;


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
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isDragging)
        {
            // Spoon to return to original start position when not being dragged
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * returnLerpSpeed);
            return;
        }

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        // Move spoon with mouse
        transform.position = Vector3.Lerp(transform.position, mouseWorld + mouseOffset, Time.deltaTime * followLerpSpeed);

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
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        mouseOffset = transform.position - mouseWorld;
        lastMousePosition = mouseWorld;
        isDragging = true;

        if (enableDebugLogs)
        {
            Debug.Log("[Stirrer] Started dragging");
        }
    }


    void OnMouseUp()
    {
        isDragging = false;
        stirIntensity = 0f;

        // Stop stirring 
        if (currentCookware != null)
        {
            currentCookware.StopStirring();
            currentCookware = null;
        }

        if (enableDebugLogs)
        {
            Debug.Log("[Stirrer] Stopped dragging, returning to rest");
        }

        transform.position = Vector3.Lerp(transform.position, startPosition, 1f);
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