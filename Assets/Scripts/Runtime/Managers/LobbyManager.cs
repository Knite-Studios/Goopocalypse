using System;
using System.Collections.Generic;
using Common;
using Common.Extensions;
using Entity.Player;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using OneJS;
using Runtime;
using Steamworks;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public partial class LobbyManager : MonoSingleton<LobbyManager>
    {
        private const string LobbyConnectKey = "HostSteamId";

        public static Action<NetworkConnectionToClient> OnPlayerConnected;
        public static Action<NetworkConnectionToClient> OnPlayerDisconnected;

        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/LobbyManager");
            if (prefab == null) throw new Exception("Missing LobbyManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate LobbyManager prefab!");

            instance.name = "Managers.LobbyManager (MonoSingleton)";
        }

        public TransportType transport = TransportType.Kcp;

        private NetworkManager _networkManager;
        private SteamManager _steamManager;

        [EventfulProperty] private List<PlayerSession> _players = new();
        [EventfulProperty] private Dictionary<string, PlayerRole> _roles = new();

        #region Steam Fields

        private CSteamID _lobbyId;

        #endregion

        protected override void OnAwake()
        {
            _networkManager = GetComponent<NetworkManager>();
            _steamManager = GetComponent<SteamManager>();

#if !USE_KCP
            if (!Application.isEditor)
            {
                transport = TransportType.Steam;
            }
#endif

            // Prepare network transport.
            _networkManager.transport = transport switch
            {
                TransportType.Kcp => gameObject.GetComponent<KcpTransport>(),
                TransportType.Steam => gameObject.GetComponent<FizzySteamworks>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (transport == TransportType.Steam)
            {
                RegisterCallbacks();
                _steamManager.enabled = true;
            }

            _networkManager.transport.enabled = true;
            _networkManager.enabled = true;

            // Add callbacks for Mirror events.
            OnPlayerConnected += OnConnected;
            OnPlayerDisconnected += OnDisconnected;

            // Register packet handlers.
            NetworkServer.RegisterHandler<ChangeRoleC2SReq>(OnChangeRole);

            NetworkClient.RegisterHandler<PlayersListS2CNotify>(OnPlayersList);
        }

        /// <summary>
        /// Sends the player list to all connected clients.
        /// </summary>
        private void UpdatePlayers()
        {
            // Convert the role dictionary into an entry list.
            var roles = new List<PlayersListS2CNotify.PlayerRoleEntry>();
            foreach (var (userId, role) in _roles)
            {
                roles.Add(new PlayersListS2CNotify.PlayerRoleEntry
                    { userId = userId, role = role });
            }

            NetworkServer.SendToAll(new PlayersListS2CNotify
                { players = _players, roles = roles });
        }

        /// <summary>
        /// Finds a player by connection.
        /// </summary>
        public PlayerSession FindPlayer(NetworkConnectionToClient conn)
            => Players.Find(p => p.address == conn.address);

        /// <summary>
        /// Determines a player's role by network connection.
        /// </summary>
        public PlayerRole GetPlayerRole(NetworkConnectionToClient conn)
            => Roles[FindPlayer(conn).userId];

        #region Mirror Callbacks

        /// <summary>
        /// Invoked when a client connects to the server.
        /// </summary>
        private void OnConnected(NetworkConnectionToClient conn)
        {
            // Check if the client is already connected.
            if (_players.Exists(p => p.connection == conn))
            {
                Debug.LogWarning($"Client {conn.address} tried connecting multiple times!");
                return;
            }

            string userId;
            Texture2D profileIcon = null;
            if (transport == TransportType.Steam)
            {
                var steamId = conn.address.ToSteamId();
                userId = steamId.m_SteamID.ToString();

                profileIcon = SteamUtilities.LoadAvatar(steamId);
            }
            else
            {
                userId = conn.connectionId.ToString();
            }

            Players.Add(new PlayerSession
            {
                connection = conn,
                address = conn.address,
                userId = userId,
                profileIcon = profileIcon
            });
            Roles[userId] = PlayerRole.None;

            OnPlayersChanged?.Invoke(Players);
            OnRolesChanged?.Invoke(Roles);

            // Notify all players of the change in player list.
            UpdatePlayers();
            // Inform the client that the login was successful.
            conn.Send(new PlayerLoginSuccessS2CNotify());
        }

        /// <summary>
        /// Invoked when a client disconnects from the server.
        /// </summary>
        private void OnDisconnected(NetworkConnectionToClient conn)
            => Players.Remove(FindPlayer(conn));

        #endregion

        #region Packet Handlers

        /// <summary>
        /// Invoked when the client requests to change their role.
        /// </summary>
        private static void OnChangeRole(NetworkConnectionToClient conn, ChangeRoleC2SReq req)
        {
            var roles = Instance.Roles;

            // Check if the role has already been taken.
            if (roles.ContainsValue(req.role))
            {
                Debug.LogWarning($"Role {req.role} is already taken!");
                return;
            }

            // Update the player's role.
            var player = Instance.FindPlayer(conn);
            roles[player.userId] = req.role;

            // Send the updated player list to all clients.
            Instance.UpdatePlayers();
        }

        /// <summary>
        /// Sets the list of players connected to the server.
        /// </summary>
        /// <param name="notify"></param>
        private static void OnPlayersList(PlayersListS2CNotify notify)
        {
            Instance.Players = notify.players;

            Instance.Roles.Clear();
            foreach (var roleEntry in notify.roles)
            {
                Instance.Roles[roleEntry.userId] = roleEntry.role;
            }
            Instance.OnRolesChanged?.Invoke(Instance.Roles);
        }

        #endregion

        #region Steam Callbacks

        /// <summary>
        /// Registers all Steam callbacks.
        /// </summary>
        private void RegisterCallbacks()
        {
            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        }

        /// <summary>
        /// Invoked when the Steam lobby is created.
        /// </summary>
        /// <param name="callback">The callback info.</param>
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            _lobbyId = callback.m_ulSteamIDLobby.ToSteamId();

            // Set the user's lobby data.
            SteamMatchmaking.SetLobbyData(
                _lobbyId, LobbyConnectKey,
                SteamUser.GetSteamID().ToString());

            if (!NetworkServer.active)
            {
                // Only start hosting if the server isn't running.
                _networkManager.StartHost();
            }
        }

        /// <summary>
        /// Invoked when the user tries to join a lobby from a friend's invite.
        /// </summary>
        /// <param name="callback">The callback info.</param>
        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
            => SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

        /// <summary>
        /// Invoked when the client connects to a Steam lobby.
        /// </summary>
        /// <param name="callback">The callback info.</param>
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            // Check if the user is already connected.
            if (_networkManager.isNetworkActive)
            {
                return;
            }

            var hostAddress = SteamMatchmaking.GetLobbyData(
                callback.m_ulSteamIDLobby.ToSteamId(),
                LobbyConnectKey);
            _networkManager.networkAddress = hostAddress;
            _networkManager.StartClient();
        }

        #endregion

        #region Network Callbacks

        /// <summary>
        /// Informs the server that the player is ready.
        /// </summary>
        // [Command]
        private void CmdReady()
        {
            // TODO: Keep track of how many players are ready.
            // Once all players are ready, give the server the go-ahead to start the game.
        }

        /// <summary>
        /// Invoked by the server to prepare the game.
        /// Creates player objects and changes to the main game scene.
        /// </summary>
        // [ClientRpc]
        private void StartGame()
        {
            // Spawn the player prefab.
        }

        #endregion

        #region Lobby Methods

        /// <summary>
        /// Creates a lobby for players to join.
        /// </summary>
        public void MakeLobby()
        {
            switch (transport)
            {
                case TransportType.Steam:
                    SteamMatchmaking.CreateLobby(
                        ELobbyType.k_ELobbyTypeFriendsOnly,
                        _networkManager.maxConnections);
                    break;
                case TransportType.Kcp:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Change the game state.
            GameManager.Instance.State = GameState.Lobby;
        }

        /// <summary>
        /// Closes an open lobby, if it exists.
        /// </summary>
        public void CloseLobby()
        {
            switch (transport)
            {
                case TransportType.Steam:
                    if (!_lobbyId.IsValid()) return;
                    SteamMatchmaking.SetLobbyJoinable(_lobbyId, false);
                    break;
                case TransportType.Kcp:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Opens a menu to invite friends.
        /// </summary>
        public void InvitePlayer()
        {
            switch (transport)
            {
                case TransportType.Steam:
                    SteamFriends.ActivateGameOverlayInviteDialog(_lobbyId);
                    break;
            }
        }

        #endregion
    }

    public enum TransportType
    {
        Kcp,
        Steam
    }

    /// <summary>
    /// Interface for managing lobbies.
    /// </summary>
    public class Lobby
    {

    }
}
