using UnityEngine;
namespace Assets.Script.DynamicCards
{
    // Keeps layout alive while withholding the whole card until its first dynamic render.
    public sealed class DynamicCardPresentation : MonoBehaviour
    {
        private CanvasGroup group;
        private RectTransform rect, visual;
        private DynamicCardPreviewGlow glow;
        private float alpha, started;
        private bool raycasts, waiting, animating;
        private Quaternion rotation;
        private Vector3 position, scale;
        private readonly System.Collections.Generic.List<DynamicCardPerspectiveMesh> perspectiveMeshes = new System.Collections.Generic.List<DynamicCardPerspectiveMesh>();
        private float projectionDistance;
        internal bool PerspectiveActive => animating;
        internal Transform Visual => visual;
        internal Quaternion PerspectiveRotation => Quaternion.Inverse(rotation) * rect.localRotation;
        internal float ProjectionDistance => projectionDistance;
        public bool Waiting { get { return waiting; } }
        public const float MaxPitch = 1.5f;
        public const float MaxYaw = 7f;
        // Original deckbuilder_base SidePreviewCardAnimationSettings: 55 degrees, one second.
        private static readonly AnimationCurve EnterCurve = new AnimationCurve(
            new Keyframe(0, 0, 6.709384f, 6.709384f),
            new Keyframe(.36976156f, 1.032603f, .1680851f, .1680851f),
            new Keyframe(.6777794f, .9950043f, -.03262398f, -.03262398f),
            new Keyframe(.907414f, 1.0006036f, -.000843068f, -.000843068f),
            new Keyframe(1, 1, 0, 0));
        // Keep the layout/description outside the rotating card. Existing serialized references stay valid.
        public void Configure(RectTransform cardBorder, Transform description)
        {
            if (visual != null || cardBorder == null) return;
            var root = (RectTransform)transform;
            var children = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in root)
                if (description == null || (child != description && !description.IsChildOf(child))) children.Add(child);
            visual = new GameObject("DynamicCardVisualPivot", typeof(RectTransform)).GetComponent<RectTransform>();
            visual.SetParent(root, false);
            visual.anchorMin = visual.anchorMax = root.pivot;
            visual.sizeDelta = root.rect.size;
            visual.pivot = new Vector2(.5f, .5f);
            visual.position = cardBorder.TransformPoint(cardBorder.rect.center);
            visual.SetAsFirstSibling();
            foreach (var child in children) child.SetParent(visual, true);
            var effect = new GameObject("PreviewEdgeGlow", typeof(RectTransform), typeof(CanvasRenderer), typeof(DynamicCardPreviewGlow));
            var effectRect = effect.GetComponent<RectTransform>();
            effectRect.SetParent(visual, false);
            effectRect.position = cardBorder.TransformPoint(cardBorder.rect.center);
            effectRect.sizeDelta = cardBorder.rect.size;
            effectRect.SetAsFirstSibling();
            glow = effect.GetComponent<DynamicCardPreviewGlow>();
            glow.raycastTarget = false;
            effect.SetActive(false);
            // UICardPreview uses a 35-degree camera. Overlay canvases need the same projective divide.
            float height = visual.InverseTransformVector(cardBorder.TransformVector(Vector3.up * cardBorder.rect.height)).magnitude;
            projectionDistance = height * .5f / Mathf.Tan(35f * .5f * Mathf.Deg2Rad);
            foreach (var graphic in visual.GetComponentsInChildren<UnityEngine.UI.Graphic>(true)) RegisterGraphic(graphic);
        }
        public void RegisterGraphic(UnityEngine.UI.Graphic graphic)
        {
            if (visual == null || !graphic.transform.IsChildOf(visual)) return;
            var mesh = graphic.GetComponent<DynamicCardPerspectiveMesh>();
            if (mesh == null) mesh = graphic.gameObject.AddComponent<DynamicCardPerspectiveMesh>();
            mesh.Presentation = this;
            if (!perspectiveMeshes.Contains(mesh)) perspectiveMeshes.Add(mesh);
        }
        public void Begin()
        {
            Restore(); rect = visual != null ? visual : (RectTransform)transform;
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
            alpha = group.alpha; raycasts = group.blocksRaycasts;
            rotation = rect.localRotation; position = rect.localPosition; scale = rect.localScale;
            waiting = true; group.alpha = 0; group.blocksRaycasts = false;
        }
        public bool AncestorsVisible(Transform art)
        {
            for (var current = art; current != null; current = current.parent)
                foreach (var cg in current.GetComponents<CanvasGroup>())
                {
                    if ((cg == group && waiting ? alpha : cg.alpha) <= .001f) return false;
                    if (cg.ignoreParentGroups) return true;
                }
            return true;
        }
        public void Reveal(bool dynamic)
        {
            if (!waiting) return;
            waiting = false; group.alpha = alpha; group.blocksRaycasts = raycasts;
            animating = dynamic; started = Time.unscaledTime;
            if (glow != null) glow.gameObject.SetActive(dynamic);
            if (animating) Tick(Vector2.zero);
        }
        public void InterruptEntrance() { started = Time.unscaledTime - 1; }
        public void RevealInPlace()
        {
            Begin();
            Reveal(true);
            InterruptEntrance();
            Tick(Vector2.zero);
        }
        public void Tick(Vector2 perspective)
        {
            if (!animating) return;
            float t = Time.unscaledTime - started;
            float remaining = 1 - EnterCurve.Evaluate(Mathf.Clamp01(t));
            // Entry and idle share the card centre, never the description or the padded art texture.
            // The same normalized angles drive CardPerspectiveHandler's 3D layers.
            // One owner applies both automatic and manual movement to this visual pivot.
            rect.localRotation = rotation * Quaternion.Euler(perspective.y * MaxPitch, 55 * remaining + perspective.x * MaxYaw, 0);
            rect.localPosition = position;
            rect.localScale = scale;
            RefreshMeshes();
        }
        private void RefreshMeshes()
        {
            for (int i = perspectiveMeshes.Count - 1; i >= 0; i--)
                if (perspectiveMeshes[i] == null) perspectiveMeshes.RemoveAt(i); else perspectiveMeshes[i].Refresh();
        }
        private void RestoreTransform()
        { if (rect != null) { rect.localRotation = rotation; rect.localPosition = position; rect.localScale = scale; } }
        public void Restore()
        {
            if (group != null && waiting) { group.alpha = alpha; group.blocksRaycasts = raycasts; }
            if (animating) RestoreTransform();
            waiting = animating = false;
            RefreshMeshes();
            if (glow != null) glow.gameObject.SetActive(false);
        }
        private void OnDisable() { Restore(); }
    }
}
