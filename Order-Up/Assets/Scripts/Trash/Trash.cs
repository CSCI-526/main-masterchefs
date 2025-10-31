using UnityEngine;
using System.Collections.Generic;


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
        if (plate != null)
        {
            plate.ClearPlate();
        }
    }
}
