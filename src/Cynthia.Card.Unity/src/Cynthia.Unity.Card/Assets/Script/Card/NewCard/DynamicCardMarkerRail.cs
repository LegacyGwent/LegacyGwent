using System.Collections.Generic;
using System.Linq;
using Assets.Script.Localization;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class DynamicCardMarkerRail : MonoBehaviour
{
    private static Font _font;
    private RectTransform _root;
    private GwentClientService _client;
    private LocalizationService _translator;

    public static DynamicCardMarkerRail Attach(CardShowInfo owner)
    {
        if (owner == null) return null;
        var rail = owner.GetComponent<DynamicCardMarkerRail>() ?? owner.gameObject.AddComponent<DynamicCardMarkerRail>();
        rail.Initialize(owner);
        return rail;
    }

    private void Initialize(CardShowInfo owner)
    {
        if (_root != null) return;
        _font = _font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        _client = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        // CardShowInfo lives on the 3D CardObj transform. Anchoring a UI rail
        // directly to that transform leaves its RectTransform at canvas origin.
        // Use the rendered card border as the coordinate space instead.
        var parent = owner.CardBorder != null
            ? owner.CardBorder.rectTransform
            : owner.CardImg != null ? owner.CardImg.rectTransform : owner.transform;
        var root = new GameObject("DynamicMarkerRail", typeof(RectTransform), typeof(VerticalLayoutGroup));
        root.transform.SetParent(parent, false);
        _root = root.GetComponent<RectTransform>();
        _root.anchorMin = Vector2.one;
        _root.anchorMax = Vector2.one;
        _root.pivot = Vector2.one;
        _root.anchoredPosition = new Vector2(-4, -4);
        _root.sizeDelta = new Vector2(30, 132);
        var layout = root.GetComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = 2;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
    }

    public void Render(IList<DynamicCardMarker> markers)
    {
        if (_root == null) return;
        foreach (Transform child in _root) Destroy(child.gameObject);
        var visible = (markers ?? new List<DynamicCardMarker>()).Where(x => x != null).ToList();
        _root.gameObject.SetActive(visible.Count > 0);
        if (visible.Count == 0) return;

        var definitions = (_client?.FeatureManifest?.CardMarkerDefinitions ?? new List<DynamicCardMarkerDefinition>())
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Id))
            .GroupBy(x => x.Id)
            .ToDictionary(x => x.Key, x => x.First());
        var groups = visible
            .GroupBy(x => x.DefinitionId ?? "")
            .ToList();
        foreach (var group in groups.Take(3))
        {
            var marker = group.First();
            definitions.TryGetValue(marker.DefinitionId ?? "", out var definition);
            var title = definition?.Name?.Resolve(CurrentLanguage) ?? definition?.ShortLabel ?? marker.DefinitionId;
            var groupCount = group.Count();
            if (groupCount > 1) title += " ×" + groupCount;
            var description = definition?.Description?.Resolve(CurrentLanguage) ?? "";
            if (groupCount > 1)
            {
                var instances = group
                    .Where(x => !string.IsNullOrWhiteSpace(x.InstanceId) || x.Value.HasValue)
                    .Select(x => (string.IsNullOrWhiteSpace(x.InstanceId) ? "•" : x.InstanceId) +
                                 (x.Value.HasValue ? "：" + x.Value.Value : ""));
                var instanceText = string.Join("  ", instances);
                if (!string.IsNullOrWhiteSpace(instanceText))
                    description = string.IsNullOrWhiteSpace(description)
                        ? instanceText
                        : description + "\n" + instanceText;
            }
            MakeBadge(
                definition?.ShortLabel ?? FallbackLabel(marker.DefinitionId),
                groupCount > 1 ? "×" + groupCount : ValueText(definition, marker),
                StyleColor(definition?.StyleToken),
                title,
                description);
        }
        if (groups.Count > 3)
        {
            var hidden = groups.Skip(3).Select(group =>
            {
                var marker = group.First();
                definitions.TryGetValue(marker.DefinitionId ?? "", out var definition);
                var name = definition?.Name?.Resolve(CurrentLanguage) ?? definition?.ShortLabel ?? marker.DefinitionId;
                var description = definition?.Description?.Resolve(CurrentLanguage) ?? "";
                return string.IsNullOrWhiteSpace(description) ? name : name + "：" + description;
            });
            MakeBadge("+", (groups.Count - 3).ToString(), StyleColor("neutral"),
                CurrentLanguage.StartsWith("en") ? "More statuses" : "更多状态",
                string.Join("\n", hidden));
        }
    }

    private void MakeBadge(string label, string value, Color color, string title, string description)
    {
        var badge = new GameObject("Marker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        badge.transform.SetParent(_root, false);
        badge.GetComponent<RectTransform>().sizeDelta = new Vector2(28, 28);
        var layout = badge.GetComponent<LayoutElement>();
        layout.preferredWidth = 28;
        layout.preferredHeight = 28;
        var image = badge.GetComponent<Image>();
        image.color = new Color(color.r, color.g, color.b, .94f);
        image.raycastTarget = !string.IsNullOrWhiteSpace(description);
        if (image.raycastTarget)
        {
            var hover = badge.AddComponent<DynamicMarkerHoverTarget>();
            hover.Bind(title, description);
        }
        var text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        text.transform.SetParent(badge.transform, false);
        var rect = text.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        text.font = _font;
        text.fontSize = string.IsNullOrEmpty(value) ? 16 : 12;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = string.IsNullOrEmpty(value) ? label : label + value;
    }

    private string CurrentLanguage => _translator?.TextLocalization?.ChosenLanguage?.Filename ?? "cn";

    private static string FallbackLabel(string id)
        => string.IsNullOrWhiteSpace(id) ? "?" : id.Substring(0, 1).ToUpperInvariant();

    private static string ValueText(DynamicCardMarkerDefinition definition, DynamicCardMarker marker)
    {
        if (!marker.Value.HasValue || definition == null ||
            string.Equals(definition.ValueDisplay, "none", System.StringComparison.OrdinalIgnoreCase)) return "";
        return marker.Value.Value.ToString();
    }

    internal static Color StyleColor(string style)
    {
        switch ((style ?? "neutral").ToLowerInvariant())
        {
            case "amber": return new Color32(171, 113, 35, 255);
            case "frost": return new Color32(47, 132, 164, 255);
            case "venom": return new Color32(74, 132, 65, 255);
            case "blood": return new Color32(146, 49, 52, 255);
            case "arcane": return new Color32(98, 67, 151, 255);
            case "nature": return new Color32(52, 115, 83, 255);
            case "steel": return new Color32(81, 95, 104, 255);
            default: return new Color32(62, 82, 88, 255);
        }
    }
}

public sealed class DynamicMarkerHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string _title;
    private string _description;

    public void Bind(string title, string description)
    {
        _title = title;
        _description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
        => DynamicMarkerTooltip.Show(_title, _description, eventData.position);

    public void OnPointerExit(PointerEventData eventData) => DynamicMarkerTooltip.Hide();
}

internal static class DynamicMarkerTooltip
{
    private static RectTransform _canvasRect;
    private static GameObject _tooltip;
    private static Text _text;

    public static void Show(string title, string description, Vector2 screenPosition)
    {
        if (string.IsNullOrWhiteSpace(description)) return;
        EnsureCreated();
        if (_tooltip == null) return;
        _text.text = string.IsNullOrWhiteSpace(title) ? description : title + "\n" + description;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPosition, null, out var local);
        _tooltip.GetComponent<RectTransform>().anchoredPosition = local + new Vector2(18, 18);
        _tooltip.SetActive(true);
        _tooltip.transform.SetAsLastSibling();
    }

    public static void Hide()
    {
        if (_tooltip != null) _tooltip.SetActive(false);
    }

    private static void EnsureCreated()
    {
        if (_tooltip != null && _canvasRect != null) return;
        var canvasObject = GameObject.Find("DynamicMarkerTooltipCanvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("DynamicMarkerTooltipCanvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 480;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;
        }
        _canvasRect = canvasObject.GetComponent<RectTransform>();
        _tooltip = new GameObject("DynamicMarkerTooltip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _tooltip.transform.SetParent(canvasObject.transform, false);
        var rect = _tooltip.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = Vector2.zero;
        rect.sizeDelta = new Vector2(380, 112);
        var image = _tooltip.GetComponent<Image>();
        image.color = new Color32(6, 20, 25, 248);
        image.raycastTarget = false;
        _text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        _text.transform.SetParent(_tooltip.transform, false);
        _text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        _text.fontSize = 15;
        _text.alignment = TextAnchor.MiddleLeft;
        _text.color = new Color32(239, 233, 215, 255);
        _text.raycastTarget = false;
        _text.horizontalOverflow = HorizontalWrapMode.Wrap;
        _text.verticalOverflow = VerticalWrapMode.Truncate;
        _text.rectTransform.anchorMin = Vector2.zero;
        _text.rectTransform.anchorMax = Vector2.one;
        _text.rectTransform.offsetMin = new Vector2(14, 10);
        _text.rectTransform.offsetMax = new Vector2(-14, -10);
        _tooltip.SetActive(false);
    }
}
