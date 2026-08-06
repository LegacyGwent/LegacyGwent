using System.Collections;
using System.Collections.Generic;
using Cynthia.Card.Client;
using Cynthia.Card;
using UnityEngine;
using Alsein.Extensions;
using System.Linq;
using Autofac;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Assets.Script.Localization;

public class MatchInfo : MonoBehaviour
{
    public ArtCard ShowArtCard;
    public GameObject BlacklistObject;
    public Toggle RecordStatus;
    public Text BlacklistMessage;

    public GameObject LaderPrefab;
    public GameObject CardPrefab;
    public GameObject DeckPrefab;
    //-------------------------------------------
    public Transform CardsContext;
    public Transform DecksContext;
    public Text GoldCount;
    public Text SilverCount;
    public Text CopperCount;
    public Text AllCount;
    public Text AllCountText;
    public Image HeadT;
    public Image HeadB;
    public Sprite[] HeadTSprite;
    public Sprite[] HeadBSprite;

    public Text DeckName;
    public InputField MatchPassword;
    public GameObject MatchPasswordObject;
    public Transform DeckNameBackground;
    public Image DeckIcon;
    //-------------------------------------------
    public Sprite[] FactionIcon;
    public Faction[] FactionIndex;
    //-------------------------------------------
    public GameObject ReturnButton;
    public GameObject SwitchButton;
    public GameObject CloseButton;
    public GameObject MatchButton;
    public GameObject DeckSwitch;
    public GameObject CardsScrollbar;
    public GameObject DecksScrollbar;
    //
    public GameObject[] DeckPrefabs;
    //
    public Text MatchButtonText;
    public Text MatchMessage;
    public Text MainMenu_MatchTitle;
    //-------------------------------------------
    public GameObject MainUI;
    public GameObject MatchUI;
    public MainMenuEffect[] ResetTextMenus;
    //-------------------------------------------
    public string CurrentDeckId { get; private set; }
    public bool IsDoingMatch { get; private set; }
    public bool IsRankMatch { get; private set; }

    private GwentClientService _client { get => DependencyResolver.Container.Resolve<GwentClientService>(); }
    private GlobalUIService _UIService { get => DependencyResolver.Container.Resolve<GlobalUIService>(); }
    private LocalizationService _translator { get => DependencyResolver.Container.Resolve<LocalizationService>(); }
    public GwentClientService Client => _client;
    private ServerModeMenu _serverModeMenu;
    private DeckRuleFilter _deckRuleFilter = DeckRuleFilter.All;


    public async void MatchMenuClick()
    {
        await _client.RefreshGameFeatureManifest();
        var availableDecks = GetPlayerVisibleDecks().ToList();
        if (availableDecks.Count <= 0)
        {
            await _UIService.YNMessageBox(_translator.GetText("PopupWindow_NoDeckTitle"),
                _translator.GetText("PopupWindow_NoDeckDesc"), isOnlyYes: true);
            return;
        }
        else
        {
            ResetMatch();
            MainUI.SetActive(false);
            MatchUI.SetActive(true);
            ResetTextMenus.ForAll(x => x.TextReset());
        }
        if (IsRankMatch)
        {
            MainMenu_MatchTitle.text = _translator.GetText("MainMenu_MatchTitle_Rank");
            MatchPasswordObject.SetActive(false);
            BlacklistObject.SetActive(false);
        }
        else
        {
            MainMenu_MatchTitle.text = _translator.GetText("MainMenu_MatchTitle");
            MatchPasswordObject.SetActive(true);
            BlacklistObject.SetActive(true);
        }
        if (_serverModeMenu != null && _serverModeMenu.HasModes)
        {
            _serverModeMenu.SetVisible(true);
            _serverModeMenu.SelectDefault(IsRankMatch ? "pvp.ranked" : "pvp.casual");
            var mode = _serverModeMenu.SelectedMode;
            MatchPasswordObject.SetActive(!IsRankMatch && (mode == null || mode.MatchKind == "pvp"));
        }
    }
    public void NormalMatchMenuClick()
    {
        IsRankMatch = false;
        MatchMenuClick();
    }
    public void RankMatchMenuClick()
    {
        IsRankMatch = true;
        MatchMenuClick();
    }
    public void ResetMatch()
    {
        Debug.Log("重置");
        var availableDecks = GetPlayerVisibleDecks().ToList();
        if (availableDecks.Count == 0) return;
        if (!availableDecks.Any(x => x.Id == ClientGlobalInfo.DefaultDeckId))
        {
            ClientGlobalInfo.DefaultDeckId = availableDecks.First().Id;
        }
        SetDeck(availableDecks.Single(x => x.Id == ClientGlobalInfo.DefaultDeckId), ClientGlobalInfo.DefaultDeckId);
        SetDeckList(availableDecks);
    }
    public void ShowMatch()/////待编辑
    {
        ReturnButton.SetActive(false);
        SwitchButton.SetActive(false);
        MatchMessage.text = _translator.GetText("MatchmakingMenu_LookingForOpponent");
        MatchButtonText.text = _translator.GetText("MatchmakingMenu_CancelButton");
        MatchPassword.readOnly = true;
    }
    public void ShowStopMatch()/////待编辑
    {
        ReturnButton.SetActive(true);
        SwitchButton.SetActive(true);
        MatchMessage.text = _translator.GetText("MatchmakingMenu_DeckReady");
        MatchButtonText.text = _translator.GetText("MatchmakingMenu_PlayButton");
        MatchPassword.readOnly = false;
    }

    public async void MatchButtonClick()/////点击匹配按钮的话
    {
        int usingBlacklist = RecordStatus.isOn ? 1 : 0;

        try
        {
            //如果正在进行匹配
            if (IsDoingMatch)
            {
                //停止匹配
                await _client.StopMatch();
                return;
            }
            var selectedDeck = _client.User.Decks.Single(x => x.Id == CurrentDeckId);
            if (!DeckRuleEngine.CanPlayerSelectRuleDeck(_client.FeatureManifest, selectedDeck))
            {
                await _UIService.YNMessageBox("PopupWindow_IncompleteDeckTitle", "PopupWindow_IncompleteDeckDesc", "PopupWindow_OkButton", isOnlyYes: true);
                return;
            }
            var customPassword = IsRankMatch ? "" : (MatchPassword.text ?? "").Trim();
            // Typing a password is an explicit custom entry point. It deliberately
            // bypasses public-queue rule fingerprint pairing, but the local deck
            // must still be legal under its own rules.
            if (!string.IsNullOrWhiteSpace(customPassword))
            {
                var hasRules = HasRuleCards(selectedDeck);
                var valid = IsFeatureDeckComplete(selectedDeck, true) ||
                            (!hasRules && (selectedDeck.IsBasicDeck() || selectedDeck.IsSpecialDeck()));
                if (!valid)
                {
                    await _UIService.YNMessageBox("PopupWindow_IncompleteDeckTitle", "PopupWindow_IncompleteDeckDesc", "PopupWindow_OkButton", isOnlyYes: true);
                    return;
                }
                var password = !hasRules && selectedDeck.IsSpecialDeck() ? "special" + customPassword : customPassword;
                _ = _client.NewMatchOfPassword(CurrentDeckId, password, usingBlacklist);
            }
            else if (_serverModeMenu != null && _serverModeMenu.HasModes)
            {
                var selected = _serverModeMenu.SelectedMode;
                var setBlacklist = selected != null && selected.IsRanked ? 0 : usingBlacklist;
                if (selected == null || !await _client.MatchMode(CurrentDeckId, selected.Id, setBlacklist))
                {
                    await _UIService.YNMessageBox(
                        ResolveLocalized(new LocalizedText { ZhCn = "无法开始对战", En = "Unable to start" }),
                        ResolveLocalized(new LocalizedText { ZhCn = "当前卡组不符合所选模式，或模式暂时不可用。", En = "The selected deck is not valid for this mode, or the mode is temporarily unavailable." }),
                        isOnlyYes: true);
                    return;
                }
            }
            //兼容旧服务器/旧密码入口
            else if (!HasRuleCards(selectedDeck) && selectedDeck.IsBasicDeck())
            {
                var password = IsRankMatch ? "rank" : (MatchPassword.text).Replace("special", "");
                var setBlacklist = IsRankMatch ? 0 : usingBlacklist;
                _ = _client.NewMatchOfPassword(CurrentDeckId, password, setBlacklist);
            }
            else if (HasRuleCards(selectedDeck) || !selectedDeck.IsSpecialDeck())
            {
                await _UIService.YNMessageBox("PopupWindow_IncompleteDeckTitle", "PopupWindow_IncompleteDeckDesc", "PopupWindow_OkButton", isOnlyYes: true);
                return;
            }
            //否则以乱斗卡组匹配(目前不关注匹配结果)
            else
                _ = _client.NewMatchOfPassword(CurrentDeckId, "special" + MatchPassword.text, usingBlacklist);



            // else if (!await _client.Match(CurrentDeckId))
            // {
            //     //如果发生错误的话,匹配失败
            //     Debug.Log("发送未知错误,匹配失败");
            // }

            //将状态和显示都变成正在匹配
            IsDoingMatch = true;
            _client.ClientState = ClientState.Match;
            ShowMatch();

            //等待匹配的结果,如果是true代表成功匹配
            if (await _client.MatchResult())
            {
                if (!this || !Application.isPlaying)
                    return;

                //进入了游戏
                Debug.Log("成功匹配,进入游戏");
                ClientGlobalInfo.IsToMatch = false;
                ClientGlobalInfo.IsPreviousRankMatch = IsRankMatch;
#if UNITY_STANDALONE_WIN
                ClientGlobalInfo.OpenWindow("UnityWndClass", "MyGwent");
#endif
                SceneManager.LoadScene("GamePlay");
                _client.ClientState = ClientState.Play;
                return;
            }
            else
            {
                //否则代表取消了匹配
                Debug.Log("成功停止匹配");
                IsDoingMatch = false;
                _client.ClientState = ClientState.Standby;
                ShowStopMatch();
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            IsDoingMatch = false;
            _client.ClientState = ClientState.Standby;
            ShowStopMatch();
            await _UIService.YNMessageBox(
                ResolveLocalized(new LocalizedText { ZhCn = "匹配请求失败", En = "Match request failed" }),
                ResolveLocalized(new LocalizedText { ZhCn = "连接仍在时可以直接重试；若服务器已断开，客户端会自动返回登录界面。", En = "You can retry while connected. If the server disconnected, the client will return to login automatically." }),
                isOnlyYes: true);
        }
    }
    public void SwitchDeckOpen()
    {
        ReturnButton.SetActive(false);
        SwitchButton.SetActive(false);
        MatchButton.SetActive(false);
        CloseButton.SetActive(true);
        DeckNameBackground.gameObject.SetActive(false);
        DecksScrollbar.GetComponent<Scrollbar>().value = 1;
        DeckSwitch.GetComponent<Animator>().Play("SwitchDeckOpen");
    }
    public void MatchReset()
    {
        ReturnButton.SetActive(true);
        SwitchButton.SetActive(true);
        MatchButton.SetActive(true);
        CloseButton.SetActive(false);
        CardsScrollbar.GetComponent<Scrollbar>().value = 1;
        DeckNameBackground.gameObject.SetActive(true);
    }
    public void SwitchDeckClose()
    {
        MatchReset();
        DeckSwitch.GetComponent<Animator>().Play("SwitchDeckClose");
    }


    void Start()
    {
        _serverModeMenu = ServerModeMenu.Attach(this);
        RecordStatus.onValueChanged.AddListener(x =>
       {
           PlayerPrefs.SetInt("RecordBlacklist", x ? 1 : 0);
       });
        ResetMatch();
        IsDoingMatch = false;
        _client.ClientState = ClientState.Standby;
        RecordStatus.isOn = PlayerPrefs.GetInt("RecordBlacklist", 0) != 0;
        BlacklistMessage.text = _translator.GetText("MatchmakingMenu_BlacklistCheckbox");
    }

    public void OnServerModeSelected(GameModeDefinition mode)
    {
        if (mode == null) return;
        IsRankMatch = mode.IsRanked;
        MainMenu_MatchTitle.text = ResolveLocalized(mode.Name);
        BlacklistObject.SetActive(mode.MatchKind == "pvp" && !mode.IsRanked);
        MatchPasswordObject.SetActive(mode.MatchKind == "pvp" && !mode.IsRanked);
        var deck = _client.User?.Decks?.FirstOrDefault(x => x.Id == CurrentDeckId);
        if (deck != null) SetDeck(deck, CurrentDeckId);
    }
    public string ResolveLocalized(LocalizedText text)
    {
        var language = _translator.TextLocalization.ChosenLanguage?.Filename ?? "cn";
        return text?.Resolve(string.Equals(language, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "zh-CN") ?? "";
    }
    public void SetDeckList(IList<DeckModel> decks)
    {
        decks = (decks ?? new List<DeckModel>())
            .Where(x => DeckRuleEngine.CanPlayerSelectRuleDeck(_client.FeatureManifest, x))
            .ToList();
        var count = DecksContext.childCount;
        // Debug.Log($"数量为:{count}");
        for (var i = count - 1; i >= 0; i--)
        {
            // Debug.Log($"消除{i}");
            Destroy(DecksContext.GetChild(i).gameObject);
        }
        // Debug.Log($"完成消除,脱离");
        DecksContext.DetachChildren();
        var hasAnyRuleDeck = decks.Any(HasRuleCards);
        if (!hasAnyRuleDeck) _deckRuleFilter = DeckRuleFilter.All;
        if (hasAnyRuleDeck)
            DeckRuleFilterBar.Create(
                DecksContext,
                _deckRuleFilter,
                DeckName != null ? DeckName.font : null,
                string.Equals(_translator.TextLocalization.ChosenLanguage?.Filename, "en", StringComparison.OrdinalIgnoreCase),
                value =>
                {
                    if (_deckRuleFilter == value) return;
                    _deckRuleFilter = value;
                    SetDeckList(_client.User.Decks);
                });
        var visibleDecks = decks.Where(x =>
            _deckRuleFilter == DeckRuleFilter.All ||
            (_deckRuleFilter == DeckRuleFilter.Rules && HasRuleCards(x)) ||
            (_deckRuleFilter == DeckRuleFilter.Standard && !HasRuleCards(x))).ToList();
        Debug.Log(visibleDecks.Select(x => x.Name).Join(","));
        visibleDecks.ForAll(x =>
        {
            var deck = Instantiate(DeckPrefabs[GetFactionIndex(GwentMap.CardMap[x.Leader].Faction)]);
            deck.transform.SetParent(DecksContext, false);
            string leaderartid = GwentMap.CardMap[x.Leader].CardArtsId;
            var legacyValid = !HasRuleCards(x) && (x.IsBasicDeck() || x.IsSpecialDeck());
            var showInfo = deck.GetComponent<DeckShowInfo>();
            showInfo.SetDeckInfo(x.Name, IsFeatureDeckComplete(x) || legacyValid);
            showInfo.SetRuleInfo(x, _client.FeatureManifest, _translator);
            deck.GetComponent<DeckEditorMiniatures>().SetMiniatureArt(leaderartid);
            deck.GetComponent<SwitchMatchDeck>().SetDeckId(x.Id);
        });
        // Debug.Log("生成完毕,设置高");
        var height = visibleDecks.Count * 83 + (hasAnyRuleDeck ? 81 : 35);
        DecksContext.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(370, height > 800 ? height : 800);
        // Debug.Log("顺利完成");
    }

    public void SetMatchArtCard(CardStatus card, bool isOver = true)
    {
        ShowArtCard.CurrentCore = card;
        ShowArtCard.gameObject.SetActive(isOver);
    }

    public void SetDeck(DeckModel deck, string id)
    {
        Debug.Log($"设置");
        CurrentDeckId = id;
        var count = CardsContext.childCount;
        for (var i = count - 1; i >= 0; i--)
        {
            Destroy(CardsContext.GetChild(i).gameObject);
        }
        CardsContext.DetachChildren();
        //////////////////////////////////////////////////
        DeckName.text = deck.Name;
        DeckNameBackground.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(25 * DeckName.text.Length + 150, 71);
        DeckName.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(25 * DeckName.text.Length, 40);
        DeckName.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25 * DeckName.text.Length / 2 - 50, 0);
        DeckIcon.overrideSprite = FactionIcon[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
        //DeckIcon.sprite = Resources.Load<Sprite>("Sprites/Control/coin_northern");
        //////////////////////////////////////////////////
        var leader = Instantiate(LaderPrefab);
        leader.GetComponent<LeaderShow>().SetLeader(deck.Leader);
        leader.transform.SetParent(CardsContext, false);
        var cards = deck.Deck.Select(x => GwentMap.CardMap[x]).ToList();
        var playableCards = deck.Deck.Where(x => !DeckRuleEngine.IsRuleCard(x)).Select(x => GwentMap.CardMap[x]).ToList();
        cards.OrderByDescending(x => x.Group).ThenByDescending(x => x.Strength).GroupBy(x => x.Name).ForAll(x =>
            {
                var card = Instantiate(CardPrefab);
                card.GetComponent<ListCardShowInfo>().SetCardInfo(x.First().CardId, x.Count());
                card.transform.SetParent(CardsContext, false);
            });
        CopperCount.text = playableCards.Count(x => x.Group == Group.Copper).ToString();
        SilverCount.text = playableCards.Count(x => x.Group == Group.Silver).ToString();
        GoldCount.text = playableCards.Count(x => x.Group == Group.Gold).ToString();
        AllCount.text = playableCards.Count.ToString();
        var legacyValid = !HasRuleCards(deck) && (deck.IsBasicDeck() || deck.IsSpecialDeck());
        var valid = IsFeatureDeckComplete(deck) || legacyValid || (deck.IsBlacklist() && deck.Id == "blacklist");
        AllCount.color = valid ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        AllCountText.color = valid ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        HeadT.sprite = HeadTSprite[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
        HeadB.sprite = HeadBSprite[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
        //////////////////////////////////////////////////
        var height = ((41.5f + 3f) * CardsContext.childCount) + 8f + 38f;
        CardsContext.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, height);
    }
    private bool IsFeatureDeckComplete(DeckModel deck, bool ignoreSelectedMode = false)
    {
        var manifest = _client.FeatureManifest ?? new GameFeatureManifest { RulesetVersion = "offline-standard" };
        var mode = ignoreSelectedMode ? null : _serverModeMenu?.SelectedMode;
        if (mode != null)
            return DeckRuleEngine.ValidateMode(deck, manifest, mode, true).IsComplete;
        var selected = deck.Deck.Where(DeckRuleEngine.IsRuleCard).Distinct(StringComparer.Ordinal);
        var rules = DeckRuleEngine.Resolve(manifest.RuleCards, selected, manifest.RulesetVersion, manifest.CardPools);
        return DeckRuleEngine.Validate(deck, rules, true).IsComplete;
    }
    private IEnumerable<DeckModel> GetPlayerVisibleDecks()
        => (_client.User?.Decks ?? new List<DeckModel>())
            .Where(x => DeckRuleEngine.CanPlayerSelectRuleDeck(_client.FeatureManifest, x));
    private static bool HasRuleCards(DeckModel deck)
        => (deck?.Deck ?? new List<string>()).Any(DeckRuleEngine.IsRuleCard);
    public int GetFactionIndex(Faction faction)
    {
        return FactionIndex.Indexed().Single(x => x.Value == faction).Key;
    }
    public void ReturnButtonClick()
    {
        ClientGlobalInfo.IsToMatch = false;
    }
}
