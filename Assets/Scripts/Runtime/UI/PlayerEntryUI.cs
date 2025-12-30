using Entity.Player;
using Managers;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntryUI : MonoBehaviour
{
    [SerializeField] private RawImage avatarImage;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Dropdown roleDropdown;

    public string UserId { get; private set; }

    private bool _isLocalPlayer;

    public void Setup(PlayerSession session, PlayerRole currentRole)
    {
        UserId = session.userId;

        if (avatarImage != null && session.profileIcon != null)
        {
            avatarImage.texture = session.profileIcon;
        }

        var displayName = GetDisplayName(session.userId);
        if (playerNameText != null)
        {
            playerNameText.text = displayName;
        }

        _isLocalPlayer = IsLocalPlayer(session.userId);

        if (roleDropdown != null)
        {
            roleDropdown.ClearOptions();
            roleDropdown.AddOptions(new System.Collections.Generic.List<string> { "Fwend", "Buddie" });

            roleDropdown.value = currentRole switch
            {
                PlayerRole.Fwend => 0,
                PlayerRole.Buddie => 1,
                _ => 0
            };

            roleDropdown.interactable = _isLocalPlayer;
            roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        }
    }

    public void UpdateRole(PlayerRole role)
    {
        if (roleDropdown == null) return;

        roleDropdown.SetValueWithoutNotify(role switch
        {
            PlayerRole.Fwend => 0,
            PlayerRole.Buddie => 1,
            _ => 0
        });
    }

    private void OnRoleChanged(int index)
    {
        if (!_isLocalPlayer) return;

        var newRole = index == 0 ? PlayerRole.Fwend : PlayerRole.Buddie;
        GameManager.Instance.ChangeRole(newRole);
    }

    private string GetDisplayName(string visitorId)
    {
#if !DISABLESTEAMWORKS
        if (SteamAPI.IsSteamRunning())
        {
            if (ulong.TryParse(UserId, out var steamIdValue))
            {
                var steamId = new CSteamID(steamIdValue);
                var name = SteamFriends.GetFriendPersonaName(steamId);
                if (!string.IsNullOrEmpty(name) && name != "[unknown]")
                {
                    return name;
                }
            }
        }
#endif
        return $"Player {UserId}";
    }

    private bool IsLocalPlayer(string visitorId)
    {
#if !DISABLESTEAMWORKS
        if (SteamAPI.IsSteamRunning())
        {
            var localSteamId = SteamUser.GetSteamID().m_SteamID.ToString();
            return UserId == localSteamId;
        }
#endif
        return LobbyManager.HasInstance() &&
               LobbyManager.Instance.Players.Count > 0 &&
               LobbyManager.Instance.Players[0].userId == UserId;
    }

    private void OnDestroy()
    {
        if (roleDropdown != null)
        {
            roleDropdown.onValueChanged.RemoveListener(OnRoleChanged);
        }
    }
}
