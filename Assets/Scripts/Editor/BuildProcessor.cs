using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string LuaResourcesPath = "Resources/Lua";
        private const string LuaStreamingAssetsPath = "StreamingAssets/Lua";
        private const string LuaTempPath = "Temp/Lua";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log($"BuildPreProcess: {report.summary.platform} build starting...");

            var streamingAssetsPath = Path.Combine(Application.dataPath, LuaStreamingAssetsPath);
            var resourcesPath = Path.Combine(Application.dataPath, LuaResourcesPath);

            // Ensure Resources/Lua directory exists.
            if (!Directory.Exists(resourcesPath))
                Directory.CreateDirectory(resourcesPath);

            CopyAllLuaFiles(streamingAssetsPath, resourcesPath);

            Debug.Log("BuildPreProcess: Lua files copied to Resources/Lua");

            BackupLuaFiles(streamingAssetsPath);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log($"BuildPreProcess: {report.summary.platform} build starting...");

            var streamingAssetsPath = Path.Combine(Application.dataPath, LuaStreamingAssetsPath);
            var resourcesPath = Path.Combine(Application.dataPath, LuaResourcesPath);

            // Delete the Resources/Lua directory.
            if (Directory.Exists(resourcesPath))
                Directory.Delete(resourcesPath, true);

            Debug.Log("BuildPreProcess: Lua files deleted from Resources/Lua");

            RestoreLuaFiles(streamingAssetsPath);
        }

        private void CopyAllLuaFiles(string sourceDir, string destDir, string targetExtension = ".txt")
        {
            // Create all directories.
            foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));

            // Copy all .lua files and change the extension to .txt.
            foreach (var filePath in Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories))
            {
                var destFile = filePath.Replace(sourceDir, destDir) + targetExtension;
                File.Copy(filePath, destFile, true);
            }
        }

        private void CopyAllFiles(string sourceDir, string destDir)
        {
            foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));

            foreach (var filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                var destFilePath = filePath.Replace(sourceDir, destDir);
                Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                File.Copy(filePath, destFilePath, true);
            }
        }

        private void BackupLuaFiles(string streamingAssetPath)
        {
            var tempPath = Path.Combine(Application.dataPath, LuaTempPath);

            // Ensure Temp/Lua directory exists.
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);

            Directory.CreateDirectory(tempPath);

            // Copy all files and directories to the temp directory.
            CopyAllFiles(streamingAssetPath, tempPath);

            // Delete the streaming assets directory.
            Directory.Delete(streamingAssetPath, true);

            Debug.Log("BuildPreProcess: Lua files backed up to Temp/Lua");
        }

        private void RestoreLuaFiles(string streamingAssetsPath)
        {
            var tempLuaPath = Path.Combine(Application.dataPath, LuaTempPath);

            // Copy all files and directories to the streaming assets directory.
            CopyAllFiles(tempLuaPath, streamingAssetsPath);

            // Delete the temp directory.
            if (Directory.Exists(tempLuaPath))
                Directory.Delete(tempLuaPath, true);

            var tempPath = Path.Combine(Application.dataPath, "Temp");
            Directory.Delete(tempPath, true);

            Debug.Log("BuildPreProcess: Lua files restored from Temp/Lua");
        }
    }
}
