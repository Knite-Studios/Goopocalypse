using UnityEditor;
using UnityEngine;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using Achievements;

namespace Achievements
{
    public class AchievementsDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Achievements Debug")]
        private static void OpenMenu() => GetWindow<AchievementsDebug>().Show();

        // Achievement selection
        private string selectedAchievementID;
        private string[] achievementIDs;
        private int selectedAchievementIndex = 0;

        private void OnEnable()
        {
            // Load all achievements
            if (AchievementsManager.Achievements != null)
            {
                // Get all achievement IDs
                achievementIDs = AchievementsManager.Achievements.Keys.ToArray();
                // Set the selected achievement ID
                selectedAchievementID = achievementIDs.Length > 0 ? achievementIDs[0] : null;
            }
        }

        private void OnGUI()
        {
            // Achievements Debugging
            EditorGUILayout.LabelField("Achievements Debugging", EditorStyles.boldLabel);

            if (achievementIDs != null && achievementIDs.Length > 0)
            {
                // Set the selected achievement ID
                selectedAchievementIndex = EditorGUILayout.Popup("Select Achievement", selectedAchievementIndex, achievementIDs);
                // Set the selected achievement ID
                selectedAchievementID = achievementIDs[selectedAchievementIndex];
            }

            if (GUILayout.Button("Unlock Achievement"))
            {
                // Unlock the selected achievement
                if (SteamManager.Initialized && selectedAchievementID != null)
                {
                    // Unlock the achievement
                    SteamUserStats.SetAchievement(selectedAchievementID);
                    // Store the stats
                    SteamUserStats.StoreStats();
                    // Log the achievement unlock
                    Debug.Log($"Achievement Unlocked: {selectedAchievementID}");
                }
            }

            if (GUILayout.Button("Lock Achievement"))
            {
                // Lock the selected achievement
                if (SteamManager.Initialized && selectedAchievementID != null)
                {
                    // Lock the achievement
                    SteamUserStats.ClearAchievement(selectedAchievementID);
                    // Store the stats
                    SteamUserStats.StoreStats();
                    // Log the achievement lock
                    Debug.Log($"Achievement Locked: {selectedAchievementID}");
                }
            }
        }
    }
}

