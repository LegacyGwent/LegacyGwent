using System;
using System.Collections.Generic;
using Assets.Script.Localization;
using UnityEngine;
using UnityEngine.UI;

// A compact casual-match picker built solely from the extracted old-client
// GwentButton prefab. Selecting a row starts its forced AI match immediately.
// The trigger is anchored under the whole card/password panel (CardShow in
// Game.unity) and the opponent list still unfolds upwards from it.
public sealed class AiQuickMatchSelector : MonoBehaviour
{
    public sealed class Opponent
    {
        public readonly int Index;
        public readonly string Password;
        public readonly string NameKey;

        public Opponent(int index, string password, string nameKey)
        {
            Index = index;
            Password = password;
            NameKey = nameKey;
        }
    }

    public static readonly IList<Opponent> Opponents = new List<Opponent>
    {
        new Opponent(0, "ai", "ai0_name"), new Opponent(1, "ai1", "ai1_name"),
        new Opponent(2, "ai2", "ai2_name"), new Opponent(3, "ai3", "ai3_name"),
        new Opponent(4, "ai4", "ai4_name"), new Opponent(5, "ai5", "ai5_name"),
    };

    private const string LegacyButtonPrefab = "Prefab/GwentButton";
    private const float TriggerHeight = 46f;
    private const float RowHeight = 42f;
    // The trigger hangs from the bottom edge of the whole card/password panel.
    private const float PanelGap = 12f;
    private const float PopupGap = 4f;
    private MatchInfo owner;
    private GameObject root;
    private GameObject popup;
    private Text triggerLabel;
    private readonly List<Row> rows = new List<Row>();

    private sealed class Row { public Opponent Opponent; public Text Label; }

    public static AiQuickMatchSelector Attach(MatchInfo matchInfo)
    {
        if (matchInfo == null || matchInfo.MatchUI == null) return null;
        var selector = matchInfo.MatchUI.GetComponent<AiQuickMatchSelector>();
        if (selector == null) selector = matchInfo.MatchUI.AddComponent<AiQuickMatchSelector>();
        selector.Initialize(matchInfo);
        return selector;
    }

    private void Initialize(MatchInfo matchInfo)
    {
        if (owner != null) return;
        owner = matchInfo;
        var prefab = Resources.Load<GameObject>(LegacyButtonPrefab);
        var passwordRect = owner.MatchPasswordObject == null ? null : owner.MatchPasswordObject.GetComponent<RectTransform>();
        // In Game.unity the casual-match password lives directly inside the framed
        // CardShow panel (anchored centre, 419x851). Taking the bounds from that
        // parent keeps the trigger under the real panel instead of at a fixed
        // offset above the password box.
        var panelRect = passwordRect == null ? null : passwordRect.parent as RectTransform;
        if (prefab == null || panelRect == null)
        {
            Debug.LogError("AI quick match requires Prefab/GwentButton and the casual-match password field.");
            enabled = false;
            return;
        }

        var panelWidth = panelRect.rect.width;
        if (panelWidth <= 0f && passwordRect != null) panelWidth = passwordRect.rect.width;

        root = new GameObject("AiQuickMatch", typeof(RectTransform));
        root.layer = owner.MatchPasswordObject.layer;
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.SetParent(panelRect, false);
        // Anchor to the panel's bottom edge and pivot at the top, so the trigger
        // always hangs just below the panel and can never cover the password box.
        // Equal left/right insets make it span the panel's full width.
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(.5f, 0f);
        rootRect.pivot = new Vector2(.5f, 1f);
        rootRect.sizeDelta = new Vector2(panelWidth, TriggerHeight);
        rootRect.anchoredPosition = new Vector2(0f, -PanelGap);
        root.transform.SetAsLastSibling();

        var trigger = MakeLegacyButton(prefab, root.transform, "AiQuickMatchTrigger", Vector2.zero, TriggerHeight, panelWidth);
        triggerLabel = trigger.GetComponentInChildren<Text>();
        trigger.GetComponent<Button>().onClick.AddListener(Toggle);

        popup = new GameObject("AiQuickMatchPopup", typeof(RectTransform));
        popup.layer = root.layer;
        var popupRect = popup.GetComponent<RectTransform>();
        popupRect.SetParent(root.transform, false);
        popupRect.anchorMin = popupRect.anchorMax = new Vector2(.5f, 0f);
        popupRect.pivot = new Vector2(.5f, 0f);
        popupRect.sizeDelta = new Vector2(panelWidth, Opponents.Count * RowHeight);
        // Rows keep unfolding upwards, exactly as they did before the move.
        popupRect.anchoredPosition = new Vector2(0f, TriggerHeight + PopupGap);

        foreach (var opponent in Opponents)
        {
            var current = opponent;
            var button = MakeLegacyButton(prefab, popup.transform, "AiQuickMatch-" + current.Index,
                new Vector2(0f, current.Index * RowHeight), RowHeight, panelWidth);
            var label = button.GetComponentInChildren<Text>();
            rows.Add(new Row { Opponent = current, Label = label });
            button.GetComponent<Button>().onClick.AddListener(() => StartOpponent(current));
        }
        popup.SetActive(false);
        RefreshLabels();
    }

    private static GameObject MakeLegacyButton(GameObject prefab, Transform parent, string name, Vector2 position, float height, float width)
    {
        var button = Instantiate(prefab, parent, false);
        button.name = name;
        var rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, 0f);
        rect.pivot = new Vector2(.5f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, height);
        return button;
    }

    public void SetVisible(bool visible)
    {
        if (root == null) return;
        if (!visible) Close();
        root.SetActive(visible);
    }

    public void Close() { if (popup != null) popup.SetActive(false); }

    private void Toggle()
    {
        if (owner == null || owner.IsRankMatch || owner.IsDoingMatch || popup == null) return;
        popup.SetActive(!popup.activeSelf);
    }

    private void StartOpponent(Opponent opponent)
    {
        if (owner == null || owner.IsRankMatch || owner.IsDoingMatch) return;
        Close();
        owner.StartAiQuickMatch(opponent.Password, forceAi: true);
    }

    private void OnEnable() { TextLocalization.LanguageChanged += RefreshLabels; RefreshLabels(); }
    private void OnDisable() { TextLocalization.LanguageChanged -= RefreshLabels; }

    private void RefreshLabels()
    {
        // The trigger owns its own label key; MainMenu_PlayingvsAIText stays reserved
        // for the online-count HUD statistic.
        if (triggerLabel != null) triggerLabel.text = LocalizedLabel.Get("MainMenu_PlayVsAIButton");
        foreach (var row in rows)
            if (row.Label != null)
                row.Label.text = string.Format("AI {0} - {1}", row.Opponent.Index, LocalizedLabel.Get(row.Opponent.NameKey));
    }
}
