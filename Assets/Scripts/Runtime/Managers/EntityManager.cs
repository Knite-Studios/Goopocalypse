using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Enemies;
using Entity.Player;
using Scriptable;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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

        public static void RegisterPlayer(PlayerController player)
        {
            if (Instance.players.Contains(player)) return;
            Instance.players.Add(player);
        }

        public static void UnregisterPlayer(PlayerController player)
        {
            if (!Instance.players.Contains(player)) return;
            Instance.players.Remove(player);
        }

        public static void RegisterEnemy(Enemy enemy)
        {
            if (Instance.enemies.Contains(enemy)) return;
            Instance.enemies.Add(enemy);
        }

        public static void UnregisterEnemy(Enemy enemy)
        {
            if (!Instance.enemies.Contains(enemy)) return;
            Instance.enemies.Remove(enemy);
        }

        #endregion

        public List<PlayerController> players = new();
        public List<Enemy> enemies = new();

        [SerializeField] private SpawnData spawnData;

        protected override void OnSceneUnloaded(Scene scene)
        {
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
