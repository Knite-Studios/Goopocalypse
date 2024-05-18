using Entity.Pathfinding;
using UnityEditor;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;

namespace Editor
{
    public class PathfinderDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Pathfinder Debug")]
        private static void OpenMenu() => GetWindow<PathfinderDebug>().Show();

        private Transform _targetPosition;
        private Vector2Int _gridDimensions = new Vector2Int(50, 50);
        private Pathfinder _pathfinder;
        private Grid _grid;
        private int _nodePadding = 1;

        private void OnGUI()
        {
            _targetPosition = EditorGUILayout.ObjectField(
                "Target Position",
                _targetPosition,
                typeof(Transform),
                true) as Transform;

            _gridDimensions = EditorGUILayout.Vector2IntField("Grid Dimensions", _gridDimensions);

            _pathfinder = EditorGUILayout.ObjectField(
                "Pathfinder",
                _pathfinder,
                typeof(Pathfinder),
                true) as Pathfinder;

            _grid = EditorGUILayout.ObjectField(
                "Grid",
                _grid,
                typeof(Grid),
                true) as Grid;

            if (GUILayout.Button("Find Path"))
            {
                if (!_targetPosition)
                {
                    Debug.LogError("Target Position is not set.");
                    return;
                }

                if (!_pathfinder)
                {
                    Debug.LogError("Pathfinder is not set.");
                    return;
                }

                _grid?.InitializeGrid(
                    _gridDimensions.x,
                    _gridDimensions.y,
                    _grid.unwalkableLayer,
                    _grid.walkableLayer,
                    _grid.nodeRadius);

                _pathfinder.grid = _grid;

                var target = new Vector2Int((int)_targetPosition.position.x, (int)_targetPosition.position.y);

                var startTime = Time.realtimeSinceStartup;
                var path = _pathfinder.FindPath(target);
                Debug.Log($"Time taken: {Time.realtimeSinceStartup - startTime}");

                if (path != null)
                {
                    foreach (var node in path)
                    {
                        Debug.Log($"Path Node: {node.worldPosition}");
                    }
                }
                else
                {
                    Debug.Log("No path found!");
                }
            }
        }
    }
}
