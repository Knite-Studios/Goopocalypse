using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Shows the game over overlay when GameState transitions to GameOver.
    /// Assign gameOverPanel, scoreText, restartButton, quitButton in the Inspector for a custom layout.
    /// If not assigned, a minimal panel is created at runtime so game over always displays.
    /// </summary>
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            EnsureGameOverPanel();
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestart);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);
        }

        /// <summary>
        /// If the game over panel or refs were not set in the scene, create a minimal panel at runtime.
        /// </summary>
        private void EnsureGameOverPanel()
        {
            if (gameOverPanel != null) return;

            var canvas = GetComponent<RectTransform>();
            if (canvas == null) return;

            var panelGo = new GameObject("GameOverPanel");
            panelGo.transform.SetParent(transform, false);

            var rect = panelGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = panelGo.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.85f);

            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(panelGo.transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.7f);
            titleRect.sizeDelta = new Vector2(400, 80);
            titleRect.anchoredPosition = Vector2.zero;
            var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "GAME OVER";
            titleTmp.fontSize = 48;
            titleTmp.alignment = TextAlignmentOptions.Center;

            var scoreGo = new GameObject("ScoreText");
            scoreGo.transform.SetParent(panelGo.transform, false);
            var scoreRect = scoreGo.AddComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
            scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
            scoreRect.sizeDelta = new Vector2(300, 40);
            scoreRect.anchoredPosition = Vector2.zero;
            var scoreTmp = scoreGo.AddComponent<TextMeshProUGUI>();
            scoreTmp.text = "Score: 0";
            scoreTmp.fontSize = 28;
            scoreTmp.alignment = TextAlignmentOptions.Center;

            var buttonsGo = new GameObject("Buttons");
            buttonsGo.transform.SetParent(panelGo.transform, false);
            var buttonsRect = buttonsGo.AddComponent<RectTransform>();
            buttonsRect.anchorMin = new Vector2(0.5f, 0.25f);
            buttonsRect.anchorMax = new Vector2(0.5f, 0.25f);
            buttonsRect.sizeDelta = new Vector2(320, 60);
            buttonsRect.anchoredPosition = Vector2.zero;

            var restartGo = CreateButton("Restart", buttonsRect, new Vector2(-80, 0));
            var quitGo = CreateButton("Quit", buttonsRect, new Vector2(80, 0));

            gameOverPanel = panelGo;
            scoreText = scoreTmp;
            restartButton = restartGo.GetComponent<Button>();
            quitButton = quitGo.GetComponent<Button>();
        }

        private static GameObject CreateButton(string label, RectTransform parent, Vector2 anchoredPos)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(140, 44);
            rect.anchoredPosition = anchoredPos;
            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.25f, 0.35f, 1f);
            var button = go.AddComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
            button.colors = colors;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 22;
            tmp.alignment = TextAlignmentOptions.Center;
            return go;
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += ShowGameOver;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= ShowGameOver;
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (scoreText != null && WaveManager.HasInstance())
                scoreText.text = $"Score: {WaveManager.Instance.Score}";
        }

        private void OnRestart()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            Time.timeScale = 1f;

            if (GameManager.HasInstance())
                GameManager.Instance.RestartGame();
        }

        private void OnQuit()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            Time.timeScale = 1f;

            if (GameManager.HasInstance())
                GameManager.Instance.StopGame();
        }

        private void OnDestroy()
        {
            if (restartButton != null)
                restartButton.onClick.RemoveListener(OnRestart);
            if (quitButton != null)
                quitButton.onClick.RemoveListener(OnQuit);
        }
    }
}
