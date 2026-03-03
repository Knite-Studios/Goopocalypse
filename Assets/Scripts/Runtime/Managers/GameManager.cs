using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Common;
using Discord;
using Entity;
using Entity.Player;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public partial class GameManager : MonoSingleton<GameManager>
    {
        [NaughtyAttributes.Scene] public int menuScene;
        [NaughtyAttributes.Scene] public int gameScene;

        public static Action OnGameStart;
        public static UnityAction<GameEvent> OnGameEvent;
        public static UnityAction<int> OnWaveSpawn;
        public static Action OnGameOver;
        public static UnityAction<int> OnAfterSceneLoad;
        public static UnityAction<bool> OnGamePause;
        public static Action OnGameResume;

        private NetworkManager _networkManager;
        private readonly List<NetworkConnectionToClient> _loadedPlayers = new();

        private TaskCompletionSource<object> _loadTask;

        private int _currentScene;

        #region State Properties

        private Texture2D _profilePicture;
        private string _username;
        private bool _localMultiplayer = false;
        private GameState _state = GameState.Menu;
        private float _loadingProgress;

        public Texture2D ProfilePicture
        {
            get => _profilePicture;
            set
            {
                _profilePicture = value;
                OnProfilePictureChanged?.Invoke(value);
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnUsernameChanged?.Invoke(value);
            }
        }

        public bool LocalMultiplayer
        {
            get => _localMultiplayer;
            set
            {
                _localMultiplayer = value;
                OnLocalMultiplayerChanged?.Invoke(value);
            }
        }

        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnGameStateChanged?.Invoke(value);
            }
        }

        public float LoadingProgress
        {
            get => _loadingProgress;
            set
            {
                _loadingProgress = value;
                OnLoadingProgressChanged?.Invoke(value);
            }
        }

        // Events for property changes
        public static event System.Action<Texture2D> OnProfilePictureChanged;
        public static event System.Action<string> OnUsernameChanged;
        public static event System.Action<bool> OnLocalMultiplayerChanged;
        public static event System.Action<GameState> OnGameStateChanged;
        public static event System.Action<float> OnLoadingProgressChanged;

        #endregion

        #region Unity Events

        protected override void OnAwake()
        {
            // Initialize other managers.
            LobbyManager.Initialize();
            InputManager.Initialize();
            ScriptManager.Initialize();
            WaveManager.Initialize();
            PrefabManager.Initialize();
            EntityManager.Initialize();
            DiscordController.Initialize();
            AudioManager.Initialize();
            SettingsManager.Initialize();
            UltimateManager.Initialize();
            UpgradeManager.Initialize();

            // Find references.
            _networkManager = FindObjectOfType<NetworkManager>();
        }

        private void Start()
        {
            // Add event listener for game events.
            OnGameStart += () =>
            {
                State = GameState.Playing;
            };
            OnGameOver += GameOver;
            OnAfterSceneLoad += AfterSceneLoad;
            OnGamePause += PauseGame;

            // When opening the game scene directly in the editor, bootstrap local game so players and link exist and ULT charges.
            StartCoroutine(MaybeBootstrapLocalGameWhenSceneOpenedDirectly());

            // Check if Steam is active.
            if (SteamAPI.IsSteamRunning())
            {
                // Load the user's profile picture.
                ProfilePicture = SteamUtilities.LoadLocalAvatar();
                Username = SteamFriends.GetPersonaName();
            }
        }

        /// <summary>
        /// If we're in the game scene with no players (e.g. opened 01_Game and pressed Play), set up local game so link and ULT work.
        /// </summary>
        private System.Collections.IEnumerator MaybeBootstrapLocalGameWhenSceneOpenedDirectly()
        {
            yield return null;

            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != gameScene) yield break;
            if (NetworkServer.active) yield break;
            if (EntityManager.Instance == null) yield break;
            if (EntityManager.Instance.GetPlayers().Count > 0) yield break;
            if (_networkManager == null || _networkManager.playerPrefab == null) yield break;

            SetupLocalGameInCurrentScene();
        }

        /// <summary>
        /// Spawns local players and link in the current scene (no scene load). Used when opening the game scene directly.
        /// </summary>
        private void SetupLocalGameInCurrentScene()
        {
            var player1 = CreatePlayer(PlayerRole.Fwend, null);
            var player2 = CreatePlayer(PlayerRole.Buddie, null);
            if (player1 == null || player2 == null) return;

            player2.Input = InputManager.Movement2;
            LocalMultiplayer = true;
            LinkPlayers(player1, player2);
            CreateTargetGroup(player1.transform, player2.transform);
            State = GameState.Playing;
            OnGameStart?.Invoke();
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
            // TODO: Add more clean up code.
            Time.timeScale = 1;
        }

        #endregion

        #region Game Management

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
        /// Starts the online co-op game.
        /// Called by host when both players are ready.
        /// </summary>
        public async void StartRemoteGame()
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

            var playerControllers = (from player in _loadedPlayers
                let role = LobbyManager.Instance.GetPlayerRole(player)
                select CreatePlayer(role, player)).ToList();

            EntityManager.SendSceneEntityUpdate();

            // Connect the players with the link.
            if (playerControllers.Count == 2)
            {
                LinkPlayers(
                    playerControllers.First(player => player.playerRole == PlayerRole.Fwend),
                    playerControllers.First(player => player.playerRole == PlayerRole.Buddie));
            }

            // Dismiss loading screen.
            // TODO: Implement loading screen.

            // Invoke the game start event.
            Debug.Log("Finished! Starting game...");
            NetworkServer.SendToAll(new GameStartS2CNotify());
        }

        /// <summary>
        /// Starts the local co-op game.
        /// Creates two players on the same machine.
        /// </summary>
        public async void StartLocalGame()
        {
            // Transfer to the game scene.
            var task = new TaskCompletionSource<object>();
            var operation = await LoadScene(gameScene);
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
            player2.Input = InputManager.Movement2;

            // Set the local multiplayer flag.
            LocalMultiplayer = true;

            // Create link between players.
            LinkPlayers(player1, player2);

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
        /// Resumes game playback if paused.
        /// </summary>
        public void ResumeGame()
        {
            OnGameResume?.Invoke();
            OnGamePause?.Invoke(false);
        }

        private void PauseGame(bool paused)
        {
            if (State is GameState.GameOver) return;

            State = paused ? GameState.Paused : GameState.Playing;

            // Only freeze time in local multiplayer (can't pause a networked server)
            if (LocalMultiplayer)
                Time.timeScale = paused ? 0 : 1;
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public void RestartGame()
        {
            if (LocalMultiplayer)
            {
                StartLocalGame();
            }
            else
            {
                if (!NetworkManager.IsHost()) return;
                StartRemoteGame();
            }
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        public async void StopGame()
        {
            State = GameState.Menu;
            await LoadScene(menuScene);

            // TODO: We could return the players to the lobby scene.
            if (!LocalMultiplayer)
                LobbyManager.Instance.LeaveLobby();
        }

        /// <summary>
        /// Invoked when any player dies (game over).
        /// </summary>
        private void GameOver()
        {
            State = GameState.GameOver;
            Time.timeScale = 0;
        }

        /// <summary>
        /// Invoked after the scene has finished loading.
        /// </summary>
        /// <param name="sceneId">The scene that was loaded.</param>
        private void AfterSceneLoad(int sceneId)
        {
            _currentScene = sceneId;
            State = sceneId == menuScene ? GameState.Menu : GameState.Playing;
        }

        #endregion

        /// <summary>
        /// Links two players together.
        /// </summary>
        private void LinkPlayers(PlayerController player1, PlayerController player2)
        {
            var link = PrefabManager.Create<Link>(PrefabType.Link);
            link.fwend = player1.transform;
            link.buddie = player2.transform;
        }

        /// <summary>
        /// Creates a player instance.
        /// This should only run on the server.
        /// </summary>
        private PlayerController CreatePlayer(PlayerRole role, NetworkConnectionToClient conn = null)
        {
            var playerObj = Instantiate(_networkManager.playerPrefab);
            var controller = playerObj.GetComponent<PlayerController>();
            controller.playerRole = role;

            if (conn != null)
            {
                NetworkServer.AddPlayerForConnection(conn, playerObj);
            }

            EntityManager.RegisterEntity(controller);

            return controller;
        }

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
        public static void OnEnterSceneDone(
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
        public static void OnTransferScene(TransferSceneS2CNotify notify)
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
        public static void OnLoginSuccess(PlayerLoginSuccessS2CNotify notify)
        {
            // Instance.State = GameState.Lobby;
            Debug.Log("Client finished connecting to the server.");
        }

        /// <summary>
        /// Invoked when the server notifies the client that the game has started.
        /// </summary>
        public static void OnNetworkGameStart(GameStartS2CNotify notify)
            => OnGameStart?.Invoke();

        /// <summary>
        /// Invoked when the server notifies the client that a scene entity has been updated.
        /// </summary>
        /// <param name="notify"></param>
        public static void OnSceneEntityUpdate(SceneEntityUpdateS2CNotify notify)
        {
            var manager = EntityManager.Instance;

            foreach (var entityData in notify.entities)
            {
                if (!NetworkServer.spawned.TryGetValue(entityData.netId, out var identity)) continue;
                if (!identity.TryGetComponent(out BaseEntity entity)) continue;

                if (!manager.entities.Any(e => e.entity == entity && e.netId == entityData.netId))
                {
                    manager.entities.Add(entityData);
                }
            }
        }

        #endregion

        #region Scene Management

        public async Task<AsyncOperation> LoadScene(int sceneId)
        {
            // LoadSceneMode.Single replaces the current scene; do not unload first (Unity disallows UnloadSceneAsync on the last loaded scene).
            var loadOperation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Single);
            if (loadOperation == null) throw new Exception("Failed to load scene.");

            while (!loadOperation.isDone)
            {
                State = GameState.Loading;
                LoadingProgress = loadOperation.progress * 100;
                await Task.Yield();
            }

            LoadingProgress = 0;
            OnAfterSceneLoad?.Invoke(sceneId);
            return loadOperation;
        }

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
        Playing,
        Paused,
        GameOver
    }
}
