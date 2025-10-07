using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // This function will be linked to the button
    public void QuitGame()
    {
        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build, quit the application
        Application.Quit();
#endif
    }
}
