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
        }
    }
}