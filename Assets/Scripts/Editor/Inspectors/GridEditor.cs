using UnityEditor;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;

namespace Editor.Inspectors
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var grid = target as Grid;
            Handles.color = Color.yellow;

            if (!grid!.drawGrid) return;

            for (var x = 0; x < grid?.width; x++)
            {
                for (var y = 0; y < grid.height; y++)
                {
                    Handles.DrawWireCube(grid.gridPosition, new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 0));
                }
            }
        }
    }
}
