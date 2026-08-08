using System;
using System.Collections.Generic;
using System.Linq;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ServerModeMenu : MonoBehaviour
{
    private static readonly Color Ink = new Color32(12, 25, 31, 248);
    private static readonly Color Panel = new Color32(21, 42, 48, 255);
    private static readonly Color Card = new Color32(29, 54, 59, 255);
    private static readonly Color CardHover = new Color32(38, 69, 73, 255);
    private static readonly Color Gold = new Color32(222, 170, 78, 255);
    private static readonly Color Cream = new Color32(238, 231, 210, 255);
    private static Font _font;

    private MatchInfo _owner;
    private GameObject _overlay;
    private Button _launcher;
    private Text _launcherText;
    private readonly Dictionary<string, Image> _rowBackgrounds = new Dictionary<string, Image>();
    private readonly Dictionary<string, Image> _rowMarkers = new Dictionary<string, Image>();
    private readonly Dictionary<string, Button> _rowButtons = new Dictionary<string, Button>();
    private List<GameModeDefinition> _modes = new List<GameModeDefinition>();

    public string SelectedModeId { get; private set; }
    public bool HasModes => _modes.Count > 0;
    public GameModeDefinition SelectedMode => _modes.FirstOrDefault(x => x.Id == SelectedModeId);

    public static ServerModeMenu Attach(MatchInfo owner)
    {
        var component = owner.MatchUI.GetComponent<ServerModeMenu>() ?? owner.MatchUI.AddComponent<ServerModeMenu>();
        component.Initialize(owner);
        return component;
    }

    private void Initialize(MatchInfo owner)
    {
        _owner = owner;
        _font = _font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        RefreshFromManifest();
        if (HasModes) Select(SelectedModeId);
    }

    // The server manifest is the source of truth for the mode catalogue. Rebuild
    // the generated rows whenever that manifest is refreshed instead of keeping
    // the snapshot captured when this component was first attached.
    public void RefreshFromManifest()
    {
        var previousModeId = SelectedModeId;
        _modes = (_owner.Client.FeatureManifest?.Modes ?? new List<GameModeDefinition>())
            // Rule cards are a property of the selected deck, not a standalone
            // advertised game mode. Older manifests may still expose this entry.
            .Where(x => x.IsEnabled && x.Id != "pvp.rules")
            .OrderBy(x => x.SortOrder)
            .ToList();

        if (_overlay != null)
        {
            _overlay.SetActive(false);
            Destroy(_overlay);
            _overlay = null;
        }
        _rowBackgrounds.Clear();
        _rowMarkers.Clear();
        _rowButtons.Clear();

        if (_modes.Count == 0)
        {
            SelectedModeId = null;
            if (_launcher != null) _launcher.gameObject.SetActive(false);
            return;
        }

        SelectedModeId = _modes.Any(x => x.Id == previousModeId)
            ? previousModeId
            : _modes.FirstOrDefault(x => x.Id == "pvp.casual")?.Id ?? _modes[0].Id;
        if (_launcher == null) BuildLauncher();
        BuildOverlay();
        UpdateSelectionVisuals(SelectedMode);
    }

    public void SetVisible(bool visible)
    {
        if (_launcher != null) _launcher.gameObject.SetActive(visible && HasModes);
        if (!visible && _overlay != null) _overlay.SetActive(false);
    }

    public void SelectDefault(string modeId)
    {
        if (!HasModes) return;
        Select(_modes.Any(x => x.Id == modeId) ? modeId : SelectedModeId ?? _modes[0].Id);
    }

    public void Open()
    {
        if (_overlay == null) return;
        _overlay.SetActive(true);
        _overlay.transform.SetAsLastSibling();
    }

    private void BuildLauncher()
    {
        var launcherObject = new GameObject("ServerModeLauncher", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
        launcherObject.transform.SetParent(_owner.MatchPasswordObject.transform.parent, false);
        var source = _owner.MatchPasswordObject.GetComponent<RectTransform>();
        var rect = launcherObject.GetComponent<RectTransform>();
        rect.anchorMin = source.anchorMin;
        rect.anchorMax = source.anchorMax;
        rect.pivot = source.pivot;
        rect.anchoredPosition = source.anchoredPosition + new Vector2(0, 82);
        rect.sizeDelta = source.sizeDelta;
        var image = launcherObject.GetComponent<Image>();
        image.color = new Color32(31, 57, 62, 255);
        _launcher = launcherObject.GetComponent<Button>();
        _launcher.targetGraphic = image;
        _launcher.colors = Colors(image.color, new Color32(58, 91, 91, 255), new Color32(24, 46, 51, 255));
        _launcher.onClick.AddListener(Open);

        _launcherText = MakeText(launcherObject.transform, "Label", "", 22, TextAnchor.MiddleLeft, Cream);
        Stretch(_launcherText.rectTransform, 24, 54, 6, 6);
        var arrow = MakeText(launcherObject.transform, "Arrow", "›", 30, TextAnchor.MiddleCenter, Gold);
        var arrowRect = arrow.rectTransform;
        arrowRect.anchorMin = new Vector2(1, 0);
        arrowRect.anchorMax = new Vector2(1, 1);
        arrowRect.pivot = new Vector2(1, .5f);
        arrowRect.sizeDelta = new Vector2(48, 0);
        arrowRect.anchoredPosition = Vector2.zero;
        launcherObject.AddComponent<StableModeHover>().Bind(launcherObject.GetComponent<Outline>(), arrowRect);
    }

    private void BuildOverlay()
    {
        _overlay = new GameObject("ServerModeOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasRenderer), typeof(GraphicRaycaster), typeof(Image));
        _overlay.transform.SetParent(_owner.MatchUI.transform, false);
        var overlayCanvas = _overlay.GetComponent<Canvas>();
        overlayCanvas.overrideSorting = true;
        overlayCanvas.sortingOrder = 1200;
        Stretch(_overlay.GetComponent<RectTransform>(), 0, 0, 0, 0);
        _overlay.GetComponent<Image>().color = new Color32(2, 9, 12, 210);

        var dialog = new GameObject("Dialog", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dialog.transform.SetParent(_overlay.transform, false);
        var dialogRect = dialog.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(.16f, .10f);
        dialogRect.anchorMax = new Vector2(.84f, .90f);
        dialogRect.offsetMin = Vector2.zero;
        dialogRect.offsetMax = Vector2.zero;
        dialog.GetComponent<Image>().color = Ink;

        var accent = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.transform.SetParent(dialog.transform, false);
        var accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0, 1);
        accentRect.anchorMax = Vector2.one;
        accentRect.pivot = new Vector2(.5f, 1);
        accentRect.sizeDelta = new Vector2(0, 5);
        accentRect.anchoredPosition = Vector2.zero;
        accent.GetComponent<Image>().color = Gold;

        var title = MakeText(dialog.transform, "Title", Resolve(new LocalizedText { ZhCn = "选择对战模式", En = "Choose a mode" }), 32, TextAnchor.MiddleLeft, Cream);
        var titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(.5f, 1);
        titleRect.offsetMin = new Vector2(34, -78);
        titleRect.offsetMax = new Vector2(-100, -16);

        var subtitle = MakeText(dialog.transform, "Subtitle",
            Resolve(new LocalizedText { ZhCn = "模式由服务器提供，后续内容无需重新安装客户端。", En = "Modes are provided by the server and can evolve without reinstalling." }),
            16, TextAnchor.MiddleLeft, new Color32(160, 178, 177, 255));
        var subtitleRect = subtitle.rectTransform;
        subtitleRect.anchorMin = new Vector2(0, 1);
        subtitleRect.anchorMax = new Vector2(1, 1);
        subtitleRect.pivot = new Vector2(.5f, 1);
        subtitleRect.offsetMin = new Vector2(36, -110);
        subtitleRect.offsetMax = new Vector2(-100, -74);

        var close = MakeButton(dialog.transform, "Close", "×", 26);
        var closeRect = close.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.pivot = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(58, 48);
        closeRect.anchoredPosition = new Vector2(-20, -18);
        close.GetComponent<Button>().onClick.AddListener(() => _overlay.SetActive(false));

        var scrollObject = new GameObject("ModeScroll", typeof(RectTransform), typeof(ScrollRect));
        scrollObject.transform.SetParent(dialog.transform, false);
        var scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = Vector2.zero;
        scrollRectTransform.anchorMax = Vector2.one;
        scrollRectTransform.offsetMin = new Vector2(28, 28);
        scrollRectTransform.offsetMax = new Vector2(-28, -122);

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollObject.transform, false);
        Stretch(viewport.GetComponent<RectTransform>(), 0, 18, 0, 0);
        viewport.GetComponent<Image>().color = new Color(0, 0, 0, .01f);
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(.5f, 1);
        contentRect.sizeDelta = Vector2.zero;
        var layout = content.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 12;
        layout.padding = new RectOffset(2, 2, 2, 12);
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = scrollObject.GetComponent<ScrollRect>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 32;

        var scrollbarObject = new GameObject("Scrollbar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Scrollbar));
        scrollbarObject.transform.SetParent(scrollObject.transform, false);
        var scrollbarRect = scrollbarObject.GetComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = Vector2.one;
        scrollbarRect.pivot = new Vector2(1, .5f);
        scrollbarRect.sizeDelta = new Vector2(8, 0);
        scrollbarRect.anchoredPosition = Vector2.zero;
        scrollbarObject.GetComponent<Image>().color = new Color32(12, 31, 36, 220);
        var slidingArea = new GameObject("Sliding Area", typeof(RectTransform));
        slidingArea.transform.SetParent(scrollbarObject.transform, false);
        Stretch(slidingArea.GetComponent<RectTransform>(), 0, 0, 0, 0);
        var handle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        handle.transform.SetParent(slidingArea.transform, false);
        Stretch(handle.GetComponent<RectTransform>(), 0, 0, 0, 0);
        var handleImage = handle.GetComponent<Image>();
        handleImage.color = new Color32(92, 123, 113, 235);
        var scrollbar = scrollbarObject.GetComponent<Scrollbar>();
        scrollbar.handleRect = handle.GetComponent<RectTransform>();
        scrollbar.targetGraphic = handleImage;
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scroll.verticalScrollbar = scrollbar;
        scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        scroll.verticalScrollbarSpacing = 5;

        string lastCategory = null;
        foreach (var mode in _modes)
        {
            if (lastCategory != mode.Category)
            {
                var category = MakeText(content.transform, "Category-" + mode.Category,
                    ResolveCategoryName(mode),
                    16, TextAnchor.MiddleLeft, Gold);
                category.gameObject.AddComponent<LayoutElement>().preferredHeight = 34;
                lastCategory = mode.Category;
            }
            BuildModeRow(content.transform, mode);
        }
        _overlay.SetActive(false);
    }

    private void BuildModeRow(Transform parent, GameModeDefinition mode)
    {
        var row = new GameObject("Mode-" + mode.Id, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement), typeof(Outline));
        row.transform.SetParent(parent, false);
        row.GetComponent<LayoutElement>().preferredHeight = 88;
        var background = row.GetComponent<Image>();
        background.color = Card;
        _rowBackgrounds[mode.Id] = background;
        var button = row.GetComponent<Button>();
        button.targetGraphic = background;
        button.colors = Colors(Card, new Color32(53, 88, 89, 255), new Color32(23, 44, 49, 255));
        row.AddComponent<StableModeHover>().Bind(row.GetComponent<Outline>());
        _rowButtons[mode.Id] = button;
        button.onClick.AddListener(() =>
        {
            Select(mode.Id);
            _overlay.SetActive(false);
        });

        var marker = new GameObject("Marker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        marker.transform.SetParent(row.transform, false);
        var markerRect = marker.GetComponent<RectTransform>();
        markerRect.anchorMin = Vector2.zero;
        markerRect.anchorMax = new Vector2(0, 1);
        markerRect.pivot = new Vector2(0, .5f);
        markerRect.sizeDelta = new Vector2(5, 0);
        var markerImage = marker.GetComponent<Image>();
        markerImage.color = Gold;
        markerImage.enabled = mode.Id == SelectedModeId;
        _rowMarkers[mode.Id] = markerImage;

        var name = MakeText(row.transform, "Name", Resolve(mode.Name), 23, TextAnchor.MiddleLeft, Cream);
        var nameRect = name.rectTransform;
        nameRect.anchorMin = new Vector2(0, .48f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = new Vector2(24, 0);
        nameRect.offsetMax = new Vector2(-118, -8);

        var description = MakeText(row.transform, "Description", Resolve(mode.Description), 15, TextAnchor.UpperLeft, new Color32(167, 184, 181, 255));
        var descriptionRect = description.rectTransform;
        descriptionRect.anchorMin = Vector2.zero;
        descriptionRect.anchorMax = new Vector2(1, .52f);
        descriptionRect.offsetMin = new Vector2(25, 10);
        descriptionRect.offsetMax = new Vector2(-118, 0);

        var type = MakeText(row.transform, "Type", ResolveTypeLabel(mode), 14, TextAnchor.MiddleCenter, Gold);
        var typeRect = type.rectTransform;
        typeRect.anchorMin = new Vector2(1, .5f);
        typeRect.anchorMax = new Vector2(1, .5f);
        typeRect.pivot = new Vector2(1, .5f);
        typeRect.sizeDelta = new Vector2(72, 30);
        typeRect.anchoredPosition = new Vector2(-22, 0);
    }

    private void Select(string id)
    {
        var mode = _modes.FirstOrDefault(x => x.Id == id);
        if (mode == null) return;
        SelectedModeId = id;
        UpdateSelectionVisuals(mode);
        _owner.OnServerModeSelected(mode);
    }

    private void UpdateSelectionVisuals(GameModeDefinition mode)
    {
        if (mode == null || _launcherText == null) return;
        _launcherText.text = (Resolve(new LocalizedText { ZhCn = "模式 · ", En = "MODE · " })) + Resolve(mode.Name);
        foreach (var pair in _rowBackgrounds)
        {
            var selected = pair.Key == mode.Id;
            var normal = selected ? (Color)new Color32(45, 75, 75, 255) : Card;
            pair.Value.color = normal;
            if (_rowButtons.TryGetValue(pair.Key, out var button))
                button.colors = Colors(normal, CardHover, new Color32(23, 44, 49, 255));
            if (_rowMarkers.TryGetValue(pair.Key, out var marker))
                marker.enabled = selected;
        }
    }

    private string Resolve(LocalizedText text) => _owner.ResolveLocalized(text);

    private string ResolveCategoryName(GameModeDefinition mode)
    {
        if (mode?.CategoryName != null &&
            (!string.IsNullOrWhiteSpace(mode.CategoryName.ZhCn) || !string.IsNullOrWhiteSpace(mode.CategoryName.En)))
            return Resolve(mode.CategoryName);
        if (string.Equals(mode?.Category, "ai", StringComparison.OrdinalIgnoreCase))
            return Resolve(new LocalizedText { ZhCn = "人机试炼", En = "AI challenges" });
        if (string.Equals(mode?.Category, "pvp", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(mode?.Category))
            return Resolve(new LocalizedText { ZhCn = "玩家对战", En = "Player versus player" });
        return mode.Category;
    }

    private string ResolveTypeLabel(GameModeDefinition mode)
    {
        if (mode?.TypeLabel != null &&
            (!string.IsNullOrWhiteSpace(mode.TypeLabel.ZhCn) || !string.IsNullOrWhiteSpace(mode.TypeLabel.En)))
            return Resolve(mode.TypeLabel);
        return string.Equals(mode?.MatchKind, "ai", StringComparison.OrdinalIgnoreCase) ? "AI" : "PVP";
    }

    private static Text MakeText(Transform parent, string name, string value, int size, TextAnchor alignment, Color color)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        obj.transform.SetParent(parent, false);
        var text = obj.GetComponent<Text>();
        text.font = _font;
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static GameObject MakeButton(Transform parent, string name, string label, int size)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        var image = obj.GetComponent<Image>();
        image.color = Panel;
        var button = obj.GetComponent<Button>();
        button.targetGraphic = image;
        button.colors = Colors(Panel, CardHover, Card);
        var text = MakeText(obj.transform, "Label", label, size, TextAnchor.MiddleCenter, Cream);
        Stretch(text.rectTransform, 0, 0, 0, 0);
        return obj;
    }

    private static ColorBlock Colors(Color normal, Color highlighted, Color pressed)
    {
        var colors = ColorBlock.defaultColorBlock;
        colors.normalColor = normal;
        colors.highlightedColor = highlighted;
        colors.pressedColor = pressed;
        colors.selectedColor = highlighted;
        colors.disabledColor = new Color(normal.r, normal.g, normal.b, .45f);
        colors.colorMultiplier = 1;
        colors.fadeDuration = .08f;
        return colors;
    }

    private static void Stretch(RectTransform rect, float left, float right, float bottom, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}

// Strong, size-stable hover feedback. It deliberately avoids transform scaling,
// which made neighbouring controls twitch in earlier menus.
public sealed class StableModeHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline _outline;
    private RectTransform _arrow;
    private Vector2 _arrowHome;

    public void Bind(Outline outline, RectTransform arrow = null)
    {
        _outline = outline;
        _arrow = arrow;
        if (_arrow != null) _arrowHome = _arrow.anchoredPosition;
        if (_outline != null)
        {
            _outline.effectDistance = new Vector2(2, -2);
            _outline.effectColor = new Color32(222, 170, 78, 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_outline != null) _outline.effectColor = new Color32(222, 170, 78, 230);
        if (_arrow != null) _arrow.anchoredPosition = _arrowHome + new Vector2(-6, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_outline != null) _outline.effectColor = new Color32(222, 170, 78, 0);
        if (_arrow != null) _arrow.anchoredPosition = _arrowHome;
    }
}
