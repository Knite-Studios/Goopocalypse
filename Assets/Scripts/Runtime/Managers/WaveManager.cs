using System.Collections;
using Entity;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class WaveManager : NetworkSingleton<WaveManager>
    {
        [ReadOnly, SyncVar]
        public int waveCount = 1;
        [ReadOnly, SyncVar]
        public long matchTimer;

        private bool _gameRunning;
        private Camera _camera;

        /// <summary>
        /// Cache a reference to the camera for spawning.
        /// </summary>
        protected override void OnAwake() => _camera = Camera.main;

        private void Start()
        {
            if (isClient) return;

            GameManager.OnGameStart += OnGameStart;
        }

        /// <summary>
        /// Invoked when the game starts.
        /// </summary>
        [Server]
        private void OnGameStart()
        {
            _gameRunning = true;
            waveCount = 1;

            StartCoroutine(CountTimer());
        }

        /// <summary>
        /// Counts the time elapsed since the game started.
        /// </summary>
        private IEnumerator CountTimer()
        {
            while (_gameRunning)
            {
                yield return new WaitForSeconds(1);
                matchTimer++;
            }
        }

        /// <summary>
        /// Creates a wave of enemies.
        /// </summary>
        [Server]
        public void SpawnWave()
        {
            // TODO: Replace with a proper calculation for enemy count.
            for (var i = 0; i < waveCount; i++)
            {
                // Spawn an enemy.
                var enemy = PrefabManager.Create<Enemy>(PrefabType.MeleeEnemy);
                NetworkServer.Spawn(enemy.gameObject);
            }
        }
    }
}
