using System.Collections.Generic;
using Common;
using Entity.Enemies;
using Managers;
using Mirror;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;

namespace Runtime.World
{
    public class EntitySpawner : MonoBehaviour
    {
        private Camera _camera;
        private Grid _grid;

        /// <summary>
        /// Cache a reference to the camera for spawning.
        /// </summary>
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable() => GameManager.OnWaveSpawn += SpawnWave;

        private void OnDisable() => GameManager.OnWaveSpawn -= SpawnWave;

        /// <summary>
        /// Determines if an enemy can spawn on a point.
        /// TODO: Check if point is within x distance of a player.
        /// </summary>
        private bool IsValidSpawn(Vector2 point)
        {
            return _grid.IsWalkable(point);
        }

        /// <summary>
        /// Returns a random enemy with the given list of enemies.
        /// </summary>
        private PrefabType GetRandomEnemy(List<PrefabType> enemies)
        {
            return enemies[Random.Range(0, enemies.Count)];
        }

        /// <summary>
        /// Spawns a wave of enemies.
        /// </summary>
        /// <param name="count">The amount of enemies to spawn.</param>
        private void SpawnWave(int count)
        {
            for (var i = 0; i < count; i++)
            {
                // Determine where to spawn the enemy.
                // TODO: Pick a random spawn point center.
                var radius = _camera.orthographicSize * 2;
                var spawnPoint = MathUtilities.FindValidSpawn(
                    Vector2.zero, radius, IsValidSpawn);

                // TODO: Temporary for prototype.
                var enemyList = new List<PrefabType>
                {
                    PrefabType.BasicEnemy,
                    PrefabType.DashEnemy,
                    PrefabType.LootGoblin,
                    PrefabType.RandomEnemy,
                    PrefabType.RangedEnemy,
                    PrefabType.WeepingAngelEnemy
                };
                // Spawn an enemy.
                var enemy = PrefabManager.Create<Enemy>(GetRandomEnemy(enemyList));

                GameObject enemyObject;
                (enemyObject = enemy.gameObject).transform.SetPositionAndRotation(
                    spawnPoint, Quaternion.identity);

                NetworkServer.Spawn(enemyObject);
            }
        }
    }
}
