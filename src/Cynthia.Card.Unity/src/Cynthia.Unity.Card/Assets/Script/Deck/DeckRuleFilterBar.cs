using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DeckRuleFilter
{
    All,
    Standard,
    Rules
}

public static class DeckRuleFilterBar
{
    private static readonly Color Normal = new Color32(27, 53, 57, 245);
    private static readonly Color Selected = new Color32(64, 105, 94, 255);
    private static readonly Color Hover = new Color32(45, 78, 79, 255);
    private static readonly Color Ink = new Color32(239, 233, 215, 255);

    public static GameObject Create(Transform parent, DeckRuleFilter current, Font font, bool english, Action<DeckRuleFilter> onChanged)
    {
        var root = new GameObject("DeckRuleFilterBar", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(DeckRuleFilterBarState));
        root.transform.SetParent(parent, false);
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 46);
        var layout = root.GetComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(5, 5, 4, 4);
        layout.spacing = 5;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        Add(DeckRuleFilter.All, english ? "All" : "全部");
        Add(DeckRuleFilter.Standard, english ? "Without rules" : "普通卡组");
        Add(DeckRuleFilter.Rules, english ? "With rules" : "带规则卡");
        return root;

        void Add(DeckRuleFilter value, string label)
        {
            var item = new GameObject(value.ToString(), typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            item.transform.SetParent(root.transform, false);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(112, 38);
            var image = item.GetComponent<Image>();
            image.color = value == current ? Selected : Normal;
            var button = item.GetComponent<Button>();
            button.targetGraphic = image;
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = value == current ? Selected : Normal;
            colors.highlightedColor = Hover;
            colors.pressedColor = new Color32(20, 43, 47, 255);
            colors.selectedColor = value == current ? Selected : Normal;
            colors.fadeDuration = .08f;
            button.colors = colors;
            button.onClick.AddListener(() => onChanged?.Invoke(value));
            root.GetComponent<DeckRuleFilterBarState>().Register(value, image, button);
            var textObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(item.transform, false);
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(4, 2);
            textRect.offsetMax = new Vector2(-4, -2);
            var text = textObject.GetComponent<Text>();
            text.font = font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 15;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Ink;
            text.text = label;
            text.raycastTarget = false;
        }
    }

    public static void SetCurrent(GameObject root, DeckRuleFilter current)
    {
        if (root == null) return;
        root.GetComponent<DeckRuleFilterBarState>()?.SetCurrent(current);
    }

    internal static Color NormalColor => Normal;
    internal static Color SelectedColor => Selected;
    internal static Color HoverColor => Hover;
}

public sealed class DeckRuleFilterBarState : MonoBehaviour
{
    private readonly Dictionary<DeckRuleFilter, Image> _images = new Dictionary<DeckRuleFilter, Image>();
    private readonly Dictionary<DeckRuleFilter, Button> _buttons = new Dictionary<DeckRuleFilter, Button>();

    public void Register(DeckRuleFilter filter, Image image, Button button)
    {
        _images[filter] = image;
        _buttons[filter] = button;
    }

    public void SetCurrent(DeckRuleFilter current)
    {
        foreach (var pair in _images)
        {
            var selected = pair.Key == current;
            pair.Value.color = selected ? DeckRuleFilterBar.SelectedColor : DeckRuleFilterBar.NormalColor;
            if (!_buttons.TryGetValue(pair.Key, out var button)) continue;
            var colors = button.colors;
            colors.normalColor = selected ? DeckRuleFilterBar.SelectedColor : DeckRuleFilterBar.NormalColor;
            colors.selectedColor = colors.normalColor;
            colors.highlightedColor = DeckRuleFilterBar.HoverColor;
            button.colors = colors;
        }
    }
}
