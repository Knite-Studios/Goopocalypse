﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Common;
using Entity;
using Entity.Player;
using Mirror;
using OneJS;
using Runtime;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public partial class GameManager : MonoSingleton<GameManager>
    {
        [NaughtyAttributes.Scene] public int gameScene;

        public static Action OnGameStart;
        public static UnityAction<GameEvent> OnGameEvent;
        public static UnityAction<int> OnWaveSpawn;

        /// <summary>
        /// Reference to the JavaScript ScriptEngine.
        /// </summary>
        public static ScriptEngine ScriptEngine => FindObjectOfType<ScriptEngine>();

        private NetworkManager _networkManager;
        private readonly List<NetworkConnectionToClient> _loadedPlayers = new();

        private TaskCompletionSource<object> _loadTask;

        #region JavaScript Accessible

        [EventfulProperty] private Texture2D _profilePicture;
        [EventfulProperty] private string _username;

        [EventfulProperty] private GameState _state = GameState.Menu;

        public event Action<string> OnRouteUpdate;

        #endregion

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

            NetworkClient.RegisterHandler<TransferSceneS2CNotify>(OnTransferScene);
            NetworkClient.RegisterHandler<PlayerLoginSuccessS2CNotify>(OnLoginSuccess);
            NetworkClient.RegisterHandler<GameStartS2CNotify>(OnNetworkGameStart);
        }

        private void Start()
        {
            // Add event listener for game events.
            OnGameStart += () => State = GameState.Playing;

            // Check if Steam is active.
            if (SteamAPI.IsSteamRunning())
            {
                // Load the user's profile picture.
                ProfilePicture = SteamUtilities.LoadLocalAvatar();
                Username = SteamFriends.GetPersonaName();
            }
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

            // TODO: Temporary for testing ONLY.
            NetworkServer.SendToAll(new TransferSceneS2CNotify { sceneId = gameScene });

            // Wait for players to finish loading.
            Debug.Log("Waiting for players to load scene...");
            await _loadTask.Task;

            var playerControllers = new List<PlayerController>();

            // Spawn all player prefabs.
            foreach (var player in _loadedPlayers)
            {
                var playerObject = Instantiate(_networkManager.playerPrefab);
                var playerController = playerObject.GetComponent<PlayerController>();
                playerController.playerRole = LobbyManager.Instance.GetPlayerRole(player);
                NetworkServer.AddPlayerForConnection(player, playerObject);
                playerControllers.Add(playerController);
                EntityManager.RegisterPlayer(playerController);
            }

            // Connect the players with the link.
            if (playerControllers.Count == 2)
            {
                var link = PrefabManager.Create<Link>(PrefabType.Link);
                link.fwend = playerControllers.First(player => player.playerRole == PlayerRole.Fwend).transform;
                link.buddie = playerControllers.First(player => player.playerRole == PlayerRole.Buddie).transform;
                NetworkServer.Spawn(link.gameObject);
            }

            // Dismiss loading screen.
            // TODO: Implement loading screen.

            // Invoke the game start event.
            Debug.Log("Finished! Starting game...");
            NetworkServer.SendToAll(new GameStartS2CNotify());
        }

        /// <summary>
        /// Method for JavaScript use.
        /// Starts the game by invoking the event.
        /// </summary>
        public async void StartLocalGame()
        {
            // Transfer to the game scene.
            var task = new TaskCompletionSource<object>();
            var operation = SceneManager.LoadSceneAsync(gameScene);
            if (operation == null)
            {
                throw new Exception("Failed to load scene.");
            }
            operation.completed += _ => task.SetResult(null);

            // Wait for the scene to load.
            await task.Task;

            // Spawn local players.
            var player1 = CreatePlayer(PlayerRole.Fwend);
            var player2 = CreatePlayer(PlayerRole.Buddie);

            // Configure the second player to use the second input.
            player2.input = InputManager.Movement2;
            // Disable the second player's camera.
            // player2.DisableCamera();

            // Create link between players.
            LinkPlayers(player1, player2, false);

            // Set the local multiplayer flag.
            LocalMultiplayer = true;

            // Set the target group of the camera to both players.
            CreateTargetGroup(player1.transform, player2.transform);

            // Invoke the game start event.
            Debug.Log("Finished! Starting game...");
            OnGameStart?.Invoke();
        }

        /// <summary>
        /// Requests to change the role on the server.
        /// </summary>
        public void ChangeRole(PlayerRole role) =>
            NetworkClient.Send(new ChangeRoleC2SReq { role = role });

        /// <summary>
        /// Creates a Cinemachine Target Group for the players.
        /// </summary>
        private void CreateTargetGroup(Transform player1, Transform player2)
        {
            // Create a new target group.
            var targetGroupGameObject = new GameObject("TargetGroup");
            var targetGroup = targetGroupGameObject.AddComponent<CinemachineTargetGroup>();

            // Add all players to the target group.
            targetGroup.m_Targets = new[]
            {
                new CinemachineTargetGroup.Target { target = player1, weight = 1, radius = 1 },
                new CinemachineTargetGroup.Target { target = player2, weight = 1, radius = 1 }
            };
        }

        /// <summary>
        /// Navigate the user interface to a specific path.
        /// </summary>
        public void Navigate(string path) => OnRouteUpdate?.Invoke(path);

        /// <summary>
        /// Global method to quit the game.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
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
        /// Invoked when the client is requested to transfer scenes.
        /// </summary>
        private static void OnTransferScene(TransferSceneS2CNotify notify)
        {
            var operation = SceneManager.LoadSceneAsync(notify.sceneId);
            if (operation == null)
            {
                throw new Exception("Failed to load scene.");
            }

            operation.completed += _ => NetworkClient.Send(new EnterSceneDoneC2SNotify());
            // TODO: Display scene loading screen.
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
