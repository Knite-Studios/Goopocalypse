using Managers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class InterfaceDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/User Interface Debug")]
        private static void OpenMenu() => GetWindow<InterfaceDebug>().Show();

        private string _proposedState = "/";

        private void OnGUI()
        {
            GameManager.Instance.State = (GameState) EditorGUILayout.EnumPopup(
                "UI State", GameManager.Instance.State);

            _proposedState = EditorGUILayout.TextField("Proposed State", _proposedState);

            if (GUILayout.Button("Submit State"))
            {
                GameManager.Instance.Navigate(_proposedState);
            }
        }
    }
}
