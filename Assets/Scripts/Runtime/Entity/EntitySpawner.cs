using System.Collections.Generic;
using Common;
using Entity.Pathfinding;
using Managers;
using Mirror;
using UnityEngine;

namespace Runtime.World
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField]
        private PathfindingGrid grid;

        private Camera _camera;

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
            return grid.IsWalkable(point);
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
                var radius = _camera.orthographicSize * 2;
                var spawn = MathUtilities.FindValidSpawn(
                    _camera.transform.position.ToVector2(),
                    radius, IsValidSpawn);

                // Spawn an enemy.
                var (enemy, spawnPoint) = EntityManager.Instance.SpawnRandom(spawn);

                GameObject enemyObject;
                (enemyObject = enemy.gameObject).transform.SetPositionAndRotation(
                    spawnPoint, Quaternion.identity);

                NetworkServer.Spawn(enemyObject);
            }
        }
    }
}
