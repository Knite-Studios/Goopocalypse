using Entity.Player;
using Mirror;
using OneJS;
using UnityEngine.SceneManagement;

namespace Managers
{
    public partial class HeartManager : NetworkSingleton<HeartManager>
    {
        #region Static Management

        public static void OnHeartsUpdate(HeartUpdateS2CNotify notify)
            => Instance.Hearts = notify.hearts;

        public static void OnPlayerRespawn(PlayerRespawnS2CNotify notify)
        {
            var player = NetworkServer.spawned[uint.Parse(notify.userId)].GetComponent<PlayerController>();
            player.RespawnPlayer();
        }

        public static void OnPlayerDeath()
        {
            Instance.Hearts--;
            if (NetworkServer.active) NetworkServer.SendToAll(new HeartUpdateS2CNotify
                { hearts = Instance.Hearts });
        }

        #endregion

        [SyncVar, EventfulProperty] private int _hearts = 3;

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnHeartsChanged += HandleHeartsChanged;
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
            OnHeartsChanged -= HandleHeartsChanged;
            Hearts = 3;
        }

        private void HandleHeartsChanged(int sharedHearts)
        {
            // TODO: Here, we can update the UI.

            // Handle the game over logic.
            if (sharedHearts <= 0)
            {
                if (NetworkServer.active)
                    NetworkServer.SendToAll(new GameOverS2CNotify());
                else
                    GameManager.OnGameOver?.Invoke();
            }
        }
    }
}
