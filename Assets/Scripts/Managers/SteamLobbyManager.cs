using Mirror;
using Steamworks;
using UnityEngine;

namespace Managers
{
    public class SteamLobbyManager : MonoBehaviour
    {
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> joinRequest;
        protected Callback<LobbyEnter_t> lobbyEntered;
        
        private const string HostAddressKey = "HostAddress";
        
        private void Start()
        {
            if (!SteamManager.Initialized) return;

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult is not EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create lobby.");
                return;
            }

            NetworkManager.singleton.StartHost();
            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby), 
                HostAddressKey, 
                SteamUser.GetSteamID().ToString());
        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkManager.singleton.isNetworkActive) return;

            var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();
        }
        
        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, NetworkManager.singleton.maxConnections);
        }

        public void JoinLobby(CSteamID lobbyID)
        {
            SteamMatchmaking.JoinLobby(lobbyID);
        }
    }
}