using UnityEngine;
using UnityEngine.UI;

public class CookwareFireController : MonoBehaviour
{
    [System.Serializable]
    public class CookwareStation
    {
        public string name;              // Name of the cookware
        public Button button;            // The button to click
        public ParticleSystem fireEffect; // The fire particle system under this cookware
        public bool isOn = false;        // Tracks if this burner is on
    }

    [Header("Cookware Stations")]
    public CookwareStation[] stations;

    void Start()
    {
        // Initialize each station - think of this as setting up each burner control
        foreach (CookwareStation station in stations)
        {
            if (station.button != null && station.fireEffect != null)
            {
                // Make sure fire is off at start
                station.fireEffect.Stop();
                station.isOn = false;

                // Add click listener - like wiring the button to the gas valve
                CookwareStation currentStation = station; // Capture for closure
                station.button.onClick.AddListener(() => ToggleFire(currentStation));
            }
        }
    }

    // This is like turning the stove knob - it switches between on and off
    void ToggleFire(CookwareStation station)
    {
        station.isOn = !station.isOn;

        if (station.isOn)
        {
            // Turn on the fire - like igniting the gas burner
            station.fireEffect.Play();
            Debug.Log($"{station.name} burner turned ON");
        }
        else
        {
            // Turn off the fire instantly - like cutting the gas AND blowing out the flames
            station.fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Debug.Log($"{station.name} burner turned OFF");
        }
    }
}









