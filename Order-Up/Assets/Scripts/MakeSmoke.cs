using UnityEngine;

public class MakeSmoke : MonoBehaviour
{

    // Make it public so you can drag & drop
    public ParticleSystem steamParticles; 
    void Start()
    {
        if (steamParticles != null)
        {
            // Start with no steam
            steamParticles.Stop();
        }
    }

    void Update()
    {
        // Press Space to toggle smoke on/off
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (steamParticles.isPlaying)
                steamParticles.Stop();
            else
                steamParticles.Play();
        }
    }

    // Call this when water starts boiling
    public void StartSteam()
    {
        if (steamParticles != null && !steamParticles.isPlaying)
            steamParticles.Play();
    }

    // Call this when pot is removed from heat
    public void StopSteam()
    {
        if (steamParticles != null)
            steamParticles.Stop();
    }
}