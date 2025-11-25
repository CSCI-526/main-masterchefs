using UnityEngine;
using UnityEngine.UI;

public class CookwareFireController : MonoBehaviour
{
    [System.Serializable]
    public class CookwareStation
    {
        public string name;               
        public Button button;             
        public ParticleSystem fireEffect; 
        public BaseCookware cookware;     
        public bool isOn = false;         
    }

    [Header("Cookware Stations")]
    public CookwareStation[] stations;

    void Start()
    {

        // initialize each station
        for (int i = 0; i < stations.Length; i++)
        {
            CookwareStation station = stations[i];

            if (station.button == null)
            {
                Debug.LogError("Station '" + station.name + "': Button is NULL");
                continue;
            }
            else
            {
                Debug.Log("Button assigned: " + station.button.gameObject.name);
            }

            if (station.fireEffect == null)
            {
                Debug.LogError("Station '" + station.name + "': Fire Effect NULL");
                continue;
            }
            else
            {
                Debug.Log("Fire Effect assigned: " + station.fireEffect.gameObject.name);
                Debug.Log("Fire Effect active: " + station.fireEffect.gameObject.activeInHierarchy);
                Debug.Log("Particle max count: " + station.fireEffect.main.maxParticles);
            }

            if (station.cookware == null)
            {
                Debug.LogWarning("Station '" + station.name + "': Cookware reference NULL");
            }

            // fire starts off
            station.fireEffect.Stop();
            station.isOn = false;

            CookwareStation currentStation = station;
            station.button.onClick.AddListener(() => ToggleFire(currentStation));
        }
    }

    // turn stove burner on or off
    void ToggleFire(CookwareStation station)
    {
        Debug.Log("\nToggleFire called for: " + station.name);
        Debug.Log("Current state - isOn: " + station.isOn);

        station.isOn = !station.isOn;

        Debug.Log("New state - isOn: " + station.isOn);

        // fire ON
        if (station.isOn)
        {
            if (station.fireEffect == null)
            {
                Debug.LogError("Fire effect NULL");
                return;
            }

            if (!station.fireEffect.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Fire effect inactive");
                station.fireEffect.gameObject.SetActive(true);
            }

            station.fireEffect.Play();
            Debug.Log(station.name + " burner turned ON");

            // notify cookware
            if (station.cookware != null)
                station.cookware.SetFire(true);
        }
        else
        {
            // fire OFF
            station.fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Debug.Log(station.name + " burner turned OFF");

            // notify cookware
            if (station.cookware != null)
                station.cookware.SetFire(false);
        }
    }
}
