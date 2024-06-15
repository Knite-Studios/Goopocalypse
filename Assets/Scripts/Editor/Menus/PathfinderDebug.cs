using Entity.Pathfinding;
using UnityEditor;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;

namespace Editor.Menus
{
    public class PathfinderDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Pathfinder Debug")]
        private static void OpenMenu() => GetWindow<PathfinderDebug>().Show();

        private Transform _targetPosition;
        private Pathfinder _pathfinder;
        private Grid _grid;

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
        }
    }
}
