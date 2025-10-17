using UnityEngine;

public class DraggableIngredient : MonoBehaviour
{
    [Header("Drag Settings")]
<<<<<<< HEAD
    public LayerMask plateLayerMask = -1; // Layer mask to identify plates
    public LayerMask potLayerMask = -1; // Layer mask to identify plates
    public float dragOffset = 0.1f; // How far in front of camera to place while dragging
    
    [Header("Cooked Result")]
    public DraggableIngredient cookedResult; // What this ingredient becomes when cooked

=======
    public LayerMask dropZoneLayerMask = -1; // Layer mask for both plates AND cookwares
    public float dragOffset = 0.1f;
>>>>>>> dave-branch

    [Header("Debug Settings")]
    public bool enableDebugLogs = false;
    public Color hoverColor = Color.yellow;

    private Camera mainCamera;
    private Vector3 originalPosition;
    private Vector3 mouseOffset;
    private bool isDragging = false;
    private bool isHovering = false;
    private Collider2D col2D;
    private int originalSortingOrder;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Events
    public System.Action<DraggableIngredient> OnStartDrag;
    public System.Action<DraggableIngredient> OnEndDrag;
    public System.Action<DraggableIngredient, Plate> OnDroppedOnPlate;
<<<<<<< HEAD
    public System.Action<DraggableIngredient, Pot> OnDroppedOnPot;
=======
    public System.Action<DraggableIngredient, Cookwares> OnDroppedOnCookware;
>>>>>>> dave-branch

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

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = transform.position.z;

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

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 100;
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

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
            spriteRenderer.color = isHovering ? hoverColor : originalColor;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Stopped dragging!");
        }

<<<<<<< HEAD
        // Check if we're over a valid drop zone
        Plate plateBelow = GetPlateBelow();
        Pot potBelow = GetPotBelow();
=======
        // Check what's below using the unified layer mask
        IDropZone dropZone = GetDropZoneBelow();
>>>>>>> dave-branch

        if (dropZone != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Dropped on: {dropZone.GetGameObject().name}");
            }
            
            // Handle based on type
            if (dropZone is Plate plate)
            {
                plate.AddIngredient(this);
                OnDroppedOnPlate?.Invoke(this, plate);
            }
            else if (dropZone is Cookwares cookware)
            {
                // Position ingredient inside cookware bounds
                transform.position = dropZone.GetGameObject().transform.position;
                OnDroppedOnCookware?.Invoke(this, cookware);
            }
        }
        else if (potBelow != null)
        {
            // Successfully dropped on plate
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Dropped on pot: {potBelow.name}");
            }
            potBelow.AddIngredient(this);
            OnDroppedOnPot?.Invoke(this, potBelow);
            transform.SetParent(potBelow.ingredientParent != null ? potBelow.ingredientParent : potBelow.transform);
            SetNewOriginalPosition();
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] No drop zone found, returning to original position");
            }
            ReturnToOriginalPosition();
        }

        OnEndDrag?.Invoke(this);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;

        if (!IsValidMousePosition(mousePos))
        {
            Debug.LogWarning($"[{gameObject.name}] Invalid mouse position detected: {mousePos}. Using fallback.");
            mousePos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Raw Mouse Position: {mousePos}");
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Using World Space Canvas: {canvas.name}");
            }

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mousePos, mainCamera, out localPoint))
            {
                Vector3 worldPos = canvasRect.TransformPoint(localPoint);

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

        if (mainCamera == null)
        {
            Debug.LogError($"[{gameObject.name}] No camera found for ScreenToWorldPoint!");
            return transform.position;
        }

        mousePos.z = mainCamera.nearClipPlane + dragOffset;
        Vector3 fallbackWorldPos = mainCamera.ScreenToWorldPoint(mousePos);

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

    IDropZone GetDropZoneBelow()
    {
        // Check using OverlapPoint with the unified layer mask
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, dropZoneLayerMask);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                // Try to get any component that implements IDropZone
                IDropZone dropZone = collider.GetComponent<IDropZone>();
                if (dropZone != null)
                    return dropZone;
            }
        }

        return null;
    }

    Pot GetPotBelow()
    {
        // Raycast downward to find plates
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 0f, potLayerMask);

        if (hit.collider != null)
        {
            return hit.collider.GetComponent<Pot>();
        }

        // Alternative method using OverlapPoint
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, potLayerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject) // Don't detect ourselves
            {
                Pot pot = collider.GetComponent<Pot>();
                if (pot != null)
                    return pot;
            }
        }

        return null;
    }

    void ReturnToOriginalPosition()
    {
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
            t = 1f - (1f - t) * (1f - t);

            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            yield return null;
        }

        transform.position = originalPosition;
    }

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