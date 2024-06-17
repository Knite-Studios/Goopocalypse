using Entity.Pathfinding;
using UnityEditor;
using UnityEngine;

namespace Editor.Menus
{
    public class PathfinderDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Pathfinder Debug")]
        private static void OpenMenu() => GetWindow<PathfinderDebug>().Show();

        private Transform _targetPosition;
        private Pathfinder _pathfinder;
        private PathfindingGrid _grid;

        private Transform _gridCheck;

        private int _targetX, _targetY;

        private void OnGUI()
        {
            _targetPosition = EditorGUILayout.ObjectField(
                "Target Position",
                _targetPosition,
                typeof(Transform),
                true) as Transform;

            _pathfinder = EditorGUILayout.ObjectField(
                "Pathfinder",
                _pathfinder,
                typeof(Pathfinder),
                true) as Pathfinder;

            _grid = EditorGUILayout.ObjectField(
                "Grid",
                _grid,
                typeof(PathfindingGrid),
                true) as PathfindingGrid;

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

                if (!_grid)
                {
                    Debug.LogError("Grid is not set.");
                    return;
                }

                _grid.InitializeGrid();

                _pathfinder.grid = _grid;

                var target = new Vector2(_targetPosition.position.x, _targetPosition.position.y);

                var startTime = Time.realtimeSinceStartup;
                var path = _pathfinder.FindPath(target);
                Debug.Log($"Time taken: {Time.realtimeSinceStartup - startTime}");

                if (path != null)
                {
                    foreach (var node in path)
                    {
                        Debug.Log($"Path Node: {node.WorldPosition}");
                    }
                }
                else
                {
                    Debug.Log("No path found!");
                }
            }

            GUILayout.Space(24);

            _gridCheck = EditorGUILayout.ObjectField(
                "Object to Check",
                _gridCheck,
                typeof(Transform),
                true) as Transform;

            if (GUILayout.Button("Log Player Details") && _gridCheck)
            {
                var gridPos = _grid!.ToGridPosition(_gridCheck.position);
                Debug.Log($"Player grid position: {gridPos}");

                if (gridPos != null)
                {
                    var node = _grid.GetNode(gridPos.x, gridPos.y);
                    Debug.Log($"Node world position: {node?.WorldPosition}");
                }
            }

            GUILayout.Space(24);

            _targetX = EditorGUILayout.IntField("Target X", _targetX);
            _targetY = EditorGUILayout.IntField("Target Y", _targetY);

            if (GUILayout.Button("Log Node Details"))
            {
                var node = _grid!.GetNode(_targetX, _targetY);
                Debug.Log($"Node world position: {node?.WorldPosition}");
            }
        }
    }
}
