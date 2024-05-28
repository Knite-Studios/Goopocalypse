﻿using Common;
using Entity.Enemies;
using Managers;
using Mirror;
using UnityEngine;

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
            return _world.IsValidSpawn(point);
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
                var spawnPoint = MathUtilities.FindValidSpawn(
                    _world.center, radius, IsValidSpawn);

                // Spawn an enemy.
                var enemy = PrefabManager.Create<Enemy>(PrefabType.MeleeEnemy);

                GameObject enemyObject;
                (enemyObject = enemy.gameObject).transform.SetPositionAndRotation(
                    spawnPoint, Quaternion.identity);

                NetworkServer.Spawn(enemyObject);
            }
        }
    }
}
