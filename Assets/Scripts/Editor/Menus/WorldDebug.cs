using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class WorldDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/World Debug")]
        private static void OpenMenu() => GetWindow<WorldDebug>().Show();

        private float _threshold = 0.6f, _multiplier = 0.1f;
        private Vector2Int _worldSize;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("World Debugging");

            _threshold = EditorGUILayout.Slider("Threshold", _threshold, 0, 1);
            _multiplier = EditorGUILayout.Slider("Multiplier", _multiplier, 0, 1);

            GUILayout.Space(10);

            _worldSize.x = EditorGUILayout.IntField("X Size", _worldSize.x);
            _worldSize.y = EditorGUILayout.IntField("Y Size", _worldSize.y);

            if (GUILayout.Button("Generate World"))
            {
                var squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png");

                var parent = GameObject.Find("WorldRoot");
                if (parent)
                {
                    DestroyImmediate(parent);
                }

                parent = new GameObject("WorldRoot");
                for (var x = 0; x < _worldSize.x; x++)
                {
                    for (var y = 0; y < _worldSize.y; y++)
                    {
                        var noise = Mathf.PerlinNoise(x * _multiplier, y * _multiplier);
                        ColorUtility.TryParseHtmlString("#222222", out var groundColor);
                        var isObstacle = noise > _threshold;
                        var color = isObstacle ? Color.gray : groundColor;

                        var tile = new GameObject($"Tile {x}, {y}", typeof(SpriteRenderer))
                        {
                            transform =
                            {
                                position = new Vector3(x + 0.5f, y + 0.5f)
                            },
                            layer = LayerMask.NameToLayer(isObstacle ? "Obstacle" : "Walkable")
                        };
                        tile.transform.SetParent(parent.transform);

                        var renderer = tile.GetComponent<SpriteRenderer>();
                        renderer.color = color;
                        renderer.sprite = squareSprite;
                        renderer.sortingOrder = -10;

                        if (isObstacle)
                        {
                            tile.AddComponent<BoxCollider2D>();

                            var rigidBody = tile.AddComponent<Rigidbody2D>();
                            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                    }
                }
            }
        }
    }
}
