using System;
using System.Collections.Generic;
using Assets.Script.Localization;
using UnityEngine;
using UnityEngine.UI;

// A compact casual-match picker built solely from the extracted old-client
// GwentButton prefab. Selecting a row starts its forced AI match immediately.
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
        if (prefab == null || passwordRect == null || passwordRect.parent == null)
        {
            Debug.LogError("AI quick match requires Prefab/GwentButton and the casual-match password field.");
            enabled = false;
            return;
        }

        root = new GameObject("AiQuickMatch", typeof(RectTransform));
        root.layer = owner.MatchPasswordObject.layer;
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.SetParent(passwordRect.parent, false);
        rootRect.anchorMin = passwordRect.anchorMin;
        rootRect.anchorMax = passwordRect.anchorMax;
        rootRect.pivot = new Vector2(.5f, 0f);
        rootRect.sizeDelta = new Vector2(passwordRect.sizeDelta.x, TriggerHeight);
        rootRect.anchoredPosition = passwordRect.anchoredPosition + new Vector2(0f, 78f);
        root.transform.SetAsLastSibling();

        var trigger = MakeLegacyButton(prefab, root.transform, "AiQuickMatchTrigger", Vector2.zero, TriggerHeight);
        triggerLabel = trigger.GetComponentInChildren<Text>();
        trigger.GetComponent<Button>().onClick.AddListener(Toggle);

        popup = new GameObject("AiQuickMatchPopup", typeof(RectTransform));
        popup.layer = root.layer;
        var popupRect = popup.GetComponent<RectTransform>();
        popupRect.SetParent(root.transform, false);
        popupRect.anchorMin = popupRect.anchorMax = new Vector2(.5f, 0f);
        popupRect.pivot = new Vector2(.5f, 0f);
        popupRect.sizeDelta = new Vector2(rootRect.sizeDelta.x, Opponents.Count * RowHeight);
        popupRect.anchoredPosition = new Vector2(0f, TriggerHeight + 4f);

        foreach (var opponent in Opponents)
        {
            var current = opponent;
            var button = MakeLegacyButton(prefab, popup.transform, "AiQuickMatch-" + current.Index,
                new Vector2(0f, current.Index * RowHeight), RowHeight);
            var label = button.GetComponentInChildren<Text>();
            rows.Add(new Row { Opponent = current, Label = label });
            button.GetComponent<Button>().onClick.AddListener(() => StartOpponent(current));
        }
        popup.SetActive(false);
        RefreshLabels();
    }

    private static GameObject MakeLegacyButton(GameObject prefab, Transform parent, string name, Vector2 position, float height)
    {
        var button = Instantiate(prefab, parent, false);
        button.name = name;
        var rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, 0f);
        rect.pivot = new Vector2(.5f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(322f, height);
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
        if (triggerLabel != null) triggerLabel.text = LocalizedLabel.Get("MainMenu_PlayingvsAIText");
        foreach (var row in rows)
            if (row.Label != null)
                row.Label.text = string.Format("AI {0} - {1}", row.Opponent.Index, LocalizedLabel.Get(row.Opponent.NameKey));
    }
}
