using System.Collections.Generic;
using Entity.Player;
using Managers;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreenController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playCoopButton;
    [SerializeField] private Button inviteButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;

    [Header("Player List")]
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerEntryPrefab;

    [Header("Status")]
    [SerializeField] private TMP_Text statusText;

    private readonly List<PlayerEntryUI> _playerEntries = new();

    private void OnEnable()
    {
        LobbyManager.OnPlayersChanged += OnPlayersChanged;
        LobbyManager.OnRolesChanged += OnRolesChanged;

        if (playCoopButton != null) playCoopButton.onClick.AddListener(OnPlayCoopClicked);
        if (inviteButton != null) inviteButton.onClick.AddListener(OnInviteClicked);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameClicked);
        if (backButton != null) backButton.onClick.AddListener(OnBackClicked);

        UpdateButtonStates();
        RefreshPlayerList();
    }

    private void OnDisable()
    {
        LobbyManager.OnPlayersChanged -= OnPlayersChanged;
        LobbyManager.OnRolesChanged -= OnRolesChanged;

        if (playCoopButton != null) playCoopButton.onClick.RemoveListener(OnPlayCoopClicked);
        if (inviteButton != null) inviteButton.onClick.RemoveListener(OnInviteClicked);
        if (startGameButton != null) startGameButton.onClick.RemoveListener(OnStartGameClicked);
        if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void Update()
    {
        UpdateButtonStates();

        // Keyboard shortcut for invite (I key)
        if (Input.GetKeyDown(KeyCode.I))
        {
            var isHost = Managers.NetworkManager.IsHost();
            var playerCount = LobbyManager.HasInstance() ? LobbyManager.Instance.Players.Count : 0;
            if (isHost && playerCount < 2)
            {
                OnInviteClicked();
            }
        }

        // Keyboard shortcut to start game (Enter key)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var isHost = Managers.NetworkManager.IsHost();
            var playerCount = LobbyManager.HasInstance() ? LobbyManager.Instance.Players.Count : 0;
            if (isHost && playerCount == 2)
            {
                OnStartGameClicked();
            }
        }

        // Escape to go back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    private void UpdateButtonStates()
    {
        var isConnected = NetworkClient.isConnected || NetworkServer.active;
        var isHost = Managers.NetworkManager.IsHost();
        var playerCount = LobbyManager.HasInstance() ? LobbyManager.Instance.Players.Count : 0;

        if (playCoopButton != null)
            playCoopButton.gameObject.SetActive(!isConnected);
        if (inviteButton != null)
            inviteButton.gameObject.SetActive(isHost && playerCount < 2);
        if (startGameButton != null)
            startGameButton.gameObject.SetActive(isHost && playerCount == 2);

        if (statusText != null)
        {
            if (!isConnected)
                statusText.text = "Click 'Play Co-op' to create a lobby";
            else if (isHost && playerCount < 2)
                statusText.text = "Press I to invite a friend";
            else if (isHost)
                statusText.text = "Press Enter to start!";
            else
                statusText.text = "Waiting for host to start...";
        }
    }

    public void OnPlayCoopClicked()
    {
        LobbyManager.Instance.MakeLobby();
    }

    public void OnInviteClicked()
    {
        LobbyManager.Instance.InvitePlayer();
    }

    public void OnStartGameClicked()
    {
        GameManager.Instance.StartRemoteGame();
    }

    public void OnBackClicked()
    {
        if (NetworkClient.isConnected || NetworkServer.active)
        {
            LobbyManager.Instance.LeaveLobby();
        }
        UIManager.Instance.ShowMainMenu();
    }

    private void OnPlayersChanged(List<PlayerSession> players)
    {
        RefreshPlayerList();
        UpdateButtonStates();
    }

    private void OnRolesChanged(Dictionary<string, PlayerRole> roles)
    {
        UpdatePlayerRoles();
    }

    private void RefreshPlayerList()
    {
        foreach (var entry in _playerEntries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }
        _playerEntries.Clear();

        if (!LobbyManager.HasInstance()) return;

        var players = LobbyManager.Instance.Players;
        var roles = LobbyManager.Instance.Roles;

        foreach (var player in players)
        {
            if (playerEntryPrefab == null || playerListContainer == null) continue;

            var entryObj = Instantiate(playerEntryPrefab, playerListContainer);
            var entry = entryObj.GetComponent<PlayerEntryUI>();

            if (entry != null)
            {
                var role = roles.TryGetValue(player.userId, out var r) ? r : PlayerRole.None;
                entry.Setup(player, role);
                _playerEntries.Add(entry);
            }
        }
    }

    private void UpdatePlayerRoles()
    {
        if (!LobbyManager.HasInstance()) return;

        var roles = LobbyManager.Instance.Roles;

        foreach (var entry in _playerEntries)
        {
            if (entry == null) continue;
            if (roles.TryGetValue(entry.UserId, out var role))
            {
                entry.UpdateRole(role);
            }
        }
    }
}
