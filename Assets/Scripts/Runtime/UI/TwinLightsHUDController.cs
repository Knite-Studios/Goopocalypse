using Entity;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// HUD: Courage bar (ultimate), XP bar + "X xp till next upgrade", and reward panel when goal reached.
    /// </summary>
    public class TwinLightsHUDController : MonoBehaviour
    {
        [Header("Courage (ultimate)")]
        [SerializeField] private Slider courageSlider;
        [SerializeField] private TextMeshProUGUI courageLabel;

        [Header("XP")]
        [SerializeField] private Slider xpSlider;
        [SerializeField] private TextMeshProUGUI xpText;

        private void Start()
        {
            EnsureCanvasVisible();
            AutoFindReferences();
        }

        private void EnsureCanvasVisible()
        {
            var rect = GetComponent<RectTransform>();
            if (!rect) return;
            var canvas = rect.GetComponentInParent<Canvas>();
            if (canvas && canvas.transform is RectTransform canvasRect)
                canvasRect.localScale = Vector3.one;
        }

        private void AutoFindReferences()
        {
            if (!courageSlider) courageSlider = FindChildSlider("UltSlider");
            if (!courageLabel) courageLabel = FindChildTMP("UltLabel");
            if (!xpSlider) xpSlider = FindChildSlider("XPSlider");
            if (!xpText) xpText = FindChildTMP("XPText");

            if (courageSlider) { courageSlider.minValue = 0f; courageSlider.maxValue = 1f; courageSlider.wholeNumbers = false; }
            if (xpSlider) { xpSlider.minValue = 0f; xpSlider.maxValue = 1f; xpSlider.wholeNumbers = false; }
        }

        private Slider FindChildSlider(string childName)
        {
            var t = transform.Find(childName);
            return t ? t.GetComponent<Slider>() : null;
        }

        private TextMeshProUGUI FindChildTMP(string childName)
        {
            var t = transform.Find(childName);
            return t ? t.GetComponent<TextMeshProUGUI>() : null;
        }

        private void Update()
        {
            UpdateCourage();
            UpdateXp();
        }

        private void UpdateCourage()
        {
            if (!courageSlider && !courageLabel) return;
            var charge = 0f;
            if (UltimateManager.Instance != null && UltimateManager.Instance.isActiveAndEnabled)
                charge = UltimateManager.Instance.NormalizedCharge;

            if (courageSlider) courageSlider.value = charge;
            if (courageLabel) courageLabel.text = $"Courage {(int)(charge * 100f)}%";
        }

        private void UpdateXp()
        {
            if (!xpSlider && !xpText) return;
            if (XpManager.Instance == null) return;

            var mgr = XpManager.Instance;
            if (xpSlider) xpSlider.value = mgr.NormalizedXp;
            if (xpText) xpText.text = $"{mgr.XpTillNext} xp till next upgrade";
        }
    }
}
