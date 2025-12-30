using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// UI component for a single rebindable keybind entry.
/// Shows action name, current binding, and allows rebinding on click.
/// </summary>
public class KeybindEntryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text actionNameText;
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private GameObject waitingOverlay;

    [Header("Settings")]
    [SerializeField] private string waitingText = "Press any key...";
    [SerializeField] private Color conflictColor = new Color(1f, 0.5f, 0.5f);

    private InputAction _action;
    private int _bindingIndex = -1;
    private string _controlScheme = "Keyboard";
    private Color _defaultTextColor;

    private void Awake()
    {
        if (bindingText != null)
            _defaultTextColor = bindingText.color;

        if (waitingOverlay != null)
            waitingOverlay.SetActive(false);
    }

    private void OnEnable()
    {
        if (rebindButton != null)
            rebindButton.onClick.AddListener(StartRebind);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefault);

        InputManager.OnRebindComplete += OnRebindComplete;
        InputManager.OnRebindCanceled += OnRebindCanceled;

        UpdateDisplay();
    }

    private void OnDisable()
    {
        if (rebindButton != null)
            rebindButton.onClick.RemoveListener(StartRebind);
        if (resetButton != null)
            resetButton.onClick.RemoveListener(ResetToDefault);

        InputManager.OnRebindComplete -= OnRebindComplete;
        InputManager.OnRebindCanceled -= OnRebindCanceled;

        // Cancel any in-progress rebind when disabled
        if (InputManager.IsRebinding)
            InputManager.CancelRebind();
    }

    /// <summary>
    /// Initializes the entry with an input action.
    /// </summary>
    public void Initialize(InputAction action, string controlScheme = "Keyboard")
    {
        _action = action;
        _controlScheme = controlScheme;
        _bindingIndex = -1;

        if (actionNameText != null && action != null)
            actionNameText.text = FormatActionName(action.name);

        UpdateDisplay();
    }

    /// <summary>
    /// Initializes the entry with an input action and specific binding index.
    /// </summary>
    public void Initialize(InputAction action, int bindingIndex)
    {
        _action = action;
        _bindingIndex = bindingIndex;

        if (actionNameText != null && action != null)
            actionNameText.text = FormatActionName(action.name);

        UpdateDisplay();
    }

    private string FormatActionName(string name)
    {
        // Convert camelCase/PascalCase to spaced words
        var result = System.Text.RegularExpressions.Regex.Replace(
            name,
            "([a-z])([A-Z])",
            "$1 $2"
        );
        return result;
    }

    private void UpdateDisplay()
    {
        if (_action == null || bindingText == null) return;

        string displayString;
        if (_bindingIndex >= 0)
        {
            displayString = _action.GetBindingDisplayString(_bindingIndex);
        }
        else
        {
            displayString = InputManager.GetBindingDisplayString(_action, _controlScheme);
        }

        bindingText.text = string.IsNullOrEmpty(displayString) ? "None" : displayString;
        bindingText.color = _defaultTextColor;

        // Check for conflicts
        if (_bindingIndex >= 0 && InputManager.HasBindingConflict(_action, _bindingIndex, out var conflict))
        {
            bindingText.color = conflictColor;
        }
    }

    private void StartRebind()
    {
        if (_action == null) return;
        if (InputManager.IsRebinding) return;

        // Show waiting state
        if (waitingOverlay != null)
            waitingOverlay.SetActive(true);
        if (bindingText != null)
            bindingText.text = waitingText;
        if (rebindButton != null)
            rebindButton.interactable = false;

        InputManager.StartRebind(_action, _bindingIndex, _controlScheme);
    }

    private void OnRebindComplete(InputAction action, int bindingIndex)
    {
        if (action != _action) return;

        if (_bindingIndex < 0)
            _bindingIndex = bindingIndex;

        EndRebindState();
        UpdateDisplay();
    }

    private void OnRebindCanceled(InputAction action)
    {
        if (action != _action) return;

        EndRebindState();
        UpdateDisplay();
    }

    private void EndRebindState()
    {
        if (waitingOverlay != null)
            waitingOverlay.SetActive(false);
        if (rebindButton != null)
            rebindButton.interactable = true;
    }

    private void ResetToDefault()
    {
        if (_action == null) return;

        InputManager.ResetBinding(_action);
        _bindingIndex = -1;
        UpdateDisplay();
    }
}
