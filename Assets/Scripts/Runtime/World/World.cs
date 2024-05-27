using System;
using System.Linq;
using Attributes;
using Entity.Pathfinding;
using Managers;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;
using Random = System.Random;

namespace Runtime.World
{
    public class World : MonoBehaviour
    {
        [SerializeField] public int seed = 1234567890;
        [SerializeField] private bool generateOnStart;

        [TitleHeader("World Settings")]
        public int width = 100;
        public int height = 100;
        public float threshold = 0.6f;
        public float multiplier = 0.1f;

        [TitleHeader("Grid Settings")]
        public LayerMask unwalkable;
        public LayerMask walkable;
        public float nodeRadius = 0.5f;

        [NonSerialized] public Vector2 center;

        private Grid _grid;
        private float _nodeDiameter;

        private int _tileOffset;

        private void Awake()
        {
            _nodeDiameter = nodeRadius * 2;
            _grid = gameObject.GetOrAddComponent<Grid>();

            // Set grid properties.
            _grid.width = width;
            _grid.height = height;
            _grid.unwalkableLayer = unwalkable;
            _grid.walkableLayer = walkable;
            _grid.nodeRadius = nodeRadius;
            _grid.nodeDiameter = _nodeDiameter;

            // Generate a random offset for the tiles.
            if (seed == -1) seed = (int) DateTime.Now.Ticks;
            _tileOffset = new Random(seed).Next(0, 1000);
        }

        private void Start()
        {
            if (!generateOnStart) return;

            Generate();
        }

        /// <summary>
        /// Determines if a point is walkable.
        /// </summary>
        public bool IsWalkable(Vector2 position) => _grid.GetNode(position).isWalkable;

        /// <summary>
        /// Initialize the grid.
        /// </summary>
        public void Generate()
        {
            var worldRoot = gameObject.transform;
            // Clear the world.
            worldRoot.transform.Cast<Transform>()
                .ToList()
                .ForEach(trans => Destroy(trans.gameObject));

            var squareSprite = Resources.Load<Sprite>("Art/Textures/Square");

            // Generate the world.
            var nodes = new Node[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var noise = Mathf.PerlinNoise(
                        (x + _tileOffset) * multiplier,
                        (y + _tileOffset) * multiplier);

                    var isObstacle = noise > threshold;
                    var color = isObstacle ? Color.gray : Color.white;

                    // Create the tile.
                    var position = new Vector2(x, y);
                    var tile = new GameObject($"Tile {x}, {y}")
                    {
                        transform = { position = position }
                    };
                    tile.transform.SetParent(worldRoot);

                    // Update the renderer.
                    var tileRenderer = tile.AddComponent<SpriteRenderer>();
                    tileRenderer.color = color;
                    tileRenderer.sprite = squareSprite;
                    tileRenderer.sortingOrder = -1;

                    // Add a collider and rigid body if it's an obstacle.
                    if (isObstacle)
                    {
                        tile.AddComponent<BoxCollider2D>();
                        var rigidBody = tile.AddComponent<Rigidbody2D>();
                        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                    }

                    // Create the node.
                    var worldPoint = new Vector2(
                        x * _nodeDiameter + nodeRadius,
                        y * _nodeDiameter + nodeRadius);
                    nodes[x, y] = new Node(position, worldPoint, !isObstacle);
                }
            }

            // Add a border to the world.
            DrawLine(1 + height, new Vector2(-1, -1), true, Color.black);
            DrawLine(1 + width, new Vector2(-1, -1), false, Color.black);
            DrawLine(2 + height, new Vector2(width, -1), true, Color.black);
            DrawLine(2 + width, new Vector2(-1, height), false, Color.black);

            // Determine the center.
            center = new Vector2(
                Mathf.RoundToInt(width / 2f),
                Mathf.RoundToInt(height / 2f));

            // Update the pathfinding grid.
            _grid.InitializeNodes(nodes);

            // Invoke world generation event.
            GameManager.OnWorldGenerated?.Invoke(this);

            // Try adding an entity spawner to the world object.
            gameObject.GetOrAddComponent<EntitySpawner>();
        }

        /// <summary>
        /// Draws a line of tiles.
        /// </summary>
        private void DrawLine(int length, Vector2 start, bool vertical, Color color)
        {
            var sprite = Resources.Load<Sprite>("Art/Textures/Square");

            for (var i = 0; i < length; i++)
            {
                var position = vertical
                    ? new Vector2(start.x, start.y + i)
                    : new Vector2(start.x + i, start.y);

                var tile = new GameObject($"Tile {position.x}, {position.y}")
                {
                    transform = { position = position }
                };
                tile.transform.SetParent(gameObject.transform);

                var tileRenderer = tile.AddComponent<SpriteRenderer>();
                tileRenderer.color = color;
                tileRenderer.sprite = sprite;
                tileRenderer.sortingOrder = -1;

                tile.AddComponent<BoxCollider2D>();
                var rigidBody = tile.AddComponent<Rigidbody2D>();
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }
}
