using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using Entity.Enemies;
using Entity.Player;
using Mirror;
using Runtime;
using Scriptable;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EntityManager : NetworkSingleton<EntityManager>
    {
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

        #region Static Management

        public static void RegisterEntity(BaseEntity entity)
        {
            if (entity.IsPlayer)
            {
                if (Instance.players.Contains(entity as PlayerController)) return;
                Instance.players.Add(entity as PlayerController);
            }
            else
            {
                if (Instance.enemies.Contains(entity as Enemy)) return;
                Instance.enemies.Add(entity as Enemy);
            }

            // if (NetworkServer.active) SendSceneEntityUpdate();
        }

        public static void UnregisterEntity(BaseEntity entity)
        {
            if (entity.IsPlayer)
            {
                if (!Instance.players.Contains(entity as PlayerController)) return;
                Instance.players.Remove(entity as PlayerController);
            }
            else
            {
                if (!Instance.enemies.Contains(entity as Enemy)) return;
                Instance.enemies.Remove(entity as Enemy);
            }

            // if (NetworkServer.active) SendSceneEntityUpdate();
        }

        public static void SendSceneEntityUpdate()
        {
            var entityData = new List<SceneEntityUpdateS2CNotify.EntityData>();
            foreach (var player in Instance.players)
            {
                if (!player) continue;

                entityData.Add(new SceneEntityUpdateS2CNotify.EntityData
                {
                    netId = player.netId,
                    isPlayer = true
                });
            }

            foreach (var enemy in Instance.enemies)
            {
                if (!enemy) continue;

                entityData.Add(new SceneEntityUpdateS2CNotify.EntityData
                {
                    netId = enemy.netId,
                    isPlayer = false
                });
            }

            var message = new SceneEntityUpdateS2CNotify { entities = entityData };
            NetworkServer.SendToAll(message);
        }

        #endregion

        public List<PlayerController> players = new();
        public List<Enemy> enemies = new();

        [SerializeField] private SpawnData spawnData;

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            players.Clear();
            enemies.Clear();
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
            players.ForEach(p =>
            {
                if (p) p.Dispose();
            });
            enemies.ForEach(e =>
            {
                if (e) e.Dispose();
            });

            players.Clear();
            enemies.Clear();
        }

        /// <summary>
        /// Spawns a random enemy from the list of enemies.
        /// </summary>
        public (Enemy, Vector2) SpawnRandom(Vector2? spawnPoint)
        {
            var allEnemies = spawnData.enemies;
            var enemy = allEnemies[Random.Range(0, allEnemies.Count)];

            var spawn = spawnData.type == SpawnType.Random ?
                GetRandomSpawn() : spawnPoint ?? GetRandomSpawn();

            return (PrefabManager.Create<Enemy>(enemy), spawn);
        }

        /// <summary>
        /// Determines a random spawn point for entities.
        /// </summary>
        private Vector2 GetRandomSpawn()
        {
            return spawnData.points.Count == 0 ?
                Vector2.zero :
                spawnData.points[Random.Range(0, spawnData.points.Count)];
        }

        /// <summary>
        /// Returns the local player.
        /// </summary>
        public PlayerController GetLocalPlayer()
        {
            return GameManager.Instance.LocalMultiplayer
                ? players.FirstOrDefault(player => player.playerRole == PlayerRole.Fwend)
                : players.FirstOrDefault(player => player.isLocalPlayer);
        }
    }
}
