using UnityEngine;
using UnityEngine.UI;
namespace Assets.Script.DynamicCards
{
    // Lightweight UI reconstruction of the original cyan preview highlight.
    // Geometry is shared with the card pivot; no camera, texture or particle simulation is needed.
    public sealed class DynamicCardPreviewGlow : MaskableGraphic
    {
        private void Update() { SetVerticesDirty(); }
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect r = rectTransform.rect;
            float pulse = .82f + .18f * Mathf.Sin(Time.unscaledTime * 2.1f);
            for (int i = 8; i >= 0; --i)
            {
                float spread = i * 1.1f;
                float opacity = (i == 0 ? .8f : .045f * (1 - i / 10f)) * pulse;
                var c = new Color(.14f, .74f, 1f, opacity);
                float left = r.xMin - spread, right = r.xMax + spread;
                float bottom = r.yMin - spread, top = r.yMax + spread;
                float width = i == 0 ? 1.5f : 2.5f;
                Quad(mesh, left, bottom, right, bottom + width, c);
                Quad(mesh, left, top - width, right, top, c);
                Quad(mesh, left, bottom + width, left + width, top - width, c);
                Quad(mesh, right - width, bottom + width, right, top - width, c);
            }
        }
        private static void Quad(VertexHelper mesh, float x0, float y0, float x1, float y1, Color color)
        {
            int start = mesh.currentVertCount;
            mesh.AddVert(new Vector3(x0, y0), color, Vector2.zero);
            mesh.AddVert(new Vector3(x0, y1), color, Vector2.zero);
            mesh.AddVert(new Vector3(x1, y1), color, Vector2.zero);
            mesh.AddVert(new Vector3(x1, y0), color, Vector2.zero);
            mesh.AddTriangle(start, start + 1, start + 2);
            mesh.AddTriangle(start, start + 2, start + 3);
        }
    }
}
