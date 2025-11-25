//using UnityEngine;
//using UnityEngine.UI;

//public class CookwareFireController : MonoBehaviour
//{
//    [System.Serializable]
//    public class CookwareStation
//    {
//        public string name;              // Name of the cookware
//        public Button button;            // The button to click
//        public ParticleSystem fireEffect; // The fire particle system under this cookware
//        public bool isOn = false;        // Tracks if this burner is on
//    }

//    [Header("Cookware Stations")]
//    public CookwareStation[] stations;

//    void Start()
//    {
//        // Initialize each station - think of this as setting up each burner control
//        foreach (CookwareStation station in stations)
//        {
//            if (station.button != null && station.fireEffect != null)
//            {
//                // Make sure fire is off at start
//                station.fireEffect.Stop();
//                station.isOn = false;

//                // Add click listener - like wiring the button to the gas valve
//                CookwareStation currentStation = station; // Capture for closure
//                station.button.onClick.AddListener(() => ToggleFire(currentStation));
//            }
//        }
//    }

//    // This is like turning the stove knob - it switches between on and off
//    void ToggleFire(CookwareStation station)
//    {
//        station.isOn = !station.isOn;

//        if (station.isOn)
//        {
//            // Turn on the fire - like igniting the gas burner
//            station.fireEffect.Play();
//            Debug.Log($"{station.name} burner turned ON");
//        }
//        else
//        {
//            // Turn off the fire instantly - like cutting the gas AND blowing out the flames
//            station.fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
//            Debug.Log($"{station.name} burner turned OFF");
//        }
//    }
//}









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
        Debug.Log("=== CookwareFireController Start ===");
        Debug.Log($"Total stations configured: {stations.Length}");

        // Initialize each station - think of this as setting up each burner control
        for (int i = 0; i < stations.Length; i++)
        {
            CookwareStation station = stations[i];

            // Detailed validation logging
            Debug.Log($"\n--- Station {i}: {station.name} ---");

            if (station.button == null)
            {
                Debug.LogError($"❌ Station '{station.name}': Button is NULL! Assign it in Inspector.");
                continue;
            }
            else
            {
                Debug.Log($"✓ Button assigned: {station.button.gameObject.name}");
            }

            if (station.fireEffect == null)
            {
                Debug.LogError($"❌ Station '{station.name}': Fire Effect is NULL! Assign it in Inspector.");
                continue;
            }
            else
            {
                Debug.Log($"✓ Fire Effect assigned: {station.fireEffect.gameObject.name}");
                Debug.Log($"  - Fire Effect active: {station.fireEffect.gameObject.activeInHierarchy}");
                Debug.Log($"  - Particle count: {station.fireEffect.main.maxParticles}");
            }

            // Make sure fire is off at start
            station.fireEffect.Stop();
            station.isOn = false;

            // Add click listener - like wiring the button to the gas valve
            CookwareStation currentStation = station; // Capture for closure
            station.button.onClick.AddListener(() => ToggleFire(currentStation));

            Debug.Log($"✓ Click listener added successfully");
        }

        Debug.Log("\n=== Initialization Complete ===\n");
    }

    // This is like turning the stove knob - it switches between on and off
    void ToggleFire(CookwareStation station)
    {
        Debug.Log($"\n🔥 ToggleFire called for: {station.name}");
        Debug.Log($"Current state - isOn: {station.isOn}");

        station.isOn = !station.isOn;

        Debug.Log($"New state - isOn: {station.isOn}");

        if (station.isOn)
        {
            // Additional checks before playing
            if (station.fireEffect == null)
            {
                Debug.LogError($"❌ Fire effect is NULL when trying to play!");
                return;
            }

            if (!station.fireEffect.gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"⚠️ Fire effect GameObject is INACTIVE! Activating it now...");
                station.fireEffect.gameObject.SetActive(true);
            }

            // Turn on the fire - like igniting the gas burner
            station.fireEffect.Play();
            Debug.Log($"✓ {station.name} burner turned ON - Play() called");
            Debug.Log($"  - IsPlaying: {station.fireEffect.isPlaying}");
            Debug.Log($"  - IsEmitting: {station.fireEffect.isEmitting}");
        }
        else
        {
            // Turn off the fire instantly - like cutting the gas AND blowing out the flames
            station.fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Debug.Log($"✓ {station.name} burner turned OFF");
        }
    }

    // Optional: Test method you can call from Inspector or another script
    [ContextMenu("Test Pot Fire")]
    void TestPotFire()
    {
        if (stations.Length > 0)
        {
            Debug.Log("🧪 Testing Pot station manually...");
            ToggleFire(stations[0]);
        }
    }
}