using System.Collections.Generic;
using Entity.Player;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the lobby panel.
/// Handles both Local and Online coop modes.
/// </summary>
public class LobbyScreenController : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] private RawImage player1Image;
    [SerializeField] private TMP_Text player1StatusText;

    [Header("Player 2")]
    [SerializeField] private RawImage player2Image;
    [SerializeField] private TMP_Text player2StatusText;

    [Header("Buttons")]
    [SerializeField] private Button player1Button;
    [SerializeField] private Button player2Button;
    [SerializeField] private Button backButton;
    [SerializeField] private Button startGameButton;

    [Header("Default Texture")]
    [SerializeField] private Texture2D defaultPlayerTexture;

    [Header("Input Settings")]
    [SerializeField] private KeyCode player1JoinKey = KeyCode.Space;
    [SerializeField] private KeyCode player2JoinKey = KeyCode.Return;
    [SerializeField] private KeyCode inviteKey = KeyCode.I;

    private MenuController _menuController;
    private bool _player1Joined;
    private bool _player2Joined;

    private void Awake()
    {
        _menuController = FindObjectOfType<MenuController>();
    }

    private void OnEnable()
    {
        if (player1Button != null)
            player1Button.onClick.AddListener(OnPlayer1ButtonClicked);
        if (player2Button != null)
            player2Button.onClick.AddListener(OnPlayer2ButtonClicked);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnStartGameClicked);

        // Subscribe to lobby events for online mode
        LobbyManager.OnPlayersChanged += OnPlayersChanged;

        ResetState();
        UpdateUI();
    }

    private void OnDisable()
    {
        if (player1Button != null)
            player1Button.onClick.RemoveListener(OnPlayer1ButtonClicked);
        if (player2Button != null)
            player2Button.onClick.RemoveListener(OnPlayer2ButtonClicked);
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGameClicked);

        LobbyManager.OnPlayersChanged -= OnPlayersChanged;
    }

    private void Update()
    {
        // Escape to go back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
            return;
        }

        // Player 1 join (local mode or host in online)
        if (!_player1Joined && Input.GetKeyDown(player1JoinKey))
        {
            OnPlayer1Join();
        }

        // Player 2 input
        if (!_player2Joined)
        {
            if (IsOnlineMode())
            {
                // Online: Press to invite
                if (Input.GetKeyDown(inviteKey))
                {
                    OnInvitePressed();
                }
            }
            else
            {
                // Local: Press to join or detect controller
                if (Input.GetKeyDown(player2JoinKey))
                {
                    OnPlayer2Join();
                }
                // Also check for second controller input
                else if (InputManager.Movement2 != null &&
                         InputManager.Movement2.ReadValue<Vector2>().magnitude > 0.5f)
                {
                    OnPlayer2Join();
                }
            }
        }
    }

    private void ResetState()
    {
        _player1Joined = false;
        _player2Joined = false;

        // In online mode, check if we're already connected
        if (IsOnlineMode() && LobbyManager.HasInstance())
        {
            var players = LobbyManager.Instance.Players;
            _player1Joined = players.Count >= 1;
            _player2Joined = players.Count >= 2;
        }
    }

    private bool IsOnlineMode()
    {
        return _menuController != null && _menuController.IsOnlineMode;
    }

    private void UpdateUI()
    {
        bool isOnline = IsOnlineMode();

        // Player 1 status text and button
        if (player1StatusText != null)
        {
            if (_player1Joined)
            {
                player1StatusText.text = "Connected";
            }
            else
            {
                player1StatusText.text = "Join";
            }
        }
        if (player1Button != null)
        {
            player1Button.interactable = !_player1Joined;
        }

        // Player 2 status text and button
        if (player2StatusText != null)
        {
            if (_player2Joined)
            {
                player2StatusText.text = "Connected";
            }
            else if (isOnline)
            {
                player2StatusText.text = "Invite";
            }
            else
            {
                player2StatusText.text = "Join";
            }
        }
        if (player2Button != null)
        {
            player2Button.interactable = !_player2Joined;
        }

        // Start Game: only interactable when both players are ready (no auto-start)
        if (startGameButton != null)
        {
            startGameButton.interactable = _player1Joined && _player2Joined;
        }

        // Player images
        if (isOnline)
        {
            UpdateOnlinePlayerImages();
        }
        else
        {
            if (player1Image != null)
                player1Image.texture = defaultPlayerTexture;
            if (player2Image != null)
                player2Image.texture = defaultPlayerTexture;
        }

        // No auto-start: Start Game button is shown and enabled only when both ready (see startGameButton.interactable above).
    }

    private void UpdateOnlinePlayerImages()
    {
        if (!LobbyManager.HasInstance()) return;

        var players = LobbyManager.Instance.Players;

        if (players.Count > 0 && player1Image != null)
        {
            player1Image.texture = players[0].profileIcon ?? defaultPlayerTexture;
        }
        else if (player1Image != null)
        {
            player1Image.texture = defaultPlayerTexture;
        }

        if (players.Count > 1 && player2Image != null)
        {
            player2Image.texture = players[1].profileIcon ?? defaultPlayerTexture;
        }
        else if (player2Image != null)
        {
            player2Image.texture = defaultPlayerTexture;
        }
    }

    private void OnPlayersChanged(List<PlayerSession> players)
    {
        // Update join states based on player count (online mode)
        _player1Joined = players.Count >= 1;
        _player2Joined = players.Count >= 2;
        UpdateUI();
    }

    private void OnPlayer1Join()
    {
        _player1Joined = true;
        UpdateUI();
    }

    private void OnPlayer2Join()
    {
        _player2Joined = true;
        UpdateUI();
    }

    /// <summary>
    /// Called when Player 1 button is clicked.
    /// </summary>
    private void OnPlayer1ButtonClicked()
    {
        if (!_player1Joined)
        {
            OnPlayer1Join();
        }
    }

    /// <summary>
    /// Called when Player 2 button is clicked.
    /// In local mode: joins P2. In online mode: opens invite.
    /// </summary>
    private void OnPlayer2ButtonClicked()
    {
        if (_player2Joined) return;

        if (IsOnlineMode())
        {
            OnInvitePressed();
        }
        else
        {
            OnPlayer2Join();
        }
    }

    private void OnInvitePressed()
    {
        // Opens Steam overlay to invite friends
        if (_menuController != null)
            _menuController.InvitePlayer();
    }

    private void OnBackClicked()
    {
        CancelInvoke(nameof(StartGame));
        if (_menuController != null)
            _menuController.ReturnFromLobby();
    }

    private void OnStartGameClicked()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (_menuController != null)
            _menuController.StartGame();
    }
}
