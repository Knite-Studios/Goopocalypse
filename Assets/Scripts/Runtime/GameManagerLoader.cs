using System;
using Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime
{
#if !UNITY_EDITOR
    public static class GameManagerLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeGameManager()
        {
            // Check if a GameManager already exists in the scene.
            if (Object.FindObjectOfType<GameManager>() != null) return;

            // Add the script engine to the scene.
            var prefab = Resources.Load<GameObject>("Prefabs/ScriptEngine");
            if (prefab == null) throw new Exception("Missing ScriptEngine prefab!");

            var instance = Object.Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate ScriptEngine prefab!");

            instance.name = "ScriptEngine (Singleton)";

            // Add the game manager to the scene.
            prefab = Resources.Load<GameObject>("Prefabs/Managers/GameManager");
            if (prefab == null) throw new Exception("Missing GameManager prefab!");

            instance = Object.Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate GameManager prefab!");

            instance.name = "Managers.GameManager (Singleton)";
        }
    }
#endif
}
