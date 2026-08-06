using System.Collections.Generic;
using System.Linq;
using Assets.Script.Localization;
using Cynthia.Card;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckShowInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text DeckText;
    public Sprite UnAvaliableIcon;
    public Sprite AvaliableIcon;
    public Image HeadIcon;
    public Image AvaliableShow;

    private readonly List<string> _ruleIds = new List<string>();
    private LocalizationService _translator;
    private GameFeatureManifest _manifest;
    private GameObject _badge;
    private GameObject _tooltip;
    private Vector2 _deckTextBasePosition;
    private int _deckTextBaseFontSize;
    private bool _deckTextMetricsCaptured;
    private bool _hovering;

    public void SetDeckInfo(string name, bool isAvaliable)
    {
        AvaliableShow.sprite = isAvaliable ? AvaliableIcon : UnAvaliableIcon;
        DeckText.text = name;
    }

    internal void SetRuleInfo(DeckModel deck, GameFeatureManifest manifest, LocalizationService translator)
    {
        _translator = translator;
        _manifest = manifest ?? new GameFeatureManifest();
        _ruleIds.Clear();
        _ruleIds.AddRange((deck?.Deck ?? new List<string>())
            .Where(DeckRuleEngine.IsRuleCard)
            .Distinct(System.StringComparer.Ordinal));
        EnsureBadge();
        _badge.SetActive(_ruleIds.Count > 0);
        RestoreDeckTextLayout();
        if (_ruleIds.Count > 0)
        {
            var english = _translator?.TextLocalization?.ChosenLanguage?.Filename == "en";
            _badge.GetComponent<Text>().text = english
                ? $"INCLUDES {_ruleIds.Count} RULE CARD{(_ruleIds.Count > 1 ? "S" : "")}"
                : $"含 {_ruleIds.Count} 张规则卡";
            if (DeckText != null)
            {
                DeckText.rectTransform.anchoredPosition = _deckTextBasePosition + new Vector2(0, 9);
                DeckText.fontSize = Mathf.Max(13, _deckTextBaseFontSize - 2);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovering = true;
        if (_ruleIds.Count > 0) ShowTooltip(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
        HideTooltip();
    }

    private void Update()
    {
        if (_hovering && _ruleIds.Count > 0 && _tooltip != null && _tooltip.activeSelf)
            MoveTooltip(Input.mousePosition);
    }

    private void EnsureBadge()
    {
        if (_badge != null) return;
        CaptureDeckTextLayout();
        _badge = new GameObject("RuleDeckStatus", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        _badge.transform.SetParent(transform, false);
        var rect = _badge.GetComponent<RectTransform>();
        var sourceRect = DeckText != null ? DeckText.rectTransform : null;
        rect.anchorMin = sourceRect != null ? sourceRect.anchorMin : new Vector2(.5f, .5f);
        rect.anchorMax = sourceRect != null ? sourceRect.anchorMax : new Vector2(.5f, .5f);
        rect.pivot = sourceRect != null ? sourceRect.pivot : new Vector2(.5f, .5f);
        rect.anchoredPosition = (sourceRect != null ? _deckTextBasePosition : Vector2.zero) + new Vector2(0, -13);
        rect.sizeDelta = sourceRect != null ? new Vector2(sourceRect.sizeDelta.x, 17) : new Vector2(170, 17);
        var text = _badge.GetComponent<Text>();
        text.font = DeckText != null ? DeckText.font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = DeckText != null ? Mathf.Max(10, DeckText.fontSize - 6) : 12;
        text.fontStyle = FontStyle.Normal;
        text.alignment = DeckText != null ? DeckText.alignment : TextAnchor.MiddleCenter;
        text.color = new Color32(226, 181, 91, 255);
        text.raycastTarget = false;
    }

    private void CaptureDeckTextLayout()
    {
        if (_deckTextMetricsCaptured || DeckText == null) return;
        _deckTextBasePosition = DeckText.rectTransform.anchoredPosition;
        _deckTextBaseFontSize = DeckText.fontSize;
        _deckTextMetricsCaptured = true;
    }

    private void RestoreDeckTextLayout()
    {
        CaptureDeckTextLayout();
        if (!_deckTextMetricsCaptured || DeckText == null) return;
        DeckText.rectTransform.anchoredPosition = _deckTextBasePosition;
        DeckText.fontSize = _deckTextBaseFontSize;
    }

    private void ShowTooltip(Vector2 screenPosition)
    {
        BuildTooltip();
        if (_tooltip == null) return;
        _tooltip.SetActive(true);
        _tooltip.transform.SetAsLastSibling();
        MoveTooltip(screenPosition);
    }

    private void MoveTooltip(Vector2 screenPosition)
    {
        if (_tooltip == null) return;
        var canvas = _tooltip.GetComponentInParent<Canvas>();
        var canvasRect = canvas != null ? canvas.transform as RectTransform : null;
        if (canvasRect == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvas.worldCamera, out var point);
        var rect = _tooltip.GetComponent<RectTransform>();
        var half = rect.sizeDelta * .5f;
        var showOnRight = point.x + half.x * 2f + 28f <= canvasRect.rect.xMax;
        point += new Vector2(showOnRight ? half.x + 22f : -half.x - 22f, -half.y - 16f);
        point.x = Mathf.Clamp(point.x, canvasRect.rect.xMin + half.x + 12, canvasRect.rect.xMax - half.x - 12);
        point.y = Mathf.Clamp(point.y, canvasRect.rect.yMin + half.y + 12, canvasRect.rect.yMax - half.y - 12);
        rect.anchoredPosition = point;
    }

    private void BuildTooltip()
    {
        if (_tooltip != null || _ruleIds.Count == 0) return;
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        _tooltip = new GameObject("DeckRuleTooltip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        _tooltip.transform.SetParent(canvas.transform, false);
        var rect = _tooltip.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(430, Mathf.Min(390, 58 + _ruleIds.Count * 74));
        _tooltip.GetComponent<Image>().color = new Color32(8, 25, 29, 252);

        var content = new GameObject("Content", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        content.transform.SetParent(_tooltip.transform, false);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(20, 15);
        contentRect.offsetMax = new Vector2(-20, -15);
        var text = content.GetComponent<Text>();
        text.font = DeckText != null ? DeckText.font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 16;
        text.alignment = TextAnchor.UpperLeft;
        text.color = new Color32(239, 233, 215, 255);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.text = BuildSummary();
        text.raycastTarget = false;
        _tooltip.SetActive(false);
    }

    private string BuildSummary()
    {
        var english = _translator?.TextLocalization?.ChosenLanguage?.Filename == "en";
        var lines = new List<string> { english ? "RULE CARDS" : "规则卡" };
        foreach (var id in _ruleIds)
        {
            var definition = _manifest.RuleCards.FirstOrDefault(x => x.Id == id);
            var name = definition != null ? definition.Name.Resolve(english ? "en" : "zh-CN") : _translator.GetCardName(id);
            var description = definition != null ? definition.Description.Resolve(english ? "en" : "zh-CN") : _translator.GetCardInfo(id);
            if (description != null && description.Length > 74) description = description.Substring(0, 74) + "…";
            lines.Add("\n◆ " + name + "\n" + description);
        }
        return string.Join("\n", lines);
    }

    private void HideTooltip()
    {
        if (_tooltip != null) _tooltip.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_tooltip != null) Destroy(_tooltip);
    }
}
