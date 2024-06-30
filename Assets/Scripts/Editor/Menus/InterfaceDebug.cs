using Managers;
using UnityEditor;

namespace Editor
{
    public class InterfaceDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/User Interface Debug")]
        private static void OpenMenu() => GetWindow<InterfaceDebug>().Show();

        private void OnGUI()
        {
            GameManager.Instance.State = (GameState) EditorGUILayout.EnumPopup(
                "UI State", GameManager.Instance.State);

            GameManager.Instance.Route = EditorGUILayout.TextField(
                "Route", GameManager.Instance.Route);
        }
    }
}
