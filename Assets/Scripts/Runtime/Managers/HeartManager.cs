using Mirror;
using UnityEngine.SceneManagement;

namespace Managers
{
    public partial class HeartManager : NetworkSingleton<HeartManager>
    {
        #region Static Management

        public static void OnHeartsUpdate(HeartUpdateS2CNotify notify)
            => Instance.Hearts = notify.hearts;

        public static void OnPlayerDeath()
        {
            Instance.Hearts--;
            if (NetworkServer.active) NetworkServer.SendToAll(new HeartUpdateS2CNotify
                { hearts = Instance.Hearts });
        }

        #endregion

        [SyncVar] private int _hearts = 3;

        public int Hearts
        {
            get => _hearts;
            set
            {
                _hearts = value;
                OnHeartsChanged?.Invoke(value);
            }
        }

        public static event System.Action<int> OnHeartsChanged;

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
