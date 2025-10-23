// Assets/Editor/WebGLBuildSettings.cs
using UnityEditor;
using UnityEngine;

public static class WebGLBuildSettings
{
    public static void DisableCompression()
    {
        Debug.Log("🟢 [WebGLBuildSettings] Running DisableCompression()...");

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = false;

        Debug.Log("✅ [WebGLBuildSettings] WebGL compression disabled successfully!");
    }
}
