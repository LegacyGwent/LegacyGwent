using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // Project the complete UI portrait through the old preview's camera, including its frame and badges.
    // The source 3D artwork is rendered separately using the same normalized card rotation.
    public sealed class DynamicCardPerspectiveMesh : BaseMeshEffect
    {
        public DynamicCardPresentation Presentation;
        public void Refresh() { if (graphic != null) graphic.SetVerticesDirty(); }
        public override void ModifyMesh(VertexHelper helper)
        {
            if (!IsActive() || Presentation == null || !Presentation.PerspectiveActive || Presentation.Visual == null) return;
            var pivot = Presentation.Visual;
            Quaternion rotation = Presentation.PerspectiveRotation, inverse = Quaternion.Inverse(rotation);
            float distance = Mathf.Max(1, Presentation.ProjectionDistance);
            UIVertex vertex = default(UIVertex);
            for (int i = 0; i < helper.currentVertCount; i++)
            {
                helper.PopulateUIVertex(ref vertex, i);
                Vector3 local = pivot.InverseTransformPoint(transform.TransformPoint(vertex.position));
                Vector3 tilted = rotation * local;
                float ratio = distance / Mathf.Max(distance * .25f, distance + tilted.z);
                Vector3 projected = inverse * new Vector3(tilted.x * ratio, tilted.y * ratio, tilted.z);
                vertex.position = transform.InverseTransformPoint(pivot.TransformPoint(projected));
                helper.SetUIVertex(vertex, i);
            }
        }
    }
}
