using System;
using Entity.Player;
using UnityEngine;

namespace Discord
{
    public class DiscordController : MonoSingleton<DiscordController>
    {
        private const long ApplicationId = 1255981877186002964;
        private const string LargeImageKey = "main_logo";
        private const string LargeImageText = "Fwend and Buddie";
        private const string FwendImageKey = "fwend_icon";
        private const string FwendImageText = "Fwend";
        private const string BuddieImageKey = "buddie_icon";
        private const string BuddieImageText = "Buddie";

        private string _smallImageKey;
        private string _smallImageText;
        private PlayerRole _playerRole;
        private Discord _discord;
        private ActivityManager _activityManager;
        private long _startTime;

        protected override void OnAwake()
        {
            _discord = new Discord(ApplicationId, (ulong)CreateFlags.Default);
            _activityManager = _discord.GetActivityManager();

            _startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdateActivity();
        }

        private void Update()
        {
            _discord.RunCallbacks();
        }

        private void LateUpdate()
        {
            UpdateActivity();
        }

        private void OnApplicationQuit()
        {
            _discord.Dispose();
        }

        private void UpdateActivity()
        {
            var playingState = GetSceneDetails().Contains("Main Menu")
                ? ""
                : string.IsNullOrEmpty(GetRoleText())
                    ? ""
                    : $"Now Playing as {GetRoleText()}";
            var activity = new Activity()
            {
                Details = GetSceneDetails(),
                State = playingState,
                Timestamps =
                {
                    Start = _startTime
                },
                Assets =
                {
                    LargeImage = LargeImageKey,
                    LargeText = LargeImageText,
                    // TODO: Add check if the player is in game.
                    SmallImage = _smallImageKey,
                    SmallText = _smallImageText
                }
            };

            _activityManager.UpdateActivity(activity, result =>
            {
                if (result != Result.Ok)
                    Debug.LogError($"Failed to update activity: {result}");
            });
        }

        private string GetRoleText()
        {
            return _playerRole switch
            {
                PlayerRole.Fwend => "Fwend",
                PlayerRole.Buddie => "Buddie",
                _ => string.Empty
            };
        }

        public void SetSmallImage(PlayerRole role)
        {
            _playerRole = role;
            switch (role)
            {
                case PlayerRole.Fwend:
                    _smallImageKey = FwendImageKey;
                    _smallImageText = FwendImageText;
                    break;
                case PlayerRole.Buddie:
                    _smallImageKey = BuddieImageKey;
                    _smallImageText = BuddieImageText;
                    break;
                case PlayerRole.None:
                default:
                    _smallImageKey = null;
                    _smallImageText = null;
                    break;
            }
        }

        private string GetSceneDetails()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            return activeScene.buildIndex switch
            {
                0 => "In the Main Menu",
                // TODO: Maybe use ternary operator if playing Local MP or CO-OP.
                1 => "In Game",
                _ => "In the Main Menu"
            };
        }
    }
}
