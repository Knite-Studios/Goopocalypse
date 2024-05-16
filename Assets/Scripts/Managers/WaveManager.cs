using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class WaveManager : NetworkSingleton<WaveManager>
    {
        [SyncVar] private int _waveCount = 1;
        [SyncVar] private long _matchTimer = 0; // Seconds elapsed since match start.

        private bool _isWaveActive;
        private int _enemiesRemaining;
        private List<GameObject> _aliveEnemies = new();

        private void Start()
        {
            GameManager.OnGameStart += OnGameStart;
            Debug.Log("Initialized wave manager.");
        }

        /// <summary>
        /// Called when the game is requested to start.
        /// </summary>
        [Server]
        private void OnGameStart()
        {
            Debug.Log("Game started");

            _isWaveActive = true;

            StartCoroutine(SpawnEnemies());
        }

        /// <summary>
        /// Handles spawning enemies during a wave.
        /// </summary>
        private IEnumerator SpawnEnemies()
        {
            while (_isWaveActive)
            {
                Debug.Log("Try spawning enemies");
                yield return new WaitForSeconds(3);
            }
        }

        private void StartWave()
        {

        }
    }
}
