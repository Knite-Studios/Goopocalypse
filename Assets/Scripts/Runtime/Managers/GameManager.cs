using System;
using Mirror;
using OneJS;
using Runtime.World;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public partial class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnGameStart;
        public static UnityAction<GameEvent> OnGameEvent;
        public static UnityAction<World> OnWorldGenerated;
        public static UnityAction<int> OnWaveSpawn;

        /// <summary>
        /// Reference to the JavaScript ScriptEngine.
        /// </summary>
        public static ScriptEngine ScriptEngine => FindObjectOfType<ScriptEngine>();

        private NetworkManager _networkManager;

        [EventfulProperty] private GameState _state = GameState.Menu;

        protected override void OnAwake()
        {
            // Initialize other managers.
            LobbyManager.Initialize();
            InputManager.Initialize();
            ScriptManager.Initialize();
            WaveManager.Initialize();
            PrefabManager.Initialize();
            EntityManager.Initialize();

            // Find references.
            _networkManager = FindObjectOfType<NetworkManager>();
        }

        private void Start()
        {
            // Add event listener for game events.
            OnGameStart += () => State = GameState.Playing;
        }

        /// <summary>
        /// Starts the KCP debugging server.
        /// </summary>
        public void StartDebugServer() => _networkManager.StartHost();

        /// <summary>
        /// Connects to the debugging server.
        /// </summary>
        /// <param name="address">The server's address.</param>
        /// <param name="port">The server's port.</param>
        public void JoinDebugServer(string address, ushort port)
        {
            _networkManager.networkAddress = address;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = port;
            }

            _networkManager.StartClient();
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
