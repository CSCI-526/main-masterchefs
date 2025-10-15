using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     Call clearplate method from Plate script to clear all ingredients on plate
/// </summary>
public class Trash : MonoBehaviour
{
    [SerializeField] private Plate plate;
    
    void Start()
    {
        if (plate == null)
        {
            Debug.LogError("Plate reference is not assigned in Trash script.");
        }
    }

    public void ClearPlate()
    {
        // Call the ClearPlate method from the Plate script
        if (plate != null)
        {
            plate.ClearPlate();
        }
    }
}
