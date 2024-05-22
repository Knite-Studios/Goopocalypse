using UnityEditor;
using UnityEngine;
using Grid = Entity.Pathfinding.Grid;

namespace Editor
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : UnityEditor.Editor
    {
        // public override void OnInspectorGUI()
        // {
        //     DrawDefaultInspector();
        //
        //     var grid = target as Grid;
        //     if (GUILayout.Button("Initialize Grid"))
        //     {
        //         grid?.InitializeGrid(grid.width, grid.height, grid.unwalkableLayer, grid.walkableLayer, grid.nodeRadius);
        //     }
        // }

        private void OnSceneGUI()
        {
            var grid = target as Grid;
            Handles.color = Color.yellow;

            if (!grid!.drawGrid) return;

            for (var x = 0; x < grid?.width; x++)
            {
                for (var y = 0; y < grid.height; y++)
                {
                    // var node = grid.GetNode(x, y);
                    // Handles.color = node.isWalkable ? Color.green : Color.red;
                    var pos = new Vector3(x * grid.nodeRadius * 2 + grid.nodeRadius, y * grid.nodeRadius * 2 + grid.nodeRadius, 0);
                    Handles.DrawWireCube(pos, new Vector3(grid.nodeRadius * 2, grid.nodeRadius * 2, 0));
                    // if (Handles.Button(pos, Quaternion.identity, 0.5f, 0.5f, Handles.CubeHandleCap))
                    // {
                    //     grid.SetWalkable(new Vector2(x, y), !grid.GetNode(x, y).isWalkable);
                    //     EditorUtility.SetDirty(target);
                    // }
                }
            }
        }
    }
}
