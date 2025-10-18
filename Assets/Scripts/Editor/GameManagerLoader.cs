using System;
#if UNITY_EDITOR
using Managers;
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// Static class that loads the game manager into the scene.
    /// </summary>
    [InitializeOnLoad]
    public static class GameManagerLoader
    {
        /// <summary>
        /// Static constructor which is called when the class is loaded.
        /// </summary>
        static GameManagerLoader()
        {
            EditorApplication.playModeStateChanged -= InitializeGameManager;
            EditorApplication.playModeStateChanged += InitializeGameManager;
        }

        /// <summary>
        /// Invoked when the play mode state is changed in the editor.
        /// </summary>
        private static void InitializeGameManager(PlayModeStateChange evt)
        {
            if (evt != PlayModeStateChange.EnteredPlayMode) return;

            // Check if a GameManager already exists in the scene.
            if (Object.FindObjectOfType<GameManager>() != null) return;

            // ScriptEngine has been removed - using XLua for scripting instead
            Debug.Log("ScriptEngine removed - using XLua for scripting");

            // Add the game manager to the scene.
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/GameManager");
            if (prefab == null) throw new Exception("Missing GameManager prefab!");

            var instance = Object.Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate GameManager prefab!");

            instance.name = "Managers.GameManager (Singleton)";

            // Optionally initialize other managers that should be present at startup
            InitializeAdditionalManagers();
        }

        /// <summary>
        /// Initializes other essential managers that should be present when entering play mode.
        /// </summary>
        private static void InitializeAdditionalManagers()
        {
            // Initialize SettingsManager if not present
            if (Object.FindObjectOfType<SettingsManager>() == null)
            {
                try
                {
                    SettingsManager.Initialize();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to initialize SettingsManager: {ex.Message}");
                }
            }

            // Initialize AudioManager if not present
            if (Object.FindObjectOfType<AudioManager>() == null)
            {
                try
                {
                    AudioManager.Initialize();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to initialize AudioManager: {ex.Message}");
                }
            }

            // Initialize ScriptManager if not present (for XLua)
            if (Object.FindObjectOfType<ScriptManager>() == null)
            {
                try
                {
                    // Create ScriptManager manually since it doesn't have an Initialize method
                    var scriptManagerPrefab = Resources.Load<GameObject>("Prefabs/Managers/ScriptManager");
                    if (scriptManagerPrefab != null)
                    {
                        var instance = Object.Instantiate(scriptManagerPrefab);
                        instance.name = "Managers.ScriptManager (Singleton)";
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to initialize ScriptManager: {ex.Message}");
                }
            }
        }
    }
#endif
}
