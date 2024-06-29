using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class ResourceCopyExtension
{
    static ResourceCopyExtension()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange newState)
    {
        if (newState != PlayModeStateChange.ExitingEditMode) return;

        // Check the JS resources.
        var baseDir = Path.GetDirectoryName(Application.dataPath);
        if (baseDir == null)
        {
            Debug.LogError("Unable to find project base directory.");
            return;
        }

        var uiBaseDir = Path.Combine(baseDir, "UserInterface");
        var resourceDir = Path.Combine(uiBaseDir, "resources");
        var outDir = Path.Combine(uiBaseDir, "out");

        // Copy resourcesDir into outDir.
        var resourcesOut = Path.Combine(outDir, "resources");
        CopyDirectory(resourceDir, resourcesOut);
    }

    /// <summary>
    /// Recursively copies a directory.
    /// </summary>
    private static void CopyDirectory(string sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var dest = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, dest, true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var dest = Path.Combine(destDir, Path.GetFileName(directory));
            CopyDirectory(directory, dest);
        }
    }
}
