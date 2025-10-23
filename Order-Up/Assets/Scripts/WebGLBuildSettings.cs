// Assets/Editor/WebGLBuildSettings.cs
using UnityEngine;
using UnityEditor;

public static class WebGLBuildSettings
{
    public static void DisableCompression()
    {
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = false;
        Debug.Log("✅ WebGL compression disabled for CI build");
    }
}
