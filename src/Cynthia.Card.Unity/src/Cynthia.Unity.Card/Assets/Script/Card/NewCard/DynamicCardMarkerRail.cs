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
        _font = _font ?? Resources.Load<Font>("FountInfo/NotoSansSC-VariableFont_wght")
                      ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
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
        _root.sizeDelta = new Vector2(36, 150);
        var layout = root.GetComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = 2;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        DynamicMarkerTooltip.Configure(owner.GetComponentInParent<Canvas>());
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
                    .Select(x => (string.IsNullOrWhiteSpace(x.InstanceId) ? "•" : InstanceLabel(x.InstanceId)) +
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
        badge.GetComponent<RectTransform>().sizeDelta = new Vector2(36, 32);
        var layout = badge.GetComponent<LayoutElement>();
        layout.preferredWidth = 36;
        layout.preferredHeight = 32;
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
        text.fontSize = string.IsNullOrEmpty(value) ? 18 : 16;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = string.IsNullOrEmpty(value) ? label : label + value;
        var outline = text.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color32(3, 8, 10, 190);
        outline.effectDistance = new Vector2(.5f, -.5f);
    }

    private string CurrentLanguage => _translator?.TextLocalization?.ChosenLanguage?.Filename ?? "cn";

    private string InstanceLabel(string id)
    {
        var english = CurrentLanguage.StartsWith("en");
        switch ((id ?? "").ToLowerInvariant())
        {
            case "deploy": return english ? "Deploy" : "部署";
            case "graveyard": return english ? "Graveyard" : "墓场";
            case "turn": return english ? "Turn" : "回合";
            case "weather": return english ? "Weather" : "天气";
            case "hand": return english ? "Hand" : "手牌";
            case "deck": return english ? "Deck" : "牌组";
            default: return id;
        }
    }

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
    private bool _hovered;

    public void Bind(string title, string description)
    {
        _title = title;
        _description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
        DynamicMarkerTooltip.Show(_title, _description, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        DynamicMarkerTooltip.Hide();
    }

    private void Update()
    {
        if (_hovered) DynamicMarkerTooltip.Move(Input.mousePosition);
    }
}

internal static class DynamicMarkerTooltip
{
    private static RectTransform _canvasRect;
    private static GameObject _tooltip;
    private static Text _titleText;
    private static Text _descriptionText;
    private static Canvas _sourceCanvas;
    private static Camera _canvasCamera;

    public static void Configure(Canvas sourceCanvas)
    {
        if (sourceCanvas != null && sourceCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            _sourceCanvas = sourceCanvas;
    }

    public static void Show(string title, string description, Vector2 screenPosition)
    {
        if (string.IsNullOrWhiteSpace(description)) return;
        EnsureCreated();
        if (_tooltip == null) return;
        _titleText.text = title ?? "";
        _descriptionText.text = description ?? "";
        var visualLines = Mathf.Max(1, (description ?? "").Split('\n').Length + Mathf.CeilToInt((description ?? "").Length / 32f) - 1);
        _tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(430, Mathf.Clamp(104 + visualLines * 20, 124, 224));
        Move(screenPosition);
        _tooltip.SetActive(true);
        _tooltip.transform.SetAsLastSibling();
    }

    public static void Move(Vector2 screenPosition)
    {
        if (_tooltip == null || _canvasRect == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPosition, _canvasCamera, out var local);
        var rect = _tooltip.GetComponent<RectTransform>();
        var half = rect.sizeDelta * .5f;
        var showOnRight = local.x + rect.sizeDelta.x + 30 <= _canvasRect.rect.xMax;
        var point = local + new Vector2(showOnRight ? half.x + 20 : -half.x - 20, half.y + 18);
        point.x = Mathf.Clamp(point.x, _canvasRect.rect.xMin + half.x + 12, _canvasRect.rect.xMax - half.x - 12);
        point.y = Mathf.Clamp(point.y, _canvasRect.rect.yMin + half.y + 12, _canvasRect.rect.yMax - half.y - 12);
        rect.anchoredPosition = point;
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
            if (_sourceCanvas == null)
            {
                _sourceCanvas = Object.FindObjectsOfType<Canvas>()
                    .Where(x => x != null && x != canvas &&
                                x.renderMode != RenderMode.ScreenSpaceOverlay && x.worldCamera != null)
                    .OrderBy(x => x.sortingOrder)
                    .FirstOrDefault();
            }
            canvas.renderMode = _sourceCanvas?.renderMode ?? RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = _sourceCanvas?.worldCamera ?? Camera.main;
            canvas.planeDistance = _sourceCanvas?.planeDistance ?? 100;
            canvas.sortingLayerID = _sourceCanvas?.sortingLayerID ?? 0;
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera == null)
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // Hover help must clear player-name HUD, while card details and
            // modal interaction canvases remain above it.
            canvas.overrideSorting = true;
            canvas.sortingOrder = 2;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;
        }
        var tooltipCanvas = canvasObject.GetComponent<Canvas>();
        _canvasCamera = tooltipCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : tooltipCanvas.worldCamera;
        _canvasRect = canvasObject.GetComponent<RectTransform>();
        _tooltip = new GameObject("DynamicMarkerTooltip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _tooltip.transform.SetParent(canvasObject.transform, false);
        var rect = _tooltip.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(430, 124);
        var image = _tooltip.GetComponent<Image>();
        image.color = new Color32(6, 19, 23, 248);
        image.raycastTarget = false;
        var outline = _tooltip.AddComponent<Outline>();
        outline.effectColor = new Color32(196, 145, 63, 210);
        outline.effectDistance = new Vector2(1, -1);

        var accent = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.transform.SetParent(_tooltip.transform, false);
        var accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = Vector2.zero;
        accentRect.anchorMax = new Vector2(0, 1);
        accentRect.pivot = new Vector2(0, .5f);
        accentRect.sizeDelta = new Vector2(5, 0);
        accent.GetComponent<Image>().color = new Color32(222, 170, 78, 255);
        accent.GetComponent<Image>().raycastTarget = false;

        _titleText = MakeTooltipText("Title", 19, FontStyle.Bold, new Color32(239, 220, 177, 255));
        _titleText.rectTransform.anchorMin = new Vector2(0, 1);
        _titleText.rectTransform.anchorMax = Vector2.one;
        _titleText.rectTransform.offsetMin = new Vector2(22, -49);
        _titleText.rectTransform.offsetMax = new Vector2(-18, -12);

        var divider = new GameObject("Divider", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        divider.transform.SetParent(_tooltip.transform, false);
        var dividerRect = divider.GetComponent<RectTransform>();
        dividerRect.anchorMin = new Vector2(0, 1);
        dividerRect.anchorMax = new Vector2(1, 1);
        dividerRect.pivot = new Vector2(.5f, 1);
        dividerRect.offsetMin = new Vector2(22, -55);
        dividerRect.offsetMax = new Vector2(-18, -53);
        divider.GetComponent<Image>().color = new Color32(99, 118, 115, 150);
        divider.GetComponent<Image>().raycastTarget = false;

        _descriptionText = MakeTooltipText("Description", 16, FontStyle.Normal, new Color32(221, 226, 218, 255));
        _descriptionText.rectTransform.anchorMin = Vector2.zero;
        _descriptionText.rectTransform.anchorMax = Vector2.one;
        _descriptionText.rectTransform.offsetMin = new Vector2(22, 14);
        _descriptionText.rectTransform.offsetMax = new Vector2(-18, -66);
        _descriptionText.lineSpacing = 1.12f;
        _tooltip.SetActive(false);
    }

    private static Text MakeTooltipText(string name, int size, FontStyle style, Color color)
    {
        var text = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        text.transform.SetParent(_tooltip.transform, false);
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.UpperLeft;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        var outline = text.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color32(0, 0, 0, 190);
        outline.effectDistance = new Vector2(1, -1);
        return text;
    }
}
