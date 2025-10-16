using UnityEngine;
using UnityEngine.UI; // Add this for UI
using TMPro; // Add this for TextMeshPro

public class TrashBin : MonoBehaviour
{
    [Header("Trash Bin Settings")]
    [SerializeField] private int maxCapacity = 20;
    [SerializeField] private Color emptyColor = Color.white;
    [SerializeField] private Color fullColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray

    [Header("References")]
    [SerializeField] private Image trashBinImage; // For UI Image
    [SerializeField] private Renderer trashBinRenderer; // For Sprite Renderer
    [SerializeField] private string materialColorProperty = "_Color";
    [SerializeField] private TextMeshProUGUI statusText; // For "I am full!" message
    [SerializeField] private Text legacyStatusText; // For legacy UI Text (if using old UI system)

    private int currentTrashLevel = 0;
    private Material trashBinMaterial;
    private bool isUIImage = false;

    void Start()
    {
        // Try to find UI Image first
        if (trashBinImage == null)
            trashBinImage = GetComponent<Image>();

        if (trashBinImage != null)
        {
            // It's a UI Image
            isUIImage = true;
            UpdateTrashBinColor();
            Debug.Log("TrashBin: Using UI Image");
        }
        else
        {
            // Try to find Renderer (for regular sprites)
            if (trashBinRenderer == null)
                trashBinRenderer = GetComponent<Renderer>();

            if (trashBinRenderer != null)
            {
                trashBinMaterial = trashBinRenderer.material;
                UpdateTrashBinColor();
                Debug.Log("TrashBin: Using Sprite Renderer");
            }
            else
            {
                Debug.LogError("No Image or Renderer found on " + gameObject.name);
            }
        }

        // Hide status text at start
        UpdateStatusText();
    }

    /// <summary>
    /// Add trash to the bin (called when cleaning cookware)
    /// </summary>
    public bool AddTrash(int amount)
    {
        // Check if trash bin is full
        if (currentTrashLevel >= maxCapacity)
        {
            Debug.Log("I am full!");
            UpdateStatusText(); // Update UI
            return false;
        }

        // Add trash and update color
        currentTrashLevel = Mathf.Min(currentTrashLevel + amount, maxCapacity);
        UpdateTrashBinColor();
        UpdateStatusText(); // Update UI
        Debug.Log($"Trash bin: {currentTrashLevel}/{maxCapacity}");

        // Check if just became full
        if (currentTrashLevel >= maxCapacity)
        {
            Debug.Log("I am full!");
        }

        return true;
    }

    /// <summary>
    /// Empty the trash bin
    /// </summary>
    public void EmptyTrash()
    {
        currentTrashLevel = 0;
        UpdateTrashBinColor();
        UpdateStatusText(); // Update UI
        Debug.Log("Trash bin emptied!");
    }

    /// <summary>
    /// Update trash bin color based on fill level
    /// </summary>
    private void UpdateTrashBinColor()
    {
        // Calculate fill percentage (0 = empty, 1 = full)
        float fillPercentage = (float)currentTrashLevel / maxCapacity;

        // Lerp between empty and full color
        Color targetColor = Color.Lerp(emptyColor, fullColor, fillPercentage);

        // Apply color based on type
        if (isUIImage && trashBinImage != null)
        {
            // For UI Image
            trashBinImage.color = targetColor;
        }
        else if (trashBinMaterial != null)
        {
            // For Sprite Renderer
            trashBinMaterial.SetColor(materialColorProperty, targetColor);
        }
    }

    /// <summary>
    /// Update status text to show "I am full!" or capacity
    /// </summary>
    private void UpdateStatusText()
    {
        string message = "";

        if (currentTrashLevel >= maxCapacity)
        {
            message = "I am full!";
        }
        else if (currentTrashLevel > maxCapacity * 0.75f)
        {
            message = $"Almost full! ({currentTrashLevel}/{maxCapacity})";
        }
        else
        {
            message = ""; // Hide text when not full
        }

        // Update TextMeshPro
        if (statusText != null)
        {
            statusText.text = message;
        }

        // Update legacy Text
        if (legacyStatusText != null)
        {
            legacyStatusText.text = message;
        }
    }

    /// <summary>
    /// Check if trash bin is full
    /// </summary>
    public bool IsFull()
    {
        return currentTrashLevel >= maxCapacity;
    }

    /// <summary>
    /// Get current trash level
    /// </summary>
    public int GetTrashLevel()
    {
        return currentTrashLevel;
    }

    /// <summary>
    /// Get remaining capacity
    /// </summary>
    public int GetRemainingCapacity()
    {
        return maxCapacity - currentTrashLevel;
    }

    // Optional: Visualize trash level in Scene view
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = IsFull() ? Color.red : Color.yellow;
        Vector3 pos = transform.position + Vector3.up * 2f;

#if UNITY_EDITOR
        UnityEditor.Handles.Label(pos, $"Trash: {currentTrashLevel}/{maxCapacity}");
#endif
    }
}