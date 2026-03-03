using Managers;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Reward panel for Twin Lights.
    /// Shows 3 upgrade options when XP goal is reached, plus Reroll and Skip.
    /// Cards display upgrade name, description, cost, and optional icon.
    /// </summary>
    public class RewardPanelController : MonoBehaviour
    {
        [Header("Panel Root")]
        [Tooltip("Root GameObject for the reward panel UI. This gets shown/hidden when rewards are ready. Do NOT assign this component's own GameObject here; use a child panel so this script stays enabled.")]
        [SerializeField] private GameObject panelRoot;

        [System.Serializable]
        private class RewardCardView
        {
            [Header("Card Button")]
            public Button button;

            [Header("Text")]
            public TextMeshProUGUI titleText;
            public TextMeshProUGUI descriptionText;
            public TextMeshProUGUI costText;

            [Header("Icon (optional)")]
            public Image iconImage;
        }

        [Header("Reward Cards (3)")]
        [SerializeField] private RewardCardView[] cardViews = new RewardCardView[3];

        [Header("Actions")]
        [SerializeField] private Button rerollButton;
        [SerializeField] private Button skipButton;

        private readonly UpgradeDefinition[] _currentOptions = new UpgradeDefinition[3];

        private void OnEnable()
        {
            XpManager.OnRewardReady += HandleRewardReady;
        }

        private void OnDisable()
        {
            XpManager.OnRewardReady -= HandleRewardReady;
        }

        private void Start()
        {
            // Start hidden but keep this component enabled so it can listen for OnRewardReady.
            if (panelRoot != null)
            {
                if (panelRoot == gameObject)
                {
                    Debug.LogWarning("RewardPanelController: panelRoot is set to this GameObject. This will disable the script when hiding the panel. Use a child GameObject as the panel root instead.");
                }
                else
                {
                    panelRoot.SetActive(false);
                }
            }

            // Wire up card buttons.
            for (var i = 0; i < cardViews.Length; i++)
            {
                var index = i;
                var view = cardViews[i];
                if (view != null && view.button != null)
                {
                    view.button.onClick.AddListener(() => OnCardClicked(index));
                }
            }

            if (rerollButton != null)
            {
                rerollButton.onClick.AddListener(OnRerollClicked);
            }

            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipClicked);
            }
        }

        private void HandleRewardReady()
        {
            if (!XpManager.Instance || !XpManager.Instance.RewardPending)
                return;

            if (!UpgradeManager.HasInstance())
            {
                Debug.LogWarning("RewardPanelController: UpgradeManager instance not available; cannot show reward options.");
                return;
            }

            ShowPanel();
            RefreshOptions();
        }

        private void ShowPanel()
        {
            if (panelRoot != null && panelRoot != gameObject)
            {
                panelRoot.SetActive(true);
            }
        }

        private void HidePanel()
        {
            if (panelRoot != null && panelRoot != gameObject)
            {
                panelRoot.SetActive(false);
            }
        }

        /// <summary>
        /// Fills the 3 card buttons with random upgrade options from UpgradeManager.
        /// </summary>
        public void RefreshOptions()
        {
            if (!UpgradeManager.HasInstance())
                return;

            var upgrades = UpgradeManager.Instance.GetRandomUpgrades(cardViews.Length);
            var currentOrbs = WaveManager.HasInstance() ? WaveManager.Instance.Score : 0L;

            for (var i = 0; i < cardViews.Length; i++)
            {
                var view = cardViews[i];
                var definition = upgrades != null && i < upgrades.Count ? upgrades[i] : null;
                if (i < _currentOptions.Length)
                    _currentOptions[i] = definition;

                if (view == null || view.button == null)
                    continue;

                if (definition == null)
                {
                    view.button.interactable = false;
                    if (view.titleText != null) view.titleText.text = string.Empty;
                    if (view.descriptionText != null) view.descriptionText.text = string.Empty;
                    if (view.costText != null) view.costText.text = string.Empty;
                    if (view.iconImage != null)
                    {
                        view.iconImage.sprite = null;
                        view.iconImage.enabled = false;
                    }
                    continue;
                }

                if (view.titleText != null)
                    view.titleText.text = definition.displayName;

                if (view.descriptionText != null)
                    view.descriptionText.text = definition.description;

                if (view.costText != null)
                    view.costText.text = $"{definition.costInOrbs} orbs";

                if (view.iconImage != null)
                {
                    view.iconImage.sprite = definition.icon;
                    view.iconImage.enabled = definition.icon != null;
                }

                var canAfford = UpgradeManager.Instance.CanAfford(definition, currentOrbs);
                view.button.interactable = canAfford;
            }
        }

        private void OnCardClicked(int index)
        {
            if (index < 0 || index >= _currentOptions.Length)
                return;

            if (!XpManager.Instance || !XpManager.Instance.RewardPending)
                return;

            if (!UpgradeManager.HasInstance())
                return;

            var definition = _currentOptions[index];
            if (definition == null)
                return;

            var applied = UpgradeManager.Instance.Apply(definition);
            if (!applied)
            {
                // Not enough orbs or missing managers; refresh to update interactable state.
                RefreshOptions();
                return;
            }

            XpManager.Instance.PurchaseReward();
            HidePanel();
        }

        private void OnRerollClicked()
        {
            if (!XpManager.Instance || !XpManager.Instance.RewardPending)
                return;

            RefreshOptions();
        }

        private void OnSkipClicked()
        {
            if (!XpManager.Instance || !XpManager.Instance.RewardPending)
                return;

            XpManager.Instance.SkipReward();
            HidePanel();
        }
    }
}
