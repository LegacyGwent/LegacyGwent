using System.Collections.Generic;
using System.Linq;
using Assets.Script.Localization;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class GameResourcePanel : MonoBehaviour
{
    private static Font _font;
    private GwentClientService _client;
    private LocalizationService _translator;
    private RectTransform _myRoot;
    private RectTransform _enemyRoot;
    private GameObject _tooltip;
    private Text _tooltipTitle;
    private Text _tooltipDescription;
    private RectTransform _tooltipCanvasRect;
    private Camera _canvasCamera;

    public static GameResourcePanel Attach(GameUIControl owner)
    {
        if (owner == null) return null;
        var panel = owner.GetComponent<GameResourcePanel>() ?? owner.gameObject.AddComponent<GameResourcePanel>();
        panel.Initialize(owner);
        return panel;
    }

    private void Initialize(Component owner)
    {
        if (_myRoot != null) return;
        _font = _font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        _client = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        var canvasObject = GameObject.Find("GameResourceCanvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("GameResourceCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            ConfigureBoardCanvas(canvas, owner);
            canvas.sortingOrder = 0;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;
        }
        _myRoot = MakeRail(canvasObject.transform, "MyResources", new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 150));
        _enemyRoot = MakeRail(canvasObject.transform, "EnemyResources", new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -150));
        BuildTooltip(owner);
    }

    public void SetResources(IEnumerable<GameResourceState> mine, IEnumerable<GameResourceState> enemy)
    {
        if (_myRoot == null) return;
        var definitions = (_client?.FeatureManifest?.ResourceDefinitions ?? new List<GameResourceDefinition>())
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Id))
            .GroupBy(x => x.Id)
            .ToDictionary(x => x.Key, x => x.First());
        Rebuild(_myRoot, mine, definitions);
        Rebuild(_enemyRoot, enemy, definitions);
    }

    private void Rebuild(RectTransform root, IEnumerable<GameResourceState> states,
        IDictionary<string, GameResourceDefinition> definitions)
    {
        foreach (Transform child in root) Destroy(child.gameObject);
        var list = (states ?? Enumerable.Empty<GameResourceState>()).Where(x => x != null).ToList();
        root.gameObject.SetActive(list.Count > 0);
        foreach (var state in list.Take(8))
        {
            definitions.TryGetValue(state.DefinitionId ?? "", out var definition);
            BuildChip(root, state, definition);
        }
    }

    private void BuildChip(Transform root, GameResourceState state, GameResourceDefinition definition)
    {
        var chip = new GameObject("Resource-" + state.DefinitionId,
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        chip.transform.SetParent(root, false);
        var layout = chip.GetComponent<LayoutElement>();
        layout.preferredWidth = 142;
        layout.preferredHeight = 44;
        var background = chip.GetComponent<Image>();
        var color = DynamicCardMarkerRail.StyleColor(definition?.StyleToken);
        background.color = new Color(color.r * .52f, color.g * .52f, color.b * .52f, .96f);
        background.raycastTarget = false;

        var accent = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.transform.SetParent(chip.transform, false);
        var accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0, 0);
        accentRect.anchorMax = new Vector2(0, 1);
        accentRect.pivot = new Vector2(0, .5f);
        accentRect.sizeDelta = new Vector2(5, 0);
        accentRect.anchoredPosition = Vector2.zero;
        accent.GetComponent<Image>().color = color;
        accent.GetComponent<Image>().raycastTarget = false;

        var label = MakeText(chip.transform, "Label", definition?.ShortLabel ?? FallbackLabel(state.DefinitionId), 15, TextAnchor.MiddleLeft);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = new Vector2(.55f, 1);
        label.rectTransform.offsetMin = new Vector2(14, 0);
        label.rectTransform.offsetMax = Vector2.zero;
        var value = MakeText(chip.transform, "Value", FormatValue(state, definition), 17, TextAnchor.MiddleRight);
        value.fontStyle = FontStyle.Bold;
        value.rectTransform.anchorMin = new Vector2(.48f, 0);
        value.rectTransform.anchorMax = Vector2.one;
        value.rectTransform.offsetMin = Vector2.zero;
        value.rectTransform.offsetMax = new Vector2(-12, 0);

        var description = definition?.Description?.Resolve(CurrentLanguage) ?? "";
        if (!string.IsNullOrWhiteSpace(description))
        {
            background.raycastTarget = true;
            var hover = chip.AddComponent<ResourceHoverTarget>();
            hover.Bind(this, (definition?.Name?.Resolve(CurrentLanguage) ?? definition?.ShortLabel ?? state.DefinitionId), description);
        }
    }

    internal void ShowTooltip(string title, string description, Vector2 position)
    {
        if (_tooltip == null || string.IsNullOrWhiteSpace(description)) return;
        _tooltipTitle.text = title ?? "";
        _tooltipDescription.text = description ?? "";
        var visualLines = Mathf.Max(1, (description ?? "").Split('\n').Length + Mathf.CeilToInt((description ?? "").Length / 30f) - 1);
        _tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(420, Mathf.Clamp(104 + visualLines * 20, 124, 214));
        _tooltip.SetActive(true);
        _tooltip.transform.SetAsLastSibling();
        MoveTooltip(position);
    }

    internal void MoveTooltip(Vector2 position)
    {
        if (_tooltip == null || !_tooltip.activeSelf) return;
        if (_tooltipCanvasRect == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_tooltipCanvasRect, position, _canvasCamera, out var local);
        var rect = _tooltip.GetComponent<RectTransform>();
        var half = rect.sizeDelta * .5f;
        var showOnRight = local.x + rect.sizeDelta.x + 30 <= _tooltipCanvasRect.rect.xMax;
        var point = local + new Vector2(showOnRight ? half.x + 20 : -half.x - 20, half.y + 18);
        point.x = Mathf.Clamp(point.x, _tooltipCanvasRect.rect.xMin + half.x + 12, _tooltipCanvasRect.rect.xMax - half.x - 12);
        point.y = Mathf.Clamp(point.y, _tooltipCanvasRect.rect.yMin + half.y + 12, _tooltipCanvasRect.rect.yMax - half.y - 12);
        rect.anchoredPosition = point;
    }

    internal void HideTooltip()
    {
        if (_tooltip != null) _tooltip.SetActive(false);
    }

    private string CurrentLanguage => _translator?.TextLocalization?.ChosenLanguage?.Filename ?? "cn";

    private static void ConfigureBoardCanvas(Canvas canvas, Component owner)
    {
        var source = owner == null ? null : owner.GetComponentInParent<Canvas>();
        if (source == null || source.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            source = FindObjectsOfType<Canvas>()
                .Where(x => x != null && x != canvas &&
                            x.renderMode != RenderMode.ScreenSpaceOverlay && x.worldCamera != null)
                .OrderBy(x => x.sortingOrder)
                .FirstOrDefault();
        }
        canvas.renderMode = source?.renderMode ?? RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = source?.worldCamera ?? Camera.main;
        canvas.planeDistance = source?.planeDistance ?? 100;
        canvas.sortingLayerID = source?.sortingLayerID ?? 0;
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera == null)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    private static RectTransform MakeRail(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 position)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup));
        root.transform.SetParent(parent, false);
        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(1200, 48);
        var layout = root.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        root.SetActive(false);
        return rect;
    }

    private void BuildTooltip(Component owner)
    {
        var canvasObject = GameObject.Find("GameHoverTooltipCanvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("GameHoverTooltipCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            var canvas = canvasObject.GetComponent<Canvas>();
            ConfigureBoardCanvas(canvas, owner);
            // Above board labels/player names, below card details (sorting 4)
            // and explicit modal overlays.
            canvas.overrideSorting = true;
            canvas.sortingOrder = 2;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;
        }
        var tooltipCanvas = canvasObject.GetComponent<Canvas>();
        _canvasCamera = tooltipCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : tooltipCanvas.worldCamera;
        _tooltipCanvasRect = canvasObject.GetComponent<RectTransform>();
        _tooltip = new GameObject("ResourceTooltip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _tooltip.transform.SetParent(canvasObject.transform, false);
        var rect = _tooltip.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(420, 124);
        var image = _tooltip.GetComponent<Image>();
        image.color = new Color32(6, 19, 23, 250);
        image.raycastTarget = false;
        var outline = _tooltip.AddComponent<Outline>();
        outline.effectColor = new Color32(188, 137, 57, 190);
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

        _tooltipTitle = MakeText(_tooltip.transform, "Title", "", 19, TextAnchor.UpperLeft);
        _tooltipTitle.fontStyle = FontStyle.Bold;
        _tooltipTitle.color = new Color32(239, 220, 177, 255);
        _tooltipTitle.rectTransform.anchorMin = new Vector2(0, 1);
        _tooltipTitle.rectTransform.anchorMax = Vector2.one;
        _tooltipTitle.rectTransform.offsetMin = new Vector2(22, -49);
        _tooltipTitle.rectTransform.offsetMax = new Vector2(-18, -12);

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

        _tooltipDescription = MakeText(_tooltip.transform, "Description", "", 16, TextAnchor.UpperLeft);
        _tooltipDescription.rectTransform.anchorMin = Vector2.zero;
        _tooltipDescription.rectTransform.anchorMax = Vector2.one;
        _tooltipDescription.rectTransform.offsetMin = new Vector2(22, 14);
        _tooltipDescription.rectTransform.offsetMax = new Vector2(-18, -66);
        _tooltipDescription.horizontalOverflow = HorizontalWrapMode.Wrap;
        _tooltipDescription.verticalOverflow = VerticalWrapMode.Truncate;
        _tooltipDescription.lineSpacing = 1.12f;
        _tooltip.SetActive(false);
    }

    private static Text MakeText(Transform parent, string name, string content, int size, TextAnchor anchor)
    {
        var text = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        text.transform.SetParent(parent, false);
        text.font = _font;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = new Color32(239, 233, 215, 255);
        text.raycastTarget = false;
        text.text = content;
        return text;
    }

    private static string FormatValue(GameResourceState state, GameResourceDefinition definition)
    {
        if (string.Equals(definition?.ValueFormat, "fraction", System.StringComparison.OrdinalIgnoreCase) && state.Max.HasValue)
            return state.Value + "/" + state.Max.Value;
        return state.Value.ToString();
    }

    private static string FallbackLabel(string id)
        => string.IsNullOrWhiteSpace(id) ? "?" : id.Substring(0, 1).ToUpperInvariant();
}

public sealed class ResourceHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameResourcePanel _owner;
    private string _title;
    private string _description;
    private bool _hovered;

    public void Bind(GameResourcePanel owner, string title, string description)
    {
        _owner = owner;
        _title = title;
        _description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
        _owner?.ShowTooltip(_title, _description, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        _owner?.HideTooltip();
    }

    private void Update()
    {
        if (_hovered) _owner?.MoveTooltip(Input.mousePosition);
    }
}
