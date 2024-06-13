using UnityEditor;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;

namespace Editor
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var grid = target as Grid;
            Handles.color = Color.yellow;

            if (!grid!.drawGrid) return;

            var gridCenterOffset = new Vector2((grid.width - 1) * grid.nodeRadius, (grid.height - 1) * grid.nodeRadius);

            for (var x = 0; x < grid?.width; x++)
            {
                for (var y = 0; y < grid.height; y++)
                {
                    var pos = new Vector3(x * grid.nodeRadius * 2, y * grid.nodeRadius * 2, 0) - (Vector3)gridCenterOffset;
                    Handles.DrawWireCube(pos, new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 0));
                }
            }
        }
    }
}
