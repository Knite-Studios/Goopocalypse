using Managers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class InterfaceDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/User Interface Debug")]
        private static void OpenMenu() => GetWindow<InterfaceDebug>().Show();

        private void OnGUI()
        {
            if (!Application.isPlaying || !GameManager.HasInstance())
            {
                EditorGUILayout.HelpBox(
                    "This is only available in Play Mode.",
                    MessageType.Info);
                return;
            }

            GameManager.Instance.State = (GameState) EditorGUILayout.EnumPopup(
                "UI State", GameManager.Instance.State);

            GameManager.Instance.Route = EditorGUILayout.TextField(
                "Route", GameManager.Instance.Route);

            EditorGUILayout.HelpBox(
                "Route is case-sensitive, be careful!",
                MessageType.Info);
        }
    }
}
