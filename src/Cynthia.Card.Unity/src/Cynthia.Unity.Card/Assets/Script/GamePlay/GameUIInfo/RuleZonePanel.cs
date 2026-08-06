using System.Collections.Generic;
using System.Linq;
using Assets.Script.Localization;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// A read-only view over the cards in the two rule zones.  The launcher does not
/// exist visually for ordinary matches, so the legacy board remains unchanged.
/// </summary>
public sealed class RuleZonePanel : MonoBehaviour
{
    private static readonly Color Ink = new Color32(7, 20, 25, 245);
    private static readonly Color Panel = new Color32(18, 42, 47, 255);
    private static readonly Color Card = new Color32(27, 57, 61, 255);
    private static readonly Color Hover = new Color32(39, 76, 78, 255);
    private static readonly Color Gold = new Color32(222, 170, 78, 255);
    private static readonly Color Cream = new Color32(239, 233, 215, 255);

    private static Font _font;
    private LocalizationService _translator;
    private Button _launcher;
    private Text _launcherText;
    private Canvas _canvas;
    private GameObject _overlay;
    private Transform _content;
    private Text _detailName;
    private Text _detailText;
    private readonly List<CardStatus> _rules = new List<CardStatus>();
    private readonly Dictionary<string, GameRuleSource> _sources = new Dictionary<string, GameRuleSource>();
    private readonly Dictionary<string, Image> _rowBackgrounds = new Dictionary<string, Image>();
    private readonly Dictionary<string, Image> _rowAccents = new Dictionary<string, Image>();
    private readonly Dictionary<string, Button> _rowButtons = new Dictionary<string, Button>();
    private string _myPlayerName = "";
    private string _enemyPlayerName = "";
    private string _selectedRuleId = "";

    public static RuleZonePanel Attach(GameCardsControl owner)
    {
        var canvasObject = GameObject.Find("RuleZoneCanvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject(
                "RuleZoneCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 6;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;
        }
        var panel = canvasObject.GetComponent<RuleZonePanel>() ?? canvasObject.AddComponent<RuleZonePanel>();
        panel._canvas = canvasObject.GetComponent<Canvas>();
        panel.Initialize(canvasObject.transform);
        return panel;
    }

    private void Initialize(Transform canvas)
    {
        if (_launcher != null) return;
        _font = _font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        BuildLauncher(canvas);
        BuildOverlay(canvas);
        _launcher.gameObject.SetActive(false);
    }

    public void SetRules(
        IEnumerable<CardStatus> rules,
        IEnumerable<GameRuleSource> sources = null,
        string myPlayerName = null,
        string enemyPlayerName = null)
    {
        _rules.Clear();
        _rules.AddRange((rules ?? Enumerable.Empty<CardStatus>()).GroupBy(x => x.CardId).Select(x => x.First()));
        if (_rules.All(x => x.CardId != _selectedRuleId))
            _selectedRuleId = _rules.FirstOrDefault()?.CardId ?? "";
        _sources.Clear();
        foreach (var source in sources ?? Enumerable.Empty<GameRuleSource>())
            if (source != null && !string.IsNullOrWhiteSpace(source.CardId))
                _sources[source.CardId] = source;
        if (!string.IsNullOrWhiteSpace(myPlayerName)) _myPlayerName = myPlayerName;
        if (!string.IsNullOrWhiteSpace(enemyPlayerName)) _enemyPlayerName = enemyPlayerName;

        var total = _rules.Count;
        if (_launcher == null) return;
        _launcher.gameObject.SetActive(total > 0);
        _launcherText.text = IsEnglish ? $"RULES  {total}" : $"规则  {total}";
        if (total == 0) CloseOverlay();
        RebuildList();
    }

    private bool IsEnglish => _translator?.TextLocalization?.ChosenLanguage?.Filename == "en";

    private void BuildLauncher(Transform canvas)
    {
        var obj = new GameObject("RuleZoneLauncher", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        obj.transform.SetParent(canvas, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, .5f);
        rect.anchorMax = new Vector2(1, .5f);
        rect.pivot = new Vector2(1, .5f);
        rect.anchoredPosition = new Vector2(-34, 0);
        rect.sizeDelta = new Vector2(160, 48);
        var image = obj.GetComponent<Image>();
        image.color = Panel;
        _launcher = obj.GetComponent<Button>();
        _launcher.targetGraphic = image;
        _launcher.colors = Colors(Panel, Hover, new Color32(14, 35, 39, 255));
        _launcher.onClick.AddListener(() =>
        {
            if (_canvas != null) _canvas.sortingOrder = 900;
            _overlay.SetActive(true);
            _overlay.transform.SetAsLastSibling();
            ShowFirstRule();
        });
        _launcherText = MakeText(obj.transform, "Label", "", 18, TextAnchor.MiddleCenter, Cream);
        Stretch(_launcherText.rectTransform, 6, 6, 4, 4);
    }

    private void BuildOverlay(Transform canvas)
    {
        _overlay = new GameObject("RuleZoneOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _overlay.transform.SetParent(canvas, false);
        Stretch(_overlay.GetComponent<RectTransform>(), 0, 0, 0, 0);
        _overlay.GetComponent<Image>().color = new Color32(1, 8, 11, 220);

        var dialog = new GameObject("Dialog", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dialog.transform.SetParent(_overlay.transform, false);
        var dialogRect = dialog.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(.13f, .10f);
        dialogRect.anchorMax = new Vector2(.87f, .90f);
        dialogRect.offsetMin = Vector2.zero;
        dialogRect.offsetMax = Vector2.zero;
        dialog.GetComponent<Image>().color = Ink;

        var title = MakeText(dialog.transform, "Title", IsEnglish ? "MATCH RULES" : "本局规则", 30, TextAnchor.MiddleLeft, Cream);
        var titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(.5f, 1);
        titleRect.offsetMin = new Vector2(32, -72);
        titleRect.offsetMax = new Vector2(-90, -16);

        var close = MakeButton(dialog.transform, "Close", "×", 28);
        var closeRect = close.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.pivot = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(56, 46);
        closeRect.anchoredPosition = new Vector2(-20, -18);
        close.GetComponent<Button>().onClick.AddListener(CloseOverlay);

        var listPanel = new GameObject("ListPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        listPanel.transform.SetParent(dialog.transform, false);
        var listRect = listPanel.GetComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0, 0);
        listRect.anchorMax = new Vector2(.45f, 1);
        listRect.offsetMin = new Vector2(26, 26);
        listRect.offsetMax = new Vector2(-10, -84);
        listPanel.GetComponent<Image>().color = new Color32(10, 30, 35, 255);

        var scrollObject = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect));
        scrollObject.transform.SetParent(listPanel.transform, false);
        Stretch(scrollObject.GetComponent<RectTransform>(), 14, 14, 14, 14);
        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollObject.transform, false);
        Stretch(viewport.GetComponent<RectTransform>(), 0, 0, 0, 0);
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
        layout.spacing = 10;
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
        scroll.scrollSensitivity = 30;
        _content = content.transform;

        var detail = new GameObject("Detail", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        detail.transform.SetParent(dialog.transform, false);
        var detailRect = detail.GetComponent<RectTransform>();
        detailRect.anchorMin = new Vector2(.45f, 0);
        detailRect.anchorMax = Vector2.one;
        detailRect.offsetMin = new Vector2(10, 26);
        detailRect.offsetMax = new Vector2(-26, -84);
        detail.GetComponent<Image>().color = new Color32(19, 41, 45, 255);
        _detailName = MakeText(detail.transform, "CardName", "", 25, TextAnchor.UpperLeft, Gold);
        var nameRect = _detailName.rectTransform;
        nameRect.anchorMin = new Vector2(0, 1);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(.5f, 1);
        nameRect.offsetMin = new Vector2(26, -74);
        nameRect.offsetMax = new Vector2(-26, -20);
        _detailText = MakeText(detail.transform, "Description", "", 18, TextAnchor.UpperLeft, Cream);
        var textRect = _detailText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(26, 26);
        textRect.offsetMax = new Vector2(-26, -88);
        _overlay.SetActive(false);
    }

    private void CloseOverlay()
    {
        if (_overlay != null) _overlay.SetActive(false);
        if (_canvas != null) _canvas.sortingOrder = 6;
    }

    private void RebuildList()
    {
        if (_content == null) return;
        foreach (Transform child in _content) Destroy(child.gameObject);
        _rowBackgrounds.Clear();
        _rowAccents.Clear();
        _rowButtons.Clear();
        var both = _rules.Where(x => IsUsedBy(x.CardId, true) && IsUsedBy(x.CardId, false)).ToList();
        var mine = _rules.Where(x => IsUsedBy(x.CardId, true) && !IsUsedBy(x.CardId, false)).ToList();
        var enemy = _rules.Where(x => !IsUsedBy(x.CardId, true) && IsUsedBy(x.CardId, false)).ToList();
        var unknown = _rules.Where(x => !_sources.ContainsKey(x.CardId)).ToList();
        BuildSide(IsEnglish ? "BOTH PLAYERS" : "双方携带", both);
        BuildSide(PlayerHeading(_myPlayerName, true), mine);
        BuildSide(PlayerHeading(_enemyPlayerName, false), enemy);
        BuildSide(IsEnglish ? "ACTIVE RULES" : "生效规则", unknown);
    }

    private bool IsUsedBy(string cardId, bool mine)
    {
        if (!_sources.TryGetValue(cardId, out var source)) return false;
        return mine ? source.MyPlayerUses : source.EnemyPlayerUses;
    }

    private string PlayerHeading(string playerName, bool mine)
    {
        playerName = DisplayPlayerName(playerName);
        if (IsEnglish)
            return string.IsNullOrWhiteSpace(playerName)
                ? (mine ? "YOUR RULES" : "OPPONENT RULES")
                : (mine ? "YOU · " : "OPPONENT · ") + playerName.ToUpperInvariant();
        return string.IsNullOrWhiteSpace(playerName)
            ? (mine ? "我方规则" : "对方规则")
            : (mine ? "我方 · " : "对方 · ") + playerName;
    }

    private void BuildSide(string label, IReadOnlyCollection<CardStatus> rules)
    {
        if (rules.Count == 0) return;
        var heading = MakeText(_content, "Heading", label, 15, TextAnchor.MiddleLeft, Gold);
        heading.gameObject.AddComponent<LayoutElement>().preferredHeight = 30;
        foreach (var rule in rules) BuildRuleRow(rule);
    }

    private void BuildRuleRow(CardStatus rule)
    {
        var row = new GameObject("Rule-" + rule.CardId, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        row.transform.SetParent(_content, false);
        row.GetComponent<LayoutElement>().preferredHeight = 94;
        var background = row.GetComponent<Image>();
        background.color = rule.CardId == _selectedRuleId ? Hover : Card;
        _rowBackgrounds[rule.CardId] = background;
        var button = row.GetComponent<Button>();
        button.targetGraphic = background;
        button.colors = Colors(rule.CardId == _selectedRuleId ? Hover : Card, Hover, Panel);
        _rowButtons[rule.CardId] = button;
        button.onClick.AddListener(() => ShowRule(rule));

        var accent = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.transform.SetParent(row.transform, false);
        var accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0, 0);
        accentRect.anchorMax = new Vector2(0, 1);
        accentRect.pivot = new Vector2(0, .5f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(5, 0);
        var accentImage = accent.GetComponent<Image>();
        accentImage.color = Gold;
        accentImage.enabled = rule.CardId == _selectedRuleId;
        _rowAccents[rule.CardId] = accentImage;

        var art = new GameObject("Art", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        art.transform.SetParent(row.transform, false);
        var artRect = art.GetComponent<RectTransform>();
        artRect.anchorMin = new Vector2(0, .5f);
        artRect.anchorMax = new Vector2(0, .5f);
        artRect.pivot = new Vector2(0, .5f);
        artRect.anchoredPosition = new Vector2(16, 0);
        artRect.sizeDelta = new Vector2(54, 76);
        var artImage = art.GetComponent<Image>();
        artImage.color = new Color32(9, 24, 28, 255);
        artImage.preserveAspect = true;
        if (GwentMap.CardMap.TryGetValue(rule.CardId, out var cardInfo))
        {
            Addressables.LoadAssetAsync<Sprite>(cardInfo.CardArtsId).Completed += operation =>
            {
                if (artImage != null && operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    artImage.sprite = CreateCardPortrait(operation.Result);
                    artImage.color = Color.white;
                    artImage.preserveAspect = false;
                }
            };
        }

        var name = MakeText(row.transform, "Name", _translator.GetCardName(rule.CardId), 18, TextAnchor.MiddleLeft, Cream);
        var nameRect = name.rectTransform;
        nameRect.anchorMin = new Vector2(0, .46f);
        nameRect.anchorMax = Vector2.one;
        nameRect.offsetMin = new Vector2(84, 0);
        nameRect.offsetMax = new Vector2(-18, -10);

        var source = MakeText(row.transform, "Source", SourceLabel(rule.CardId), 14, TextAnchor.MiddleLeft, new Color32(161, 184, 181, 255));
        var sourceRect = source.rectTransform;
        sourceRect.anchorMin = Vector2.zero;
        sourceRect.anchorMax = new Vector2(1, .46f);
        sourceRect.offsetMin = new Vector2(84, 10);
        sourceRect.offsetMax = new Vector2(-18, 0);
    }

    private void ShowFirstRule()
    {
        var first = _rules.FirstOrDefault();
        if (first != null) ShowRule(first);
    }

    private void ShowRule(CardStatus rule)
    {
        _selectedRuleId = rule.CardId;
        foreach (var entry in _rowBackgrounds)
        {
            var normal = entry.Key == _selectedRuleId ? Hover : Card;
            entry.Value.color = normal;
            if (_rowButtons.TryGetValue(entry.Key, out var button))
                button.colors = Colors(normal, Hover, Panel);
        }
        foreach (var entry in _rowAccents)
            entry.Value.enabled = entry.Key == _selectedRuleId;
        _detailName.text = _translator.GetCardName(rule.CardId);
        _detailText.text = SourceLabel(rule.CardId) + "\n\n" + _translator.GetCardInfo(rule.CardId);
    }

    private string SourceLabel(string cardId)
    {
        if (!_sources.TryGetValue(cardId, out var source))
            return IsEnglish ? "Active match rule" : "本局生效规则";
        if (source.MyPlayerUses && source.EnemyPlayerUses)
            return IsEnglish ? "Carried by both players" : "双方均携带";
        var playerName = source.MyPlayerUses ? _myPlayerName : _enemyPlayerName;
        playerName = DisplayPlayerName(playerName);
        if (!string.IsNullOrWhiteSpace(playerName))
            return IsEnglish ? "Carried by " + playerName : "由 " + playerName + " 携带";
        return IsEnglish
            ? (source.MyPlayerUses ? "Carried by you" : "Carried by opponent")
            : (source.MyPlayerUses ? "由我方携带" : "由对方携带");
    }

    private string DisplayPlayerName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName)) return "";
        return _translator != null && _translator.IsContainsKey(playerName)
            ? _translator.GetText(playerName)
            : playerName;
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

    private static Sprite CreateCardPortrait(Sprite source)
    {
        if (source == null || source.texture == null) return source;
        // Legacy card art is stored on a square canvas whose painted card sits in
        // the upper-left region. Crop that portrait instead of shrinking the
        // surrounding black area into a tiny rule icon.
        var rect = source.rect;
        var width = rect.width * .49f;
        var height = rect.height * .70f;
        var crop = new Rect(rect.x, rect.y + rect.height - height, width, height);
        return Sprite.Create(source.texture, crop, new Vector2(.5f, .5f), source.pixelsPerUnit);
    }

    private static GameObject MakeButton(Transform parent, string name, string label, int size)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        var image = obj.GetComponent<Image>();
        image.color = Panel;
        var button = obj.GetComponent<Button>();
        button.targetGraphic = image;
        button.colors = Colors(Panel, Hover, Card);
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
