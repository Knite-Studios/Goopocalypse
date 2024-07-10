using System;
using Mirror;
using Runtime;

namespace Managers
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public static bool IsHost()
            => NetworkServer.active && NetworkClient.isConnected;

        protected override void RegisterServerMessages()
        {
            base.RegisterServerMessages();

            NetworkServer.RegisterHandler<ChangeRoleC2SReq>(LobbyManager.OnChangeRole);

            NetworkServer.RegisterHandler<EnterSceneDoneC2SNotify>(GameManager.OnEnterSceneDone);
        }

        protected override void RegisterClientMessages()
        {
            base.RegisterClientMessages();

            NetworkClient.RegisterHandler<PlayersListS2CNotify>(LobbyManager.OnPlayersList);

            NetworkClient.RegisterHandler<TransferSceneS2CNotify>(GameManager.OnTransferScene);
            NetworkClient.RegisterHandler<PlayerLoginSuccessS2CNotify>(GameManager.OnLoginSuccess);
            NetworkClient.RegisterHandler<GameStartS2CNotify>(GameManager.OnNetworkGameStart);
            NetworkClient.RegisterHandler<GameOverS2CNotify>(_ => GameManager.OnGameOver?.Invoke());
            NetworkClient.RegisterHandler<SceneEntityUpdateS2CNotify>(GameManager.OnSceneEntityUpdate);

            NetworkClient.RegisterHandler<WaveInfoS2CNotify>(WaveManager.OnWaveInfo);
            NetworkClient.RegisterHandler<ScoreUpdateS2CNotify>(WaveManager.OnScoreUpdate);
        }

        public override void OnClientDisconnect()
        {
            // If in lobby as client, navigate back.
            // TODO: Show message stating user got disconnected.
            GameManager.Instance.Navigate("/");
            LobbyManager.Instance.DisposeConnection();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            LobbyManager.OnPlayerConnected?.Invoke(conn);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            LobbyManager.OnPlayerDisconnected?.Invoke(conn);
        }
    }
}
