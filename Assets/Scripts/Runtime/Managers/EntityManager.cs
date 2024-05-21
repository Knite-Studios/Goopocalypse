using System;
using Cinemachine;
using Mirror;
using Player;
using UnityEngine;

namespace Managers
{
    public class EntityManager : NetworkSingleton<EntityManager>
    {
        public CinemachineTargetGroup targetGroup;

        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/EntityManager");
            if (prefab == null) throw new Exception("Missing EntityManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate EntityManager prefab!");

            instance.name = "Managers.EntityManager (Singleton)";
        }

        public override void OnStartServer()
        {
            Debug.Log("EntityManager.OnStartServer");
            targetGroup = new GameObject("TargetGroup").AddComponent<CinemachineTargetGroup>();
            NetworkServer.Spawn(targetGroup.gameObject);
        }

        [Server]
        private void AddPlayerToTargetGroup(PlayerController player)
        {
            if (targetGroup == null) return;

            var playerTransform = player.transform;
            targetGroup.AddMember(playerTransform, 1, 2);
            RpcAddPlayerToTargetGroup(player.netIdentity);
        }

        [Server]
        private void RemovePlayerFromTargetGroup(PlayerController player)
        {
            if (targetGroup == null) return;

            var playerTransform = player.transform;
            targetGroup.RemoveMember(playerTransform);
            RpcRemovePlayerFromTargetGroup(player.netIdentity);
        }

        [ClientRpc]
        private void RpcAddPlayerToTargetGroup(NetworkIdentity playerIdentity)
        {
            var player = playerIdentity.GetComponent<PlayerController>();
            if (player == null) return;

            var playerTransform = player.transform;
            targetGroup.AddMember(playerTransform, 1, 2);
        }

        [ClientRpc]
        private void RpcRemovePlayerFromTargetGroup(NetworkIdentity playerIdentity)
        {
            var player = playerIdentity.GetComponent<PlayerController>();
            if (player == null) return;

            var playerTransform = player.transform;
            targetGroup.RemoveMember(playerTransform);
        }

        [Command]
        public void CmdAddPlayerToTargetGroup(PlayerController player)
        {
            AddPlayerToTargetGroup(player);
        }

        [Command]
        public void CmdRemovePlayerFromTargetGroup(PlayerController player)
        {
            RemovePlayerFromTargetGroup(player);
        }
    }
}
