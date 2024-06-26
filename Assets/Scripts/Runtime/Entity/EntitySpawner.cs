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
            var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            if (spawnPoints.Length > 0)
            {
                // If we find objects of tag "SpawnPoint", we will spawn enemies there.
                for (var i = 0; i < count; i++)
                {
                    var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                    var (enemy, _) = EntityManager.Instance.SpawnRandom(spawnPoint);

                    GameObject enemyObject;
                    (enemyObject = enemy.gameObject).transform.SetPositionAndRotation(
                        spawnPoint, Quaternion.identity);

                    NetworkServer.Spawn(enemyObject);
                }
            }
            else
            {
                // If we don't find any spawn points, we will spawn enemies at the camera's position.
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
}
