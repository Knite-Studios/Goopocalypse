﻿using Mirror;
using UnityEngine;

namespace Managers
{
    public class NetworkManager : Mirror.NetworkManager
    {
        public static bool IsHost()
            => NetworkServer.active && NetworkClient.isConnected;

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
