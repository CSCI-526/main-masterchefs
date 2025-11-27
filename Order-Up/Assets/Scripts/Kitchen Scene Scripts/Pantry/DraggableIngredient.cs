using UnityEngine;

public class DraggableIngredient : MonoBehaviour
{
    [Header("Drag Settings")]
    public LayerMask dropZoneLayerMask = -1;
    public float dragOffset = 0.1f;

    [Header("Debug Settings")]
    public bool enableDebugLogs = false;
    public Color hoverColor = Color.yellow;

    private Camera mainCamera;
    private Vector3 originalPosition;
    private Vector3 mouseOffset;
    private bool isDragging = false;
    private bool isHovering = false;

    private Collider2D col2D;
    private Rigidbody2D rb2D;
    private int originalSortingOrder;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Events
    public System.Action<DraggableIngredient> OnStartDrag;
    public System.Action<DraggableIngredient> OnEndDrag;
    public System.Action<DraggableIngredient, Plate> OnDroppedOnPlate;
    public System.Action<DraggableIngredient, BaseCookware> OnDroppedOnCookware;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();

        originalPosition = transform.position;
        col2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
            originalColor = spriteRenderer.color;
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

        StartDragging();
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        mouseWorldPos.z = transform.position.z;

        Vector3 newPosition = mouseWorldPos + mouseOffset;
        transform.position = newPosition;
        rb2D.MovePosition(newPosition);
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
    }

    void OnMouseExit()
    {
        if (!enabled) return;

        isHovering = false;
        if (spriteRenderer != null && !isDragging)
        {
            spriteRenderer.color = originalColor;
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
            Debug.Log($"[{gameObject.name}] Started dragging");
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
            Debug.Log($"[{gameObject.name}] Stopped dragging at position: {transform.position}");
        }

        // Check what's below using the unified layer mask
        IDropZone dropZone = GetDropZoneBelow();
        

        if (dropZone != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{gameObject.name}] Dropped on: {dropZone.GetGameObject().name}");
            }

            // Handle based on type
            if (dropZone is Plate plate)
            {
                if (plate.AddIngredient(this))
                {
                    OnDroppedOnPlate?.Invoke(this, plate);

                    // Notify the TutorialManager that ingredient has been dropped on plate
                    if (TutorialManager.Instance != null)
                    {
                        TutorialManager.Instance.OnIngredientDroppedOnPlate(this, plate);
                    }

                }
                else
                {
                    Debug.Log($"[{gameObject.name}] Plate is full or cannot accept ingredient, returning to original position");
                    ReturnToOriginalPosition();
                }
            }
            else if (dropZone is BaseCookware cookware)
            {
                // Check if cookware can accept this ingredient (only one at a time)
                if (cookware.CanAcceptIngredient(this.gameObject))
                {
                    // Position ingredient inside cookware bounds
                    transform.position = dropZone.GetGameObject().transform.position;
                    OnDroppedOnCookware?.Invoke(this, cookware);
                    cookware.ManuallyAcceptIngredient(gameObject);
                    cookware.SetIngredientParent(gameObject);

                    // Notify TutorialManager that ingredient has been dropped on cookware
                    if (TutorialManager.Instance != null)
                    {
                        TutorialManager.Instance.OnIngredientDroppedOnCookware(this, cookware);
                    }

                    if (enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] Successfully dropped in {cookware.GetCookwareType()}");
                    }
                }
                else
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[{gameObject.name}] Cookware is full or cooking, returning to original position");
                    }
                    ReturnToOriginalPosition();
                }
            }
            else if (dropZone is Trash trash)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[{gameObject.name}] Dropped on Trash: {trash.GetGameObject().name}");
                }

                trash.OnIngredientDropped(this);
                return;
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[{gameObject.name}] Dropped on unknown drop zone type, returning to original position");
                ReturnToOriginalPosition();
            }
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log($"[{gameObject.name}] No drop zone found, returning to original position");
            ReturnToOriginalPosition();
        }

        OnEndDrag?.Invoke(this);
        Physics2D.SyncTransforms();
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;

        if (!IsValidMousePosition(mousePos))
        {
            Debug.LogWarning($"[{gameObject.name}] Invalid mouse position detected. Using fallback.");
            mousePos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mousePos, mainCamera, out localPoint))
            {
                Vector3 worldPos = canvasRect.TransformPoint(localPoint);

                if (IsValidWorldPosition(worldPos))
                {
                    return worldPos;
                }
            }
        }

        if (mainCamera == null)
        {
            Debug.LogError($"[{gameObject.name}] No camera found!");
            return transform.position;
        }

        mousePos.z = mainCamera.nearClipPlane + dragOffset;
        Vector3 fallbackWorldPos = mainCamera.ScreenToWorldPoint(mousePos);

        if (!IsValidWorldPosition(fallbackWorldPos))
        {
            return transform.position;
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
        // Get ingredient collider bounds
        Collider2D myCollider = GetComponent<Collider2D>();
        Bounds bounds = myCollider.bounds;

        // Start the ray just below the ingredient�s collider
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.01f);
        Vector2 direction = Vector2.down;
        float rayDistance = 1f;
        Debug.DrawRay(origin, direction * rayDistance, Color.green, 1f);
        // Raycast only against DropZone layers
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, rayDistance, dropZoneLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
                continue; // skip self-hit

            //Debug.Log($"[DEBUG] Raycast hit: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            IDropZone dropZone = hit.collider.GetComponent<IDropZone>();
            if (dropZone != null)
                return dropZone;
        }

        Debug.Log($"[{gameObject.name}] No DropZone detected by raycast");
        return null;
    }

    void ReturnToOriginalPosition()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Returning to original position: {originalPosition}");
        }

        StartCoroutine(ReturnToPositionCoroutine());
    }

    System.Collections.IEnumerator ReturnToPositionCoroutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = originalPosition;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - (1f - t) * (1f - t); // Ease out

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }

    public void SetNewOriginalPosition()
    {
        originalPosition = transform.position;

        if (enableDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] Original position set to: {originalPosition}");
        }
    }

    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }

    public void EnableDragging()
    {
        enabled = true;
    }

    public bool IsDragging => isDragging;

}