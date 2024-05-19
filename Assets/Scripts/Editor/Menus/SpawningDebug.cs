using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor
{
    public class SpawningDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Spawning Debug")]
        private static void OpenMenu() => GetWindow<SpawningDebug>().Show();

        private static float _distance = 5f;

        private void OnGUI()
        {
            if (!Camera.main)
            {
                GUILayout.Label("No main camera found.");
                return;
            }

            GUILayout.Label("Spawning Debugging");

            _distance = EditorGUILayout.FloatField("Distance", _distance);

            if (GUILayout.Button("Spawn Enemy"))
            {
                // use Random.InsideUnitCircle to get a random point within a circle
                // the point should not be in view of the camera.
                var radius = Camera.main.orthographicSize * _distance;
                var point = Random.insideUnitCircle.normalized * radius;
                var spawnPoint = new Vector3(point.x, point.y, 0);
                var enemy = new GameObject("Enemy")
                {
                    transform =
                    {
                        position = spawnPoint,
                        localScale = new Vector3(5, 5, 5)
                    },
                };
                var renderer = enemy.AddComponent<SpriteRenderer>();
                renderer.color = Color.red;
                renderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            }
        }
    }
}
