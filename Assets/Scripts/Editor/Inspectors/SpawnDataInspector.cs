using System.Linq;
using Scriptable;
using UnityEditor;
using UnityEngine;

namespace Editor.Inspectors
{
    [CustomEditor(typeof(SpawnData))]
    public class SpawnDataInspector : UnityEditor.Editor
    {
        private SpawnData _target => target as SpawnData;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Restore From Asset"))
            {
                var root = new GameObject("- Spawn Points");

                // Create a GameObject for each point.
                foreach (var point in _target.points)
                {
                    var obj = new GameObject($"Spawn {point.x}, {point.y}")
                    {
                        transform =
                        {
                            position = point
                        },
                        tag = "SpawnPoint"
                    };
                    obj.transform.SetParent(root.transform);
                }
            }

            if (GUILayout.Button("Save To Asset"))
            {
                // Find all GameObjects tagged with 'SpawnPoint'.
                var objects = GameObject.FindGameObjectsWithTag("SpawnPoint");

                // Convert each into a Vector2.
                var points = objects
                    .Select(gameObj => gameObj.transform.position)
                    .Select(dummy => (Vector2)dummy).ToList();

                // Set the points to the target.
                _target.points = points;

                // Delete all the GameObjects.
                foreach (var obj in objects)
                {
                    DestroyImmediate(obj);
                }
            }
        }
    }
}
