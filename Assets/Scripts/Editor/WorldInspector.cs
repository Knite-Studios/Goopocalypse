using Runtime.World;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(World))]
    public class WorldInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate World"))
            {
                var world = target as World;
                world!.Initialize();
            }
        }
    }
}
