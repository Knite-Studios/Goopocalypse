using System;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class LobbyManager : MonoSingleton<LobbyManager>
    {
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
                _steamManager.enabled = true;
            }

            _networkManager.transport.enabled = true;
            _networkManager.enabled = true;
        }
    }

    public enum TransportType
    {
        Kcp,
        Steam
    }
}
