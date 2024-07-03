using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Achievements
{
    public class AchievementsManager : MonoBehaviour
    {
        private float playtime = 0f;

        public static Dictionary<string, string> Achievements = new Dictionary<string, string>
        {
            {"ACH_MISTAKE", "A mistake: Open the game"},
            {"ACH_FRIENDS", "You have friends? Play multiplayer first time"},
            {"ACH_NOLIFE1", "No Life 1: Play for 10 minutes"},
            {"ACH_NOLIFE2", "No Life 2: Play for 1 hour"},
            {"ACH_NOLIFE3", "No Life 3: Play for 3 hours"}
        };

        private void Start()
        {
            if (SteamManager.Initialized)
            {
                // Load the stats at the start of the game
                SteamUserStats.RequestCurrentStats();
            }
        }

        private void Update()
        {
            if (!SteamManager.Initialized) return;

            // Track playtime
            playtime += Time.deltaTime;

            CheckPlaytimeAchievements();

            // Trigger "A mistake" when space is pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UnlockAchievement("ACH_MISTAKE");
            }

            // M key triggers "Friends" Achievement
            if (Input.GetKeyDown(KeyCode.M))
            {
                UnlockAchievement("ACH_FRIENDS");
            }
        }
        private void CheckPlaytimeAchievements()
        {
            bool isAchieved;

            // Check playtime achievements
            // 10 minutes
            if (playtime >= 10 * 60)
            {
                SteamUserStats.GetAchievement("ACH_NOLIFE1", out isAchieved);
                if (!isAchieved)
                {
                    UnlockAchievement("ACH_NOLIFE1");
                }
            }
            // 1 hour
            if (playtime >= 60 * 60)
            {
                SteamUserStats.GetAchievement("ACH_NOLIFE2", out isAchieved);
                if (!isAchieved)
                {
                    UnlockAchievement("ACH_NOLIFE2");
                }
            }
            // 3 hours
            if (playtime >= 180 * 60)
            {
                SteamUserStats.GetAchievement("ACH_NOLIFE3", out isAchieved);
                if (!isAchieved)
                {
                    UnlockAchievement("ACH_NOLIFE3");
                }
            }
        }

        private void UnlockAchievement(string achievementID)
        {
            SteamUserStats.SetAchievement(achievementID);
            // TO:DO: Optimise this to only store stats when needed
            SteamUserStats.StoreStats();
            Debug.Log($"Achievement Unlocked: {achievementID}");
        }
    }
}
