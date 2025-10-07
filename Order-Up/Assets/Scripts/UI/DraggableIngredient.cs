using UnityEngine;

public class DraggableIngredient : MonoBehaviour
{
    [Header("Drag Settings")]
    public LayerMask plateLayerMask = -1; // Layer mask to identify plates
    public float dragOffset = 0.1f; // How far in front of camera to place while dragging
    
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public Color hoverColor = Color.yellow;
    
    private Camera mainCamera;
    private Vector3 originalPosition;
    private Vector3 mouseOffset; // Offset between mouse and object when dragging starts
    private bool isDragging = false;
    private bool isHovering = false;
    private Collider2D col2D;
    private int originalSortingOrder;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Events for other systems to hook into
    public System.Action<DraggableIngredient> OnStartDrag;
    public System.Action<DraggableIngredient> OnEndDrag;
    public System.Action<DraggableIngredient, Plate> OnDroppedOnPlate;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();
            
        originalPosition = transform.position;
        col2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
            originalColor = spriteRenderer.color;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] DraggableIngredient initialized. Camera: {(mainCamera != null ? mainCamera.name : "NULL")}, Collider: {(col2D != null ? "Found" : "NULL")}, SpriteRenderer: {(spriteRenderer != null ? "Found" : "NULL")}");
        }
    }

    void Update()
    {
        // Debug mouse position continuously (can be disabled via enableDebugLogs)
        if (enableDebugLogs && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Debug.Log($"[MOUSE DEBUG] Screen: {Input.mousePosition}, World: {mouseWorldPos}");
        }
    }

    void OnMouseDown()
    {
        if (!enabled) return;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] OnMouseDown triggered!");
        }
        
        // Calculate the offset between mouse position and object position
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = transform.position.z;
        mouseOffset = transform.position - mouseWorldPos;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Mouse World Pos: {mouseWorldPos}, Object Pos: {transform.position}, Offset: {mouseOffset}");
        }
        
        StartDragging();
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        
        // Get mouse position in world coordinates and apply the offset
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = transform.position.z;
        
        // Apply the offset so the object doesn't jump to mouse position
        Vector3 newPosition = mouseWorldPos + mouseOffset;
        transform.position = newPosition;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Dragging - Mouse: {mouseWorldPos}, New Pos: {newPosition}");
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] OnMouseUp triggered!");
        }
        
        StopDragging();
    }

    void OnMouseEnter()
    {
        if (!enabled) return;
        
        isHovering = true;
        if (spriteRenderer != null && !isDragging)
        {
            spriteRenderer.color = hoverColor;
        }
        
        if (enableDebugLogs)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Debug.Log($"[{gameObject.name}] Mouse ENTERED - Mouse World Pos: {mouseWorldPos}, Object Pos: {transform.position}");
        }
    }

    void OnMouseExit()
    {
        if (!enabled) return;
        
        isHovering = false;
        if (spriteRenderer != null && !isDragging)
        {
            spriteRenderer.color = originalColor;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Mouse EXITED");
        }
    }

    void StartDragging()
    {
        isDragging = true;
            
        // Bring to front while dragging
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 100;
            // Keep hover color while dragging
            spriteRenderer.color = hoverColor;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Started dragging!");
        }
            
        OnStartDrag?.Invoke(this);
    }

    void StopDragging()
    {
        isDragging = false;

        // Reset sorting order and color
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
            // Reset to original color unless still hovering
            spriteRenderer.color = isHovering ? hoverColor : originalColor;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Stopped dragging!");
        }
        
        // Check if we're over a valid drop zone
        Plate plateBelow = GetPlateBelow();

        if (plateBelow != null)
        {
            // Successfully dropped on plate
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Dropped on plate: {plateBelow.name}");
            }
            plateBelow.AddIngredient(this);
            OnDroppedOnPlate?.Invoke(this, plateBelow);
        }
        else
        {
            // Return to original position if not dropped on plate
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] No plate found, returning to original position");
            }
            ReturnToOriginalPosition();
        }
        
        OnEndDrag?.Invoke(this);
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        
        // Validate mouse position first
        if (!IsValidMousePosition(mousePos))
        {
            Debug.LogWarning($"[{gameObject.name}] Invalid mouse position detected: {mousePos}. Using fallback.");
            mousePos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Raw Mouse Position: {mousePos}");
        }
        
        // Check if we're using a Canvas in World Space
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Using World Space Canvas: {canvas.name}");
            }
            
            // For World Space Canvas, we need to account for the canvas transform
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            
            // Convert screen point to local point on the canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mousePos, mainCamera, out localPoint))
            {
                // Convert local canvas point to world position
                Vector3 worldPos = canvasRect.TransformPoint(localPoint);
                
                // Validate world position
                if (!IsValidWorldPosition(worldPos))
                {
                    Debug.LogWarning($"[{gameObject.name}] Invalid world position from canvas: {worldPos}. Using transform position.");
                    return transform.position;
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] Canvas Local Point: {localPoint}, World Pos: {worldPos}");
                }
                
                return worldPos;
            }
        }
        
        // Validate camera before using it
        if (mainCamera == null)
        {
            Debug.LogError($"[{gameObject.name}] No camera found for ScreenToWorldPoint!");
            return transform.position;
        }
        
        // Fallback to standard screen-to-world conversion for non-Canvas objects
        mousePos.z = mainCamera.nearClipPlane + dragOffset;
        Vector3 fallbackWorldPos = mainCamera.ScreenToWorldPoint(mousePos);
        
        // Validate fallback position
        if (!IsValidWorldPosition(fallbackWorldPos))
        {
            Debug.LogWarning($"[{gameObject.name}] Invalid fallback world position: {fallbackWorldPos}. Using transform position.");
            return transform.position;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Using fallback ScreenToWorldPoint: {fallbackWorldPos}");
        }
        
        return fallbackWorldPos;
    }
    
    bool IsValidMousePosition(Vector3 mousePos)
    {
        return !float.IsNaN(mousePos.x) && !float.IsNaN(mousePos.y) && !float.IsNaN(mousePos.z) &&
               !float.IsInfinity(mousePos.x) && !float.IsInfinity(mousePos.y) && !float.IsInfinity(mousePos.z);
    }
    
    bool IsValidWorldPosition(Vector3 worldPos)
    {
        return !float.IsNaN(worldPos.x) && !float.IsNaN(worldPos.y) && !float.IsNaN(worldPos.z) &&
               !float.IsInfinity(worldPos.x) && !float.IsInfinity(worldPos.y) && !float.IsInfinity(worldPos.z) &&
               Mathf.Abs(worldPos.x) < 1000000f && Mathf.Abs(worldPos.y) < 1000000f && Mathf.Abs(worldPos.z) < 1000000f;
    }

    Plate GetPlateBelow()
    {
        // Raycast downward to find plates
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 0f, plateLayerMask);
        
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<Plate>();
        }
        
        // Alternative method using OverlapPoint
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, plateLayerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject) // Don't detect ourselves
            {
                Plate plate = collider.GetComponent<Plate>();
                if (plate != null)
                    return plate;
            }
        }
        
        return null;
    }

    void ReturnToOriginalPosition()
    {
        // Smoothly return to original position
        StartCoroutine(ReturnToPositionCoroutine());
    }
    
    System.Collections.IEnumerator ReturnToPositionCoroutine()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float duration = 0.3f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Use easing for smooth return
            t = 1f - (1f - t) * (1f - t); // Ease out quad
            
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            yield return null;
        }
        
        transform.position = originalPosition;
    }
    
    // Call this if you want to reset the ingredient's original position
    public void SetNewOriginalPosition()
    {
        originalPosition = transform.position;
    }
    
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
    
    public void EnableDragging()
    {
        enabled = true;
    }
}