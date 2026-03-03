using Mirror;
using Scriptable;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Tracks the shared ultimate bar for Twin Lights Link.
    /// Phase 1: when full, fires a simple event; boons are handled later.
    /// </summary>
    public class UltimateManager : NetworkSingleton<UltimateManager>
    {
        public new static void Initialize()
        {
            if (FindObjectOfType<UltimateManager>() != null) return;

            var prefab = Resources.Load<GameObject>("Prefabs/Managers/UltimateManager");
            if (prefab == null)
            {
                Debug.LogWarning("Missing UltimateManager prefab; creating bare instance.");
                NetworkSingleton<UltimateManager>.Initialize();
                return;
            }

            var instance = Instantiate(prefab);
            instance.name = "Managers.UltimateManager (NetworkSingleton)";
        }

        [SerializeField] private UltimateConfig config;

        [SyncVar] private float _currentCharge;
        [SyncVar] private float _requiredCharge;
        /// <summary> Used when LocalMultiplayer so ult bar works without a server. </summary>
        private float _currentChargeLocal;
        private float _requiredChargeLocal;

        public float CurrentCharge => (GameManager.Instance != null && GameManager.Instance.LocalMultiplayer) ? _currentChargeLocal : _currentCharge;
        public float RequiredCharge => (GameManager.Instance != null && GameManager.Instance.LocalMultiplayer) ? _requiredChargeLocal : _requiredCharge;
        public float NormalizedCharge => RequiredCharge > 0f ? CurrentCharge / RequiredCharge : 0f;

        public static event System.Action OnUltimateReady;

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.LocalMultiplayer && config)
                ResetChargeLocal();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (GameManager.Instance != null)
                GameManager.OnLocalMultiplayerChanged += OnLocalMultiplayerChanged;
        }

        private void OnDisable()
        {
            GameManager.OnLocalMultiplayerChanged -= OnLocalMultiplayerChanged;
        }

        private void OnLocalMultiplayerChanged(bool isLocalMultiplayer)
        {
            if (isLocalMultiplayer && config)
                ResetChargeLocal();
        }

        protected override void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            base.OnSceneLoaded(scene, mode);

            if (GameManager.Instance != null && GameManager.Instance.LocalMultiplayer)
            {
                ResetChargeLocal();
                return;
            }
            if (!isServer) return;
            ResetCharge();
        }

        private void ResetChargeLocal()
        {
            if (!config) return;
            _requiredChargeLocal = config.ultMax;
            _currentChargeLocal = 0f;
        }

        [Server]
        private void ResetCharge()
        {
            if (!config)
            {
                Debug.LogWarning("UltimateManager has no UltimateConfig assigned.");
                _requiredCharge = 0f;
                _currentCharge = 0f;
                return;
            }

            _requiredCharge = config.ultMax;
            _currentCharge = 0f;
        }

        /// <summary>
        /// Adds time-based charge while the link is active (server).
        /// </summary>
        [Server]
        public void AddTimeCharge(float deltaTime)
        {
            if (!config || deltaTime <= 0f) return;
            var mult = UpgradeManager.HasInstance() ? UpgradeManager.Instance.GetCourageGainMultiplier() : 1f;
            var amount = config.baseChargePerSecondWhileLinked * mult * deltaTime;
            AddCharge(amount);
        }

        /// <summary>
        /// Adds time-based charge when LocalMultiplayer (no server).
        /// </summary>
        public void AddTimeChargeLocal(float deltaTime)
        {
            if (!config || deltaTime <= 0f || _requiredChargeLocal <= 0f) return;
            var mult = UpgradeManager.HasInstance() ? UpgradeManager.Instance.GetCourageGainMultiplier() : 1f;
            var amount = config.baseChargePerSecondWhileLinked * mult * deltaTime;
            _currentChargeLocal = Mathf.Min(_requiredChargeLocal, _currentChargeLocal + amount);
            if (_currentChargeLocal >= _requiredChargeLocal)
            {
                OnUltimateReady?.Invoke();
                _requiredChargeLocal *= config.chargeScalingPerBoon;
                _currentChargeLocal = 0f;
            }
        }

        [Server]
        public void AddCharge(float amount)
        {
            if (!config || amount <= 0f || _requiredCharge <= 0f) return;

            _currentCharge = Mathf.Min(_requiredCharge, _currentCharge + amount);

            if (_currentCharge >= _requiredCharge)
            {
                RpcNotifyUltimateReady();
                // Phase 1: immediately reset for next cycle with scaled requirement.
                _requiredCharge *= config.chargeScalingPerBoon;
                _currentCharge = 0f;
            }
        }

        [ClientRpc]
        private void RpcNotifyUltimateReady()
        {
            OnUltimateReady?.Invoke();
        }
    }
}

