using System.Collections;
using Mirror;
using OneJS;
using Runtime;
using UnityEngine;

namespace Managers
{
    public partial class WaveManager : MonoSingleton<WaveManager>
    {
        [EventfulProperty] private int _waveCount = 1;
        [EventfulProperty] private long _matchTimer;

        [Tooltip("The amount of seconds it takes to spawn a wave.")]
        public int spawnThreshold = 10;

        [SerializeField]
        [Tooltip("Enable to spawn waves over time; disable to spawn waves manually.")]
        private bool spawnOverTime = true;

        private bool _gameRunning;

        protected override void OnAwake()
        {
            NetworkClient.RegisterHandler<WaveInfoS2CNotify>(OnWaveInfo);
        }

        private void Start()
        {
            GameManager.OnGameStart += OnGameStart;
        }

        #region Packet Handlers

        private static void OnWaveInfo(WaveInfoS2CNotify notify)
        {
            if (NetworkServer.activeHost) return;

            Instance.WaveCount = notify.wave;
            Instance.MatchTimer = notify.timer;
        }

        #endregion

        private void OnGameStart()
        {
            if (!NetworkServer.activeHost) return;

            _gameRunning = true;
            WaveCount = 1;

            if (spawnOverTime)
            {
                StartCoroutine(CountTimer());
            }
        }

        private void Tick()
        {
            if (_matchTimer % spawnThreshold == 0 && spawnOverTime)
            {
                SpawnWave();
                WaveCount++;
            }

            NetworkServer.SendToAll(new WaveInfoS2CNotify { wave = WaveCount, timer = MatchTimer });
        }

        private IEnumerator CountTimer()
        {
            while (_gameRunning)
            {
                yield return new WaitForSeconds(1);
                MatchTimer++;
                Tick();
            }
        }

        public void SpawnWave() => GameManager.OnWaveSpawn?.Invoke(_waveCount);

        // Manual wave spawn, can be called from a UI button when spawnOverTime is false
        public void ManualSpawnWave()
        {
            if (!spawnOverTime)
            {
                SpawnWave();
                WaveCount++;
                MatchTimer = 0; // Reset timer if spawning manually
            }
        }
    }
}
