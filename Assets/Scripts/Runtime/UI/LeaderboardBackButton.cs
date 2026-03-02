using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Attach to a Back button in the Leaderboard scene (02_Leaderboard).
    /// On click, loads the menu scene (00_Menu). Ensure 00_Menu is in Build Settings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LeaderboardBackButton : MonoBehaviour
    {
        [SerializeField] private string menuSceneName = "00_Menu";

        private void Awake()
        {
            var button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(LoadMenu);
        }

        private void LoadMenu()
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
