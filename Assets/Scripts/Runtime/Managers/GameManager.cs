using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Player;
using Mirror;
using OneJS;
using Runtime;
using Runtime.World;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        private readonly List<NetworkConnectionToClient> _loadedPlayers = new();

        private TaskCompletionSource<object> _loadTask;

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

            // Register packet handlers.
            NetworkServer.RegisterHandler<EnterSceneDoneC2SNotify>(OnEnterSceneDone);
            NetworkServer.RegisterHandler<DoWorldGenC2SRsp>(OnWorldGenDone);

            NetworkClient.RegisterHandler<TransferSceneS2CNotify>(OnTransferScene);
            NetworkClient.RegisterHandler<DoWorldGenS2CReq>(OnWorldGenReq);
            NetworkClient.RegisterHandler<PlayerLoginSuccessS2CNotify>(OnLoginSuccess);
            NetworkClient.RegisterHandler<GameStartS2CNotify>(OnNetworkGameStart);
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
        public async void StartGame()
        {
            if (!NetworkServer.active) return;
            if (!NetworkServer.activeHost)
            {
                Debug.LogWarning("Client called StartGame method. Ignoring.");
                return;
            }

            // Close Steam lobby.
            LobbyManager.Instance.CloseLobby();

            // Transfer all players to game scene.
            _loadedPlayers.Clear();
            _loadTask = new TaskCompletionSource<object>();

            NetworkServer.SendToAll(new TransferSceneS2CNotify { sceneId = Scenes.Game });

            // Wait for players to finish loading.
            Debug.Log("Waiting for players to load scene...");
            await _loadTask.Task;

            // Spawn all player prefabs.
            foreach (var player in _loadedPlayers)
            {
                var playerObject = Instantiate(_networkManager.playerPrefab);
                var playerController = playerObject.GetComponent<PlayerController>();
                // TODO: Replace later with the actual player role selected from the lobby.
                playerController.playerRole = player.address == "localhost" ? PlayerRole.Buddie : PlayerRole.Fwend;
                NetworkServer.AddPlayerForConnection(player, playerObject);
            }

            // Generate the world.
            _loadedPlayers.Clear();
            _loadTask = new TaskCompletionSource<object>();

            NetworkServer.SendToAll(new DoWorldGenS2CReq { seed = (int)DateTime.Now.Ticks });

            // Wait for players to finish generating the world.
            Debug.Log("Waiting for players to generate world...");
            await _loadTask.Task;

            // Dismiss loading screen.
            // TODO: Implement loading screen.

            // Invoke the game start event.
            Debug.Log("Finished! Starting game...");
            NetworkServer.SendToAll(new GameStartS2CNotify());
        }

        #region Packet Handlers

        /// <summary>
        /// Invoked when the server is notified that the client has entered the scene.
        /// </summary>
        private static void OnEnterSceneDone(
            NetworkConnectionToClient conn,
            EnterSceneDoneC2SNotify notify)
        {
            var instance = Instance;

            instance._loadedPlayers.Add(conn);
            Debug.Log($"Player {conn.address} finished loading scene.");

            // Check if all players have finished loading.
            if (instance._loadedPlayers.Count == NetworkServer.connections.Count)
            {
                instance._loadTask.SetResult(null);
            }
        }

        /// <summary>
        /// Invoked when the server is notified that the client has finished generating the world.
        /// </summary>
        private static void OnWorldGenDone(
            NetworkConnectionToClient conn,
            DoWorldGenC2SRsp notify)
        {
            var instance = Instance;

            instance._loadedPlayers.Add(conn);
            Debug.Log($"Player {conn.address} finished generating world.");

            // Check if all players have finished generating the world.
            if (instance._loadedPlayers.Count == NetworkServer.connections.Count)
            {
                instance._loadTask.SetResult(null);
            }
        }

        /// <summary>
        /// Invoked when the client is requested to transfer scenes.
        /// </summary>
        private static void OnTransferScene(TransferSceneS2CNotify notify)
        {
            var operation = SceneManager.LoadSceneAsync(notify.sceneId);
            operation.completed += _ => NetworkClient.Send(new EnterSceneDoneC2SNotify());
            // TODO: Display scene loading screen.
        }

        /// <summary>
        /// Invoked when the server requests the client to generate the world.
        /// </summary>
        private static void OnWorldGenReq(DoWorldGenS2CReq req)
        {
            var world = WaveManager.Instance.World;
            world.seed = req.seed;
            world.Generate();

            NetworkClient.Send(new DoWorldGenC2SRsp());
        }

        /// <summary>
        /// Invoked when the server notifies the client that the login was successful.
        /// </summary>
        private static void OnLoginSuccess(PlayerLoginSuccessS2CNotify notify)
        {
            Instance.State = GameState.Lobby;
            Debug.Log("Client finished connecting to the server.");
        }

        /// <summary>
        /// Invoked when the server notifies the client that the game has started.
        /// </summary>
        private static void OnNetworkGameStart(GameStartS2CNotify notify)
            => OnGameStart?.Invoke();

        #endregion
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
        Lobby,
        Loading,
        Playing
    }
}
