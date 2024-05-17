using Managers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class WaveDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Wave Debug")]
        private static void OpenMenu() => GetWindow<WaveDebug>().Show();

        private void OnGUI()
        {
            if (GUILayout.Button("Start Game"))
            {
                GameManager.OnGameStart?.Invoke();
            }

            if (GUILayout.Button("Spawn Chest At Player"))
            {
                var gameEvent = new GameEvent
                {
                    Type = GameEventType.ChestSpawned,
                    Target = GameObject.Find("Player [connId=0]").transform
                };
                GameManager.OnGameEvent?.Invoke(gameEvent);
            }
        }
    }
}