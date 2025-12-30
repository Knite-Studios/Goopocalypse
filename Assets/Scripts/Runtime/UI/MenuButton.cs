using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Adds DOTween hover and press animations to menu buttons.
/// Attach this component to any Button to get polished interactions.
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutQuad;

    [Header("Audio")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    private Button _button;
    private Vector3 _originalScale;
    private Tween _currentTween;
    private bool _isHovered;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _originalScale = transform.localScale;
        _button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        _isHovered = true;
        _currentTween?.Kill();
        _currentTween = transform.DOScale(_originalScale * hoverScale, duration)
            .SetEase(hoverEase)
            .SetUpdate(true);

        if (playHoverSound && AudioManager.HasInstance())
            AudioManager.Instance.PlayUIHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        _currentTween?.Kill();
        _currentTween = transform.DOScale(_originalScale, duration * 0.67f)
            .SetEase(Ease.InQuad)
            .SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        _currentTween?.Kill();
        _currentTween = transform.DOScale(_originalScale * pressScale, duration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        _currentTween?.Kill();

        // Return to hover scale if still hovered, otherwise original
        var targetScale = _isHovered ? _originalScale * hoverScale : _originalScale;
        _currentTween = transform.DOScale(targetScale, duration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    private void OnClick()
    {
        if (playClickSound && AudioManager.HasInstance())
            AudioManager.Instance.PlayUIClickSound();
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
        if (_button != null)
            _button.onClick.RemoveListener(OnClick);
    }

    /// <summary>
    /// Reset scale when button becomes disabled.
    /// </summary>
    private void OnDisable()
    {
        _currentTween?.Kill();
        transform.localScale = _originalScale;
        _isHovered = false;
    }
}
