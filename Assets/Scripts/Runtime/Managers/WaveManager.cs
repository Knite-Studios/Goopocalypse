using System.Collections;
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

        [Tooltip("The amount of seconds it takes to spawn a wave.")]
        public int spawnThreshold = 30;

        private bool _gameRunning;

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
        /// Called once per second.
        /// </summary>
        [Server]
        private void Tick()
        {
            if (matchTimer % spawnThreshold == 0)
            {
                SpawnWave();
                waveCount++;
            }
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

                Tick();
            }
        }

        /// <summary>
        /// Creates a wave of enemies.
        /// TODO: Add a correct entity count.
        /// </summary>
        [Server]
        public void SpawnWave() => GameManager.OnWaveSpawn?.Invoke(waveCount);
    }
}
