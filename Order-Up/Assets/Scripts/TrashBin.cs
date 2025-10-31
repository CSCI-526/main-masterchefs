//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class TrashBin : MonoBehaviour
//{
//    [Header("Trash Bin Settings")]
//    [SerializeField] private Color emptyColor = Color.white;
//    [SerializeField] private Color fullColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray

//    [Header("References")]
//    [SerializeField] private Image trashBinImage;
//    [SerializeField] private Renderer trashBinRenderer;
//    [SerializeField] private string materialColorProperty = "_Color";

//    private int currentTrashLevel = 0;
//    private Material trashBinMaterial;
//    private bool isUIImage = false;

//    void Start()
//    {
//        // Try to find UI Image first
//        if (trashBinImage == null)
//            trashBinImage = GetComponent<Image>();

//        if (trashBinImage != null)
//        {
//            // It's a UI Image
//            isUIImage = true;
//            UpdateTrashBinColor();
//            Debug.Log("TrashBin: Using UI Image");
//        }
//        else
//        {
//            // Try to find Renderer (for regular sprites)
//            if (trashBinRenderer == null)
//                trashBinRenderer = GetComponent<Renderer>();

//            if (trashBinRenderer != null)
//            {
//                trashBinMaterial = trashBinRenderer.material;
//                UpdateTrashBinColor();
//                Debug.Log("TrashBin: Using Sprite Renderer");
//            }
//            else
//            {
//                Debug.LogError("No Image or Renderer found on " + gameObject.name);
//            }
//        }
//    }

//    void Update()
//    {
//        // Press C to reset trash bin color to original
//        if (Input.GetKeyDown(KeyCode.C))
//        {
//            ResetColor();
//        }
//    }

//    /// Add trash to the bin (called when cleaning cookware)
//    /// Unlimited capacity
//    public bool AddTrash(int amount)
//    {
//        // Add trash and update color (unlimited capacity)
//        currentTrashLevel += amount;
//        UpdateTrashBinColor();
//        Debug.Log($"Trash bin: {currentTrashLevel} items");

//        return true;
//    }


//    /// Empty the trash bin
//    public void EmptyTrash()
//    {
//        currentTrashLevel = 0;
//        UpdateTrashBinColor();
//        Debug.Log("Trash bin emptied!");
//    }

//    /// Reset the trash bin color back to original (empty color)
//    public void ResetColor()
//    {
//        currentTrashLevel = 0;
//        UpdateTrashBinColor();
//        Debug.Log("Trash bin color reset to original!");
//    }

//    /// Update trash bin color based on fill level
//    /// Uses a logarithmic scale so color changes more gradually
//    private void UpdateTrashBinColor()
//    {
//        // Use logarithmic scale for unlimited capacity
//        // This prevents the color from getting too dark too quickly
//        float fillPercentage = Mathf.Clamp01(Mathf.Log(currentTrashLevel + 1) / 5f);

//        // Lerp between empty and full color
//        Color targetColor = Color.Lerp(emptyColor, fullColor, fillPercentage);

//        // Apply color based on type
//        if (isUIImage && trashBinImage != null)
//        {
//            // For UI Image
//            trashBinImage.color = targetColor;
//        }
//        else if (trashBinMaterial != null)
//        {
//            // For Sprite Renderer
//            trashBinMaterial.SetColor(materialColorProperty, targetColor);
//        }
//    }

//    /// Check if trash bin is full (always false for unlimited capacity)
//    public bool IsFull()
//    {
//        return false;
//    }

//    /// Get current trash level
//    public int GetTrashLevel()
//    {
//        return currentTrashLevel;
//    }

//    /// Get remaining capacity (unlimited)
//    public int GetRemainingCapacity()
//    {
//        return int.MaxValue;
//    }

//    void OnDrawGizmos()
//    {
//        if (!Application.isPlaying) return;

//        Gizmos.color = Color.yellow;
//        Vector3 pos = transform.position + Vector3.up * 2f;
//    }
//}


















using UnityEngine;
using UnityEngine.UI;

public class TrashBin : MonoBehaviour
{
    [Header("Trash Bin Settings")]
    [SerializeField] private Color emptyColor = Color.white;
    [SerializeField] private Color fullColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray

    [Header("References")]
    [SerializeField] private Image trashBinImage;
    [SerializeField] private Renderer trashBinRenderer;
    [SerializeField] private string materialColorProperty = "_Color";

    private int currentTrashLevel = 0;
    private Material trashBinMaterial;
    private bool isUIImage = false;

    void Start()
    {
        if (trashBinImage == null) trashBinImage = GetComponent<Image>();
        if (trashBinImage != null)
        {
            isUIImage = true;
            UpdateTrashBinColor();
        }
        else
        {
            if (trashBinRenderer == null) trashBinRenderer = GetComponent<Renderer>();
            if (trashBinRenderer != null)
            {
                trashBinMaterial = trashBinRenderer.material;
                UpdateTrashBinColor();
            }
            else
            {
                Debug.LogError("TrashBin: No Image or Renderer found on " + name);
            }
        }
    }

    // Optional hotkey to empty trash manually
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) EmptyTrash();
    }

    /// Called by CookwareMaintenance when cleaning
    public bool AddTrash(int amount)
    {
        currentTrashLevel += Mathf.Max(1, amount);
        UpdateTrashBinColor();
        return true;
    }

    public void EmptyTrash()
    {
        currentTrashLevel = 0;
        UpdateTrashBinColor();
    }

    private void UpdateTrashBinColor()
    {
        // Log-scale so color darkens smoothly with unlimited capacity
        float fill = Mathf.Clamp01(Mathf.Log(currentTrashLevel + 1) / 5f);
        Color target = Color.Lerp(emptyColor, fullColor, fill);

        if (isUIImage && trashBinImage != null) trashBinImage.color = target;
        else if (trashBinMaterial != null)
        {
            if (trashBinMaterial.HasProperty(materialColorProperty))
                trashBinMaterial.SetColor(materialColorProperty, target);
            else if (trashBinMaterial.HasProperty("_BaseColor"))
                trashBinMaterial.SetColor("_BaseColor", target);
        }
    }

    public int GetTrashLevel() => currentTrashLevel;
    public bool IsFull() => false;
    public int GetRemainingCapacity() => int.MaxValue;
}
