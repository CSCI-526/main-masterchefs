using System.Collections.Generic;
using UnityEngine;

public class DraggableDish : MonoBehaviour
{
    [Header("Drag Settings")]
    public LayerMask dropZoneLayerMask = -1;
    public float dragOffset = 0.1f;

    [Header("Debug")]
    public bool enableDebugLogs = false;
    public Color hoverColor = Color.yellow;

    private Camera mainCamera;
    private Vector3 originalPosition;
    private Vector3 mouseOffset;

    private bool isDragging = false;
    private bool isHovering = false;

    private Collider2D col2D;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private Color originalColor;

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
    }

    void OnMouseDown()
    {
        Debug.Log("OnMouseDown called");
        if (!enabled) return;

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

        transform.position = mouseWorldPos + mouseOffset;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        StopDragging();
    }

    void OnMouseEnter()
    {
        isHovering = true;
        if (!isDragging && spriteRenderer != null)
            spriteRenderer.color = hoverColor;
    }

    void OnMouseExit()
    {
        isHovering = false;
        if (!isDragging && spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    // ───────────────────────────────────────────────────────────────
    // DRAGGING
    // ───────────────────────────────────────────────────────────────
    void StartDragging()
    {
        isDragging = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 200;
            spriteRenderer.color = hoverColor;
        }

        if (enableDebugLogs)
            Debug.Log($"[Dish] Start dragging");
    }

    void StopDragging()
    {
        isDragging = false;

        // Restore sorting + color
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
            spriteRenderer.color = isHovering ? hoverColor : originalColor;
        }

        IDropZone dropZone = GetDropZoneBelow();

        // ───────────────────────────
        // Dropped on TRASH
        // ───────────────────────────
        if (dropZone is Trash trash)
        {
            if (enableDebugLogs)
                Debug.Log("[Dish] Dropped on Trash — destroying dish");

            trash.OnDishDropped(this);
            return;
        }

        // ───────────────────────────
        // Not trash? Return to plate
        // ───────────────────────────
        if (enableDebugLogs)
            Debug.Log("[Dish] Returning to original position");

        ReturnToOriginalPosition();
    }

    // ───────────────────────────────────────────────────────────────
    // RAYCAST DETECTION (same system as DraggableIngredient)
    // ───────────────────────────────────────────────────────────────
    IDropZone GetDropZoneBelow()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        Bounds bounds = myCollider.bounds;

        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.01f);
        Vector2 direction = Vector2.down;
        float rayDistance = 1f;

        Debug.DrawRay(origin, direction * rayDistance, Color.cyan, 1f);

        RaycastHit2D[] hits =
            Physics2D.RaycastAll(origin, direction, rayDistance, dropZoneLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
                continue;

            IDropZone zone = hit.collider.GetComponent<IDropZone>();
            if (zone != null)
                return zone;
        }

        return null;
    }

    // ───────────────────────────────────────────────────────────────
    // RETURN TO START
    // ───────────────────────────────────────────────────────────────
    void ReturnToOriginalPosition()
    {
        StartCoroutine(ReturnCoroutine());
    }

    System.Collections.IEnumerator ReturnCoroutine()
    {
        Vector3 start = transform.position;
        Vector3 target = originalPosition;
        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            k = 1f - (1f - k) * (1f - k); // Ease out

            transform.position = Vector3.Lerp(start, target, k);
            yield return null;
        }

        transform.position = target;
    }

    // ───────────────────────────────────────────────────────────────
    // MOUSE UTILS  (copied from DraggableIngredient for consistency)
    // ───────────────────────────────────────────────────────────────
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane + dragOffset;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
