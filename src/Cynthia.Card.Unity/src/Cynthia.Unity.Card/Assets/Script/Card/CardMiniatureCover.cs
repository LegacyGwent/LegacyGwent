using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

/// <summary>Fills a legacy card strip proportionally, without changing its frame or shared sprite.</summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
public sealed class CardMiniatureCover : BaseMeshEffect
{
    public const float ClipInset = 3f;
    public static readonly Vector2 DefaultAlignment = new Vector2(1f, .5f);
    [SerializeField] private bool clipToParent;

    public static void Apply(Image image, bool clipToParent)
    {
        if (image == null) return;
        var cover = image.GetComponent<CardMiniatureCover>();
        if (cover == null) cover = image.gameObject.AddComponent<CardMiniatureCover>();
        cover.clipToParent = clipToParent;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        image.useSpriteMesh = false;
        image.SetVerticesDirty();
    }

    // Normalized source crop (x, y, width, height). Sizes describe this sprite, not its atlas.
    public static Vector4 CalculateCoverUv(float sourceWidth, float sourceHeight,
        float targetWidth, float targetHeight, Vector2 alignment)
    {
        if (!Positive(sourceWidth) || !Positive(sourceHeight) ||
            !Positive(targetWidth) || !Positive(targetHeight))
            return new Vector4(0, 0, 1, 1);
        float sourceAspect = sourceWidth / sourceHeight;
        float targetAspect = targetWidth / targetHeight;
        float width = Mathf.Min(1, targetAspect / sourceAspect);
        float height = Mathf.Min(1, sourceAspect / targetAspect);
        return new Vector4(Mathf.Clamp01(alignment.x) * (1 - width),
            Mathf.Clamp01(alignment.y) * (1 - height), width, height);
    }

    // Interpolate within the UVs emitted by Image. This preserves atlas offsets and UV orientation.
    // uGUI's outer UVs already exclude trimmed padding; never inset that region a second time.
    public static Vector2 MapQuadUv(Vector2 bottomLeft, Vector2 topLeft,
        Vector2 topRight, Vector2 bottomRight, Vector2 point)
    {
        return Vector2.Lerp(Vector2.Lerp(bottomLeft, bottomRight, point.x),
            Vector2.Lerp(topLeft, topRight, point.x), point.y);
    }

    public static Rect ClipRectToParent(Rect imageRect, Rect parentRect, float inset)
    {
        if (!Valid(imageRect) || !Valid(parentRect) || !Finite(inset) || inset < 0 ||
            parentRect.width <= 2 * inset || parentRect.height <= 2 * inset)
            return Rect.zero;
        float xMin = Mathf.Max(imageRect.xMin, parentRect.xMin + inset);
        float yMin = Mathf.Max(imageRect.yMin, parentRect.yMin + inset);
        float xMax = Mathf.Min(imageRect.xMax, parentRect.xMax - inset);
        float yMax = Mathf.Min(imageRect.yMax, parentRect.yMax - inset);
        return xMax > xMin && yMax > yMin ? Rect.MinMaxRect(xMin, yMin, xMax, yMax) : Rect.zero;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;
        var image = graphic as Image;
        if (image == null || image.type != Image.Type.Simple || image.useSpriteMesh ||
            image.overrideSprite == null || vh.currentVertCount != 4) return;

        Rect target = image.GetPixelAdjustedRect();
        var parent = image.rectTransform.parent as RectTransform;
        if (clipToParent && parent != null)
        {
            // Inset in the parent's UI units BEFORE converting to image coordinates.
            Rect inner = ClipRectToParent(parent.rect, parent.rect, ClipInset);
            target = Valid(inner) ? ClipRectToParent(target,
                TransformBounds(inner, parent, image.rectTransform), 0) : Rect.zero;
        }
        if (!Valid(target)) { vh.Clear(); return; }

        Sprite sprite = image.overrideSprite; // Image returns the ordinary sprite when no override is set.
        Vector4 padding = DataUtility.GetPadding(sprite);
        float sourceWidth = sprite.rect.width - padding.x - padding.z;
        float sourceHeight = sprite.rect.height - padding.y - padding.w;
        if (!Positive(sourceWidth) || !Positive(sourceHeight)) { vh.Clear(); return; }
        Vector4 crop = CalculateCoverUv(sourceWidth, sourceHeight, target.width, target.height, DefaultAlignment);

        UIVertex a = default(UIVertex), b = default(UIVertex), c = default(UIVertex), d = default(UIVertex);
        vh.PopulateUIVertex(ref a, 0); vh.PopulateUIVertex(ref b, 1);
        vh.PopulateUIVertex(ref c, 2); vh.PopulateUIVertex(ref d, 3);
        // Preserve tint, secondary UVs and other channels; only this Image's quad is changed.
        Vector2 uvA = a.uv0, uvB = b.uv0, uvC = c.uv0, uvD = d.uv0;
        SetCorner(vh, a, 0, target.xMin, target.yMin,
            MapQuadUv(uvA, uvB, uvC, uvD, new Vector2(crop.x, crop.y)));
        SetCorner(vh, b, 1, target.xMin, target.yMax,
            MapQuadUv(uvA, uvB, uvC, uvD, new Vector2(crop.x, crop.y + crop.w)));
        SetCorner(vh, c, 2, target.xMax, target.yMax,
            MapQuadUv(uvA, uvB, uvC, uvD, new Vector2(crop.x + crop.z, crop.y + crop.w)));
        SetCorner(vh, d, 3, target.xMax, target.yMin,
            MapQuadUv(uvA, uvB, uvC, uvD, new Vector2(crop.x + crop.z, crop.y)));
    }

    private static void SetCorner(VertexHelper vh, UIVertex vertex, int index, float x, float y, Vector2 uv)
    {
        vertex.position = new Vector3(x, y, vertex.position.z);
        vertex.uv0 = uv;
        vh.SetUIVertex(vertex, index);
    }

    // These banner/image transforms are axis-aligned in the authored prefabs. Re-evaluate
    // on each normal uGUI rebuild so window/layout changes cannot leave a stale clip window.
    private static Rect TransformBounds(Rect rect, RectTransform from, RectTransform to)
    {
        Vector3 a = to.InverseTransformPoint(from.TransformPoint(new Vector3(rect.xMin, rect.yMin)));
        Vector3 b = to.InverseTransformPoint(from.TransformPoint(new Vector3(rect.xMin, rect.yMax)));
        Vector3 c = to.InverseTransformPoint(from.TransformPoint(new Vector3(rect.xMax, rect.yMax)));
        Vector3 d = to.InverseTransformPoint(from.TransformPoint(new Vector3(rect.xMax, rect.yMin)));
        return Rect.MinMaxRect(Mathf.Min(Mathf.Min(a.x, b.x), Mathf.Min(c.x, d.x)),
            Mathf.Min(Mathf.Min(a.y, b.y), Mathf.Min(c.y, d.y)),
            Mathf.Max(Mathf.Max(a.x, b.x), Mathf.Max(c.x, d.x)),
            Mathf.Max(Mathf.Max(a.y, b.y), Mathf.Max(c.y, d.y)));
    }

    private static bool Finite(float value) { return !float.IsNaN(value) && !float.IsInfinity(value); }
    private static bool Positive(float value) { return value > 0 && Finite(value); }
    private static bool Valid(Rect rect)
    {
        return Finite(rect.x) && Finite(rect.y) && Positive(rect.width) && Positive(rect.height);
    }
}
