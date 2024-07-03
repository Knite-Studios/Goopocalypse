using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using Entity.Enemies;
using Entity.Player;
using Mirror;
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
            // TODO: This is temporary since on the host's side, its updating both the host and client entities.
            if (entity.isServer) return;

            var entityData = new EntityData
            {
                entity = entity,
                netId = entity.netId,
            };

            if (Instance.entities.Any(e => e.entity == entity && e.netId == entity.netId)) return;
            Instance.entities.Add(entityData);

            if (NetworkServer.active) SendSceneEntityUpdate();
        }

        public static void UnregisterEntity(BaseEntity entity)
        {
            var entityData = Instance.entities.FirstOrDefault(e => e.entity == entity);
            if (entityData.entity == null) return;

            Instance.entities.Remove(entityData);

            if (NetworkServer.active) SendSceneEntityUpdate();
        }

        // public static void RegisterPlayer(PlayerController player)
        // {
        //     if (Instance.players.Contains(player)) return;
        //     Instance.players.Add(player);
        // }
        //
        // public static void UnregisterPlayer(PlayerController player)
        // {
        //     if (!Instance.players.Contains(player)) return;
        //     Instance.players.Remove(player);
        // }
        //
        // public static void RegisterEnemy(Enemy enemy)
        // {
        //     if (Instance.enemies.Contains(enemy)) return;
        //     Instance.enemies.Add(enemy);
        // }
        //
        // public static void UnregisterEnemy(Enemy enemy)
        // {
        //     if (!Instance.enemies.Contains(enemy)) return;
        //     Instance.enemies.Remove(enemy);
        // }

        public static void SendSceneEntityUpdate()
        {
            var entityData = new List<EntityData>();
            foreach (var entity in Instance.entities)
            {
                if (!entity.entity) continue;

                entityData.Add(new EntityData
                {
                    entity = entity.entity,
                    netId = entity.netId
                });
            }

            var message = new SceneEntityUpdateS2CNotify { entities = entityData };
            NetworkServer.SendToAll(message);
        }

        #endregion

        // public List<PlayerController> players = new();
        // public List<Enemy> enemies = new();
        public List<EntityData> entities = new();

        [SerializeField] private SpawnData spawnData;

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // players.Clear();
            // enemies.Clear();
            entities.Clear();
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
            // players.ForEach(p =>
            // {
            //     if (p) p.Dispose();
            // });
            // enemies.ForEach(e =>
            // {
            //     if (e) e.Dispose();
            // });
            //
            // players.Clear();
            // enemies.Clear();

            entities.ForEach(e =>
            {
                if (e.entity) e.entity.Dispose();
            });
            entities.Clear();

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

        public List<PlayerController> GetPlayers()
        {
            return entities
                .Select(e => e.entity)
                .OfType<PlayerController>()
                .ToList();
        }

        /// <summary>
        /// Returns the local player.
        /// </summary>
        public PlayerController GetLocalPlayer()
        {
            var players = GetPlayers();
            return GameManager.Instance.LocalMultiplayer
                ? players.FirstOrDefault(player => player.playerRole == PlayerRole.Fwend)
                : players.FirstOrDefault(player => player.isLocalPlayer);
        }
    }
}
