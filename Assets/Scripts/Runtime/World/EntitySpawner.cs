using Entity.Enemies;
using Managers;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.World
{
    public class EntitySpawner : MonoBehaviour
    {
        private Camera _camera;
        private World _world;

        /// <summary>
        /// Cache a reference to the camera for spawning.
        /// </summary>
        private void Awake()
        {
            _camera = Camera.main;
            _world = FindObjectOfType<World>();
        }

        private void OnEnable() => GameManager.OnWaveSpawn += SpawnWave;

        private void OnDisable() => GameManager.OnWaveSpawn -= SpawnWave;

        /// <summary>
        /// Determines if an enemy can spawn on a point.
        /// TODO: Check if point is within x distance of a player.
        /// </summary>
        private bool IsValidSpawn(Vector2 point)
        {
            // Check if the point is outside of the world's bounds.
            if (point.x < 0 || point.x >= _world.width ||
                point.y < 0 || point.y >= _world.height)
                return false;
            return _world.IsWalkable(point);
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

                Vector3 spawnPoint;
                while (true)
                {
                    var point = (Random.insideUnitCircle.normalized * radius).Round();
                    spawnPoint = point + _world.center;

                    if (IsValidSpawn(spawnPoint)) break;
                }

                // Spawn an enemy.
                var enemy = PrefabManager.Create<Enemy>(PrefabType.MeleeEnemy);
                enemy.gameObject.transform.SetPositionAndRotation(
                    spawnPoint, Quaternion.identity);
                // Target the player.
                // enemy.Target = EntityManager.Player;

                NetworkServer.Spawn(enemy.gameObject);
            }
        }
    }
}
