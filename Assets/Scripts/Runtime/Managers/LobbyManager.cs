using System;
using Common.Extensions;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class LobbyManager : MonoSingleton<LobbyManager>
    {
        private const string LobbyConnectKey = "HostSteamId";

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
        }

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

            _networkManager.StartHost();
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
                _networkManager.StopClient();
            }

            var hostAddress = SteamMatchmaking.GetLobbyData(
                callback.m_ulSteamIDLobby.ToSteamId(),
                LobbyConnectKey);
            _networkManager.networkAddress = hostAddress;
            _networkManager.StartClient();
        }

        #endregion

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
            // TODO: Show IP and port to connect.
        }

        /// <summary>
        /// Informs the server that the player is ready.
        /// </summary>
        // [Command]
        public void CmdReady()
        {
            // TODO: Keep track of how many players are ready.
            // Once all players are ready, give the server the go-ahead to start the game.
        }

        /// <summary>
        /// Invoked by the server to prepare the game.
        /// Creates player objects and changes to the main game scene.
        /// </summary>
        // [ClientRpc]
        public void StartGame()
        {
            // Spawn the player prefab.
        }
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
