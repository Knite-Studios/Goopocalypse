using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the credits screen.
/// Attach to CreditsScreen GameObject in the scene.
/// </summary>
public class CreditsScreenController : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button backButton;

    private void OnEnable()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnDisable()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void Update()
    {
        // Escape to go back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    private void OnBackClicked()
    {
        if (UIManager.HasInstance())
            UIManager.Instance.ShowMainMenu();
    }
}
