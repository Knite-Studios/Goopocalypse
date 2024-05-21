using System;
using Mirror;
using OneJS;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public partial class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnGameStart;
        public static UnityAction<GameEvent> OnGameEvent;

        /// <summary>
        /// Reference to the JavaScript ScriptEngine.
        /// </summary>
        public static ScriptEngine ScriptEngine => Instance.scriptEngine;

        public ScriptEngine scriptEngine;
        public NetworkManager networkManager;

        private GameState _state = GameState.Menu;

        protected override void OnAwake()
        {
            // Find references.
            scriptEngine = FindObjectOfType<ScriptEngine>();
            networkManager = FindObjectOfType<NetworkManager>();

            // Initialize other managers.
            InputManager.Initialize();
            ScriptManager.Initialize();
            WaveManager.Initialize();
            PrefabManager.Initialize();
            EntityManager.Initialize();
        }

        private void Start()
        {
            // Add event listener for game events.
            OnGameStart += () => State = GameState.Playing;
        }

        /// <summary>
        /// Starts the KCP debugging server.
        /// </summary>
        public void StartDebugServer() => networkManager.StartHost();

        /// <summary>
        /// Connects to the debugging server.
        /// </summary>
        /// <param name="address">The server's address.</param>
        /// <param name="port">The server's port.</param>
        public void JoinDebugServer(string address, ushort port)
        {
            networkManager.networkAddress = address;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = port;
            }

            networkManager.StartClient();
        }

        /// <summary>
        /// Method for JavaScript use.
        /// Starts the game by invoking the event.
        /// </summary>
        public void StartGame() => OnGameStart.Invoke();
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

    public enum GameState
    {
        Menu,
        Playing
    }
}
