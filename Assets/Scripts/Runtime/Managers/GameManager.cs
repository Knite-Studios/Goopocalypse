using System;
using OneJS;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnGameStart;
        public static UnityAction<GameEvent> OnGameEvent;

        /// <summary>
        /// Reference to the JavaScript ScriptEngine.
        /// </summary>
        public static ScriptEngine ScriptEngine => Instance.scriptEngine;

        public ScriptEngine scriptEngine;

        protected override void OnAwake()
        {
            // Set JavaScript engine instance.
            scriptEngine = FindObjectOfType<ScriptEngine>();

            // Initialize other managers.
            InputManager.Initialize();
            ScriptManager.Initialize();
            WaveManager.Initialize();
            PrefabManager.Initialize();
        }
    }

    public struct GameEvent
    {
        public GameEventType Type;

        public Transform Target;
    }

    public enum GameEventType
    {
        ChestSpawned,
        EnemyKilled
    }
}
