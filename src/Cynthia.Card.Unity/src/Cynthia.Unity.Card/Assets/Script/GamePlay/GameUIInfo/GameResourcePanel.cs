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
    private Text _tooltipText;

    public static GameResourcePanel Attach(GameUIControl owner)
    {
        if (owner == null) return null;
        var panel = owner.GetComponent<GameResourcePanel>() ?? owner.gameObject.AddComponent<GameResourcePanel>();
        panel.Initialize();
        return panel;
    }

    private void Initialize()
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
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 470;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;
        }
        _myRoot = MakeRail(canvasObject.transform, "MyResources", new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 150));
        _enemyRoot = MakeRail(canvasObject.transform, "EnemyResources", new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -150));
        BuildTooltip(canvasObject.transform);
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
        _tooltipText.text = string.IsNullOrWhiteSpace(title) ? description : title + "\n" + description;
        _tooltip.SetActive(true);
        var rect = _tooltip.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform, position, null, out var local);
        rect.anchoredPosition = local + new Vector2(18, 18);
        _tooltip.transform.SetAsLastSibling();
    }

    internal void HideTooltip()
    {
        if (_tooltip != null) _tooltip.SetActive(false);
    }

    private string CurrentLanguage => _translator?.TextLocalization?.ChosenLanguage?.Filename ?? "cn";

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

    private void BuildTooltip(Transform parent)
    {
        _tooltip = new GameObject("ResourceTooltip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _tooltip.transform.SetParent(parent, false);
        var rect = _tooltip.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = Vector2.zero;
        rect.sizeDelta = new Vector2(360, 96);
        var image = _tooltip.GetComponent<Image>();
        image.color = new Color32(6, 20, 25, 248);
        image.raycastTarget = false;
        _tooltipText = MakeText(_tooltip.transform, "Text", "", 15, TextAnchor.MiddleLeft);
        _tooltipText.rectTransform.anchorMin = Vector2.zero;
        _tooltipText.rectTransform.anchorMax = Vector2.one;
        _tooltipText.rectTransform.offsetMin = new Vector2(14, 10);
        _tooltipText.rectTransform.offsetMax = new Vector2(-14, -10);
        _tooltipText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _tooltipText.verticalOverflow = VerticalWrapMode.Truncate;
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

    public void Bind(GameResourcePanel owner, string title, string description)
    {
        _owner = owner;
        _title = title;
        _description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
        => _owner?.ShowTooltip(_title, _description, eventData.position);

    public void OnPointerExit(PointerEventData eventData) => _owner?.HideTooltip();
}
