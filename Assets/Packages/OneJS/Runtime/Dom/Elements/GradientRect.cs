using UnityEngine;
using UnityEngine.UIElements;

namespace OneJS.Dom {
    public class GradientRect : VisualElement {
        static readonly Vertex[] vertices = new Vertex[4];
        static readonly ushort[] indices = { 0, 1, 2, 2, 3, 0 };

        public Color[] Colors { get; set; }
        public bool Vertical { get; set; }

        public GradientRect() {
            generateVisualContent = GenerateVisualContent;
        }

        void GenerateVisualContent(MeshGenerationContext mgc) {
            var rect = contentRect;
            if (rect.width < 0.1f || rect.height < 0.1f)
                return;
            if (Colors.Length == 1) {
                vertices[0].tint = Colors[0];
                vertices[1].tint = Colors[0];
                vertices[2].tint = Colors[0];
                vertices[3].tint = Colors[0];
            } else if (Colors.Length == 2) {
                vertices[0].tint = Colors[0];
                vertices[1].tint = Colors[0];
                vertices[2].tint = Colors[1];
                vertices[3].tint = Colors[1];
            } else if (Colors.Length == 3) {
                vertices[0].tint = Colors[0];
                vertices[1].tint = Colors[1];
                vertices[2].tint = Colors[2];
                vertices[3].tint = Colors[2];
            } else if (Colors.Length == 4) {
                vertices[0].tint = Colors[0];
                vertices[1].tint = Colors[1];
                vertices[2].tint = Colors[2];
                vertices[3].tint = Colors[3];
            }

            var left = 0f;
            var right = rect.width;
            var top = 0f;
            var bottom = rect.height;

            if (Vertical)
            {
                vertices[1].position = new Vector3(left, bottom, Vertex.nearZ); // 0
                vertices[2].position = new Vector3(left, top, Vertex.nearZ); // 1
                vertices[3].position = new Vector3(right, top, Vertex.nearZ); // 2
                vertices[0].position = new Vector3(right, bottom, Vertex.nearZ); // 3
            }
            else
            {
                vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
                vertices[1].position = new Vector3(left, top, Vertex.nearZ);
                vertices[2].position = new Vector3(right, top, Vertex.nearZ);
                vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);
            }

            MeshWriteData mwd = mgc.Allocate(vertices.Length, indices.Length);
            mwd.SetAllVertices(vertices);
            mwd.SetAllIndices(indices);
        }
    }
}
