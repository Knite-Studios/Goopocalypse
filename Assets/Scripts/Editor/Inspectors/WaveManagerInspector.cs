using Managers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(WaveManager))]
    public class WaveManagerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Spawn Wave"))
            {
                var waveManager = target as WaveManager;
                waveManager!.SpawnWave();
            }
        }
    }
}
