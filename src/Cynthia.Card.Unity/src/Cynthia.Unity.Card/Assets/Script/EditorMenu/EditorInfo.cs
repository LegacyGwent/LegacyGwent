using System.Collections;
using System.Collections.Generic;
using Cynthia.Card;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;
using Autofac;
using Cynthia.Card.Client;
using System;
using DG.Tweening;
using System.Threading.Tasks;
using Assets.Script.Localization;
using UnityEngine.Events;
using static UnityEngine.UI.Scrollbar;
using Cynthia.Card.Common.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class EditorInfo : MonoBehaviour
{
    private string LastHoveredCard;
    //static public bool RighClickActive;
    public static string RightClickedCardID;
    //展示卡牌相关
    public ArtCard EditorArtCard;
    public ArtCard ShowArtCard;
    public InputField ShowSearch;
    public RectTransform ShowCardsContent;
    public GameObject UICardPrefab;
    public Scrollbar ShowCardScroll;
    public Scrollbar ShowDeckScroll;
    public Toggle[] ShowButtons;
    public RectTransform ShowDecksContext;
    public GameObject AddDeckButtonPrefab;
    public GameObject MonstersDeckPrefab;
    public GameObject NilfgaardDeckPrefab;
    public GameObject NorthernRealmsDeckPrefab;
    public GameObject ScoiaTaelDeckPrefab;
    public GameObject SkelligeDeckPrefab;
    private Faction _showFaction = Faction.All;
    private IDictionary<Faction, GameObject> _deckPrefabMap;
    private int _nowShow = -1;
    private string _showSearchMessage = "";
    private bool isSpecial = false;
    private bool _showRuleCards = false;
    private Toggle _ruleCardGroupButton;
    private int _loadedEditorCardPages = 1;
    private DeckRuleFilter _deckRuleFilter = DeckRuleFilter.All;
    //------------------------------------------------
    //DOTween动画
    public RectTransform ShowCardsTitle;
    public RectTransform EditorCardsTitle;
    public RectTransform LeftSwitchMenu;
    public RectTransform RightSwitchMenu;
    //------------------------------------------------
    //公用
    private IList<CardStatus> _cards { get => GwentMap.GetCards(isHasAgent: true).ToList(); }//所有的卡牌
    private GwentClientService _clientService;
    private GlobalUIService _globalUIService;
    public EditorStatus EditorStatus { get; private set; } = EditorStatus.Close;
    //
    public GameObject EditorBodyMian;
    public GameObject EditorBodySwitch;
    public GameObject EditorBodyCore;
    public GameCard SwitchReturnButton;
    public GameObject EditorUI;
    public GameObject MainUI;
    //------------------------------------------------
    //选择卡牌相关
    public RectTransform SwitchCardsContext;
    public RectTransform SwitchCardsHeightContext;
    public SwitchUICard SwitchCardPrefab;
    public Scrollbar SwitchCardsScroll;
    private Faction _nowSwitchFaction = Faction.All;
    private string _nowSwitchLeaderId = null;
    //乱斗模式禁止阿瓦拉克，希拉德，蒂博尔，大锤，店店
    private List<string> SpecialBanningList = new List<string>
            { "12032","12011","12012", "32007", "32005", "22009", "12041" };
    //------------------------------------------------
    //卡组编辑相关
    private string _editorSearchMessage = "";
    public RectTransform EditorCardsContext;
    public RectTransform EditorCListContext;
    public InputField EditorSearch;
    public InputField DeckName;
    public Scrollbar EditorCardsScroll;
    public Scrollbar EditorCListScroll;
    public Toggle[] EditorGroupButtons;
    private Group _nowEditorGroup = Group.Leader;
    private DeckModel _nowEditorDeck = null;
    private readonly DeckBuildingProjectionRevisionGate _projectionRevisionGate
        = new DeckBuildingProjectionRevisionGate();
    private bool _projectionHealthy;
    private DeckBuildingProjection _acceptedProjection;
    private readonly Dictionary<string, DeckBuildingCardState> _projectedCardStates
        = new Dictionary<string, DeckBuildingCardState>(StringComparer.Ordinal);
    // Availability badges describe the remaining copies of that specific card.
    // Keep the server/rule-transition snapshot stable while ordinary cards are
    // edited locally; otherwise the global deck-size remainder makes every
    // visible badge tick down whenever any one card is added.
    private readonly Dictionary<string, int> _availableCopiesSnapshot
        = new Dictionary<string, int>(StringComparer.Ordinal);
    //
    public GameObject EditorListCardPrefab;//列表卡牌
    public GameObject EditorMenuCardPrefab;//菜单卡牌
    public GameObject[] EditorLeadersPrefab;//领袖卡牌
    public Image EditorHeadT;
    public Image EditorHeadB;
    public Sprite[] EditorSpriteHeadT;
    public Sprite[] EditorSpriteHeadB;
    public Faction[] EditorFactionIndex;
    public bool[] DeckMode;
    public Text GoldCount;  //金色数量
    public Text SilverCount;//银色数量
    public Text CopperCount;//铜色数量
    public Text AllCount;   //全部数量
    public Text AllCountText;
    private GameObject _minimumDeckBadge;
    public Text SwitchButtonText;
    public Text BlacklistButtonText;
    public Button SwitchDeckButton;
    public GameObject DeckCodeInputBackGround;
    public InputField DeckCodeInputName;
    public InputField DeckCodeInputCode;
    // mobile right click
    private float pressTime = 0;
    private bool IsRightClickMobile = false;
    //------------------------------------------------
    private LocalizationService _translator;

    private void Awake()
    {
        _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
        _globalUIService = DependencyResolver.Container.Resolve<GlobalUIService>();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }

    void Start()
    {
        _deckPrefabMap = new Dictionary<Faction, GameObject>
         {
             {Faction.NorthernRealms,NorthernRealmsDeckPrefab},
             {Faction.ScoiaTael,ScoiaTaelDeckPrefab},
             {Faction.Monsters,MonstersDeckPrefab},
             {Faction.Skellige,SkelligeDeckPrefab},
             {Faction.Nilfgaard,NilfgaardDeckPrefab},
         };
        ShowSearch.onValueChanged.RemoveAllListeners();
        ShowSearch.onValueChanged.AddListener(x => ShowSearchChange(x));
        EditorSearch.onValueChanged.RemoveAllListeners();
        EditorSearch.onValueChanged.AddListener(x => EditorSearchChange(x));
        DeckName.onValueChanged.RemoveAllListeners();
        DeckName.onValueChanged.AddListener(x => DeckNameChanged(x));
        SwitchButtonText.text = _translator.GetText("EditorMenu_SwitchDeckButton");
        HideLegacyBlacklistButton();
        DisableLegacySpecialModeToggle();
        ApplyPlayerRuleCardVisibility();
        //---------------------------------------------------------------------------
    }

    private void DisableLegacySpecialModeToggle()
    {
        var legacyButton = SwitchDeckButton != null
            ? SwitchDeckButton.gameObject
            : SwitchButtonText != null && SwitchButtonText.transform.parent != null
                ? SwitchButtonText.transform.parent.gameObject
                : null;
        if (legacyButton != null) legacyButton.SetActive(false);
    }

    private void HideLegacyBlacklistButton()
    {
        var button = BlacklistButtonText != null && BlacklistButtonText.transform.parent != null
            ? BlacklistButtonText.transform.parent.gameObject
            : null;
        if (button != null) button.SetActive(false);
    }

    private void CreateRuleCardGroupButton()
    {
        if (_ruleCardGroupButton != null || EditorGroupButtons == null || EditorGroupButtons.Length < 4) return;

        var copperButton = EditorGroupButtons[3];
        _ruleCardGroupButton = Instantiate(copperButton, copperButton.transform.parent);
        _ruleCardGroupButton.name = "RuleCardGroupButton";
        var rect = _ruleCardGroupButton.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(copperButton.GetComponent<RectTransform>().anchoredPosition.x + 69f, rect.anchoredPosition.y);

        var background = _ruleCardGroupButton.transform.Find("Bg");
        var oldIcon = background != null ? background.Find("Icon") : null;
        if (oldIcon != null) oldIcon.gameObject.SetActive(false);
        if (background != null) CreateRuleTabletIcon(background);

        EditorGroupButtons = EditorGroupButtons.Concat(new[] { _ruleCardGroupButton }).ToArray();
        _ruleCardGroupButton.isOn = false;
    }

    private bool PlayerRuleCardsEnabled
        => _clientService?.FeatureManifest?.PlayerRuleCardsEnabled == true &&
           (_clientService.FeatureManifest.RuleCards?.Any(x => x.PlayerSelectable) ?? false);

    private void ApplyPlayerRuleCardVisibility()
    {
        if (PlayerRuleCardsEnabled) CreateRuleCardGroupButton();
        if (_ruleCardGroupButton != null) _ruleCardGroupButton.gameObject.SetActive(PlayerRuleCardsEnabled);
        if (PlayerRuleCardsEnabled) return;

        _showRuleCards = false;
        _deckRuleFilter = DeckRuleFilter.All;
        if (_ruleCardGroupButton != null && _ruleCardGroupButton.isOn && EditorGroupButtons.Length > 0)
            EditorGroupButtons[0].isOn = true;
    }

    private static void CreateRuleTabletIcon(Transform parent)
    {
        var tablet = new GameObject("RuleIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        tablet.transform.SetParent(parent, false);
        var tabletRect = tablet.GetComponent<RectTransform>();
        tabletRect.anchorMin = tabletRect.anchorMax = new Vector2(.5f, .5f);
        tabletRect.sizeDelta = new Vector2(22, 28);
        tabletRect.anchoredPosition = Vector2.zero;
        tablet.GetComponent<Image>().color = new Color32(25, 62, 60, 255);

        for (var i = 0; i < 3; i++)
        {
            var line = new GameObject("RuleLine" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            line.transform.SetParent(tablet.transform, false);
            var lineRect = line.GetComponent<RectTransform>();
            lineRect.anchorMin = lineRect.anchorMax = new Vector2(.5f, .5f);
            lineRect.sizeDelta = new Vector2(i == 2 ? 10 : 14, 2);
            lineRect.anchoredPosition = new Vector2(0, 7 - i * 7);
            line.GetComponent<Image>().color = new Color32(226, 178, 82, 255);
        }
    }

    public void SetEditorCardInfo(IList<CardStatus> cards, bool resetScroll = false)
    {   //设置已有卡牌
        var pagenum = 30;
        var previousScroll = resetScroll ? 1f : EditorCardsScroll.value;
        var pagesToRestore = resetScroll ? 1 : Math.Max(1, _loadedEditorCardPages);
        RemoveAllChild(EditorCardsContext);
        var sc = -1;
        void AddCards(int skipCount, int pageCount, IList<CardStatus> showCards)
        {
            if (showCards.Count <= skipCount * pageCount)
            {
                return;
            }
            var newCards = showCards.Skip(skipCount * pageCount).Take(pageCount).ToList();
            newCards.ForAll(x =>
            {
                var card = Instantiate(EditorMenuCardPrefab).GetComponent<EditorUICoreCard>();
                card.cardShowInfo.setCurrentCore(x, true);
                card.Count = GetAvailableCopies(x);
                card.transform.SetParent(EditorCardsContext, false);
                if (DeckRuleEngine.IsRuleCard(x.CardId)) AddRuleCardBadge(card.transform);
            });
        }
        for (var page = 0; page < pagesToRestore && page * pagenum < cards.Count; page++)
        {
            AddCards(page, pagenum, cards);
            sc = page;
        }
        _loadedEditorCardPages = Math.Max(1, sc + 1);
        Canvas.ForceUpdateCanvases();
        EditorCardsScroll.value = previousScroll;
        if (_editorCardScrollEvent != null)
        {
            EditorCardsScroll.onValueChanged.RemoveListener(_editorCardScrollEvent);
        }
        _editorCardScrollEvent = x =>
        {
            if (x >= 0.3)
            {
                return;
            }
            Debug.Log("到达临界点,触发");

            sc++;
            AddCards(sc, pagenum, cards);
            _loadedEditorCardPages = Math.Max(_loadedEditorCardPages, sc + 1);
        };
        EditorCardsScroll.onValueChanged.AddListener(_editorCardScrollEvent);
    }

    private void AddRuleCardBadge(Transform card)
    {
        var badge = new GameObject("RuleCardBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
        badge.layer = card.gameObject.layer;
        badge.transform.SetParent(card, false);
        badge.transform.SetAsLastSibling();
        var badgeRect = badge.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0, 1);
        badgeRect.anchorMax = new Vector2(0, 1);
        badgeRect.pivot = new Vector2(0, 1);
        badgeRect.anchoredPosition = new Vector2(6, -6);
        badgeRect.sizeDelta = new Vector2(64, 26);
        badge.GetComponent<Image>().color = new Color32(15, 53, 55, 245);
        var outline = badge.GetComponent<Outline>();
        outline.effectColor = new Color32(222, 174, 78, 210);
        outline.effectDistance = new Vector2(1, -1);

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        labelObject.layer = badge.layer;
        labelObject.transform.SetParent(badge.transform, false);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        var label = labelObject.GetComponent<Text>();
        label.font = SwitchButtonText != null ? SwitchButtonText.font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 15;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color32(238, 220, 174, 255);
        label.text = Local("规则", "RULE");
    }

    private UnityAction<float> _showCardScrollEvent = null;
    private UnityAction<float> _editorCardScrollEvent = null;

    public void SetShowCardInfo(IList<CardStatus> cards)
    {   //设置已有卡牌
        var pagenum = 30;
        ShowCardScroll.value = 1;
        RemoveAllChild(ShowCardsContent);
        var sc = 0;
        AddCards(sc, pagenum, cards);
        void AddCards(int skipCount, int pageCount, IList<CardStatus> showCards)
        {
            if (showCards.Count <= skipCount * pageCount)
            {
                return;
            }
            var newCards = showCards.Skip(skipCount * pageCount).Take(pageCount).ToList();
            newCards.ForAll(x =>
            {
                var card = Instantiate(UICardPrefab).GetComponent<CardShowInfo>();
                card.setCurrentCore(x, true);
                card.transform.SetParent(ShowCardsContent, false);
            });
        }
        if (_showCardScrollEvent != null)
        {
            ShowCardScroll.onValueChanged.RemoveListener(_showCardScrollEvent);
        }
        _showCardScrollEvent = x =>
        {
            if (x >= 0.3)
            {
                return;
            }
            Debug.Log("到达临界点,触发");

            sc++;
            AddCards(sc, pagenum, cards);
        };
        ShowCardScroll.onValueChanged.AddListener(_showCardScrollEvent);
    }

    public async void OpenEditor(bool IsMoveLeftRight = true)
    {
        await _clientService.RefreshGameFeatureManifest();
        ApplyPlayerRuleCardVisibility();
        ShowCardScroll.value = 1;
        EditorCardsScroll.value = 1;
        EditorStatus = EditorStatus.ShowCards;
        _nowSwitchLeaderId = null;
        _nowEditorDeck = null;
        DeckName.text = _translator.GetText("EditorMenu_DefaultDeckname");
        _nowEditorGroup = Group.Leader;
        EditorBodyCore.SetActive(false);
        EditorBodyMian.SetActive(true);
        if (IsMoveLeftRight)
        {
            ShowCardsTitle.anchoredPosition = new Vector2(0, -63f);
            EditorCardsTitle.anchoredPosition = new Vector2(0, 150f);
            LeftSwitchMenu.anchoredPosition = new Vector2(-1700, 0);
            RightSwitchMenu.anchoredPosition = new Vector2(1700, 0);
        }
        ResetEditor();
    }

    public void AutoSetShowCards()
    {   //按照筛选条件进行筛选
        SetShowCardInfo
        (
            _cards
            .Where(x => ((_showFaction == Faction.All) ? true : (x.Faction == _showFaction)))
            .Where(x => ((_showSearchMessage == "") ? true :
                (_translator.GetCardName(x.CardInfo().CardId).Contains(_showSearchMessage, StringComparison.OrdinalIgnoreCase) ||
                _translator.GetCardInfo(x.CardInfo().CardId).Contains(_showSearchMessage, StringComparison.OrdinalIgnoreCase) ||
                x.CardInfo().Strength.ToString().Contains(_showSearchMessage) ||
                x.Categories.Select(tag => _translator.GetText($"CardTag_{GwentMap.CategorieInfoMap[tag]}")).Any(text => text.Contains(_showSearchMessage, StringComparison.OrdinalIgnoreCase))
                )))
            .ToList()
        );
    }

    public void ResetEditor()
    {
        EditorSearch.text = "";
        ShowSearch.text = "";
        _nowSwitchFaction = Faction.All;
        //
        SetDeckList(_clientService.User.Decks);
        ShowButtons[0].isOn = true;
    }

    public void ShowFactionClick()
    {   //展示卡牌中,切换势力显示的按钮被点击
        ShowCardScroll.value = 1;
        ShowCardScroll.value = 1;
        if (!ShowButtons.Any(x => x.isOn)) return;
        var result = ShowButtons.Select((item, index) => (item, index)).First(x => x.item.isOn).index;
        if (result == _nowShow) return;
        _showFaction = (Faction)result;
        AutoSetShowCards();
        _nowShow = result;
    }

    public void ShowSearchChange(string value)
    {   //展示卡牌中,搜索框改变
        _showSearchMessage = value;
        AutoSetShowCards();
    }

    public void RemoveAllChild(Transform father)
    {   //删除所有子物体
        for (var i = father.childCount - 1; i >= 0; i--)
        {
            Destroy(father.GetChild(i).gameObject);
        }
        father.DetachChildren();
    }

    private void RemoveAllChildExcept(Transform father, Transform preserved)
    {
        for (var i = father.childCount - 1; i >= 0; i--)
        {
            var child = father.GetChild(i);
            if (child == preserved) continue;
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    public void SetDeckList(IList<DeckModel> decks)
    {
        //设置已有卡组
        var availableDecks = (decks ?? new List<DeckModel>())
            .Where(x => DeckRuleEngine.CanPlayerSelectRuleDeck(_clientService.FeatureManifest, x))
            .ToList();
        var hasAnyRuleDeck = availableDecks.Any(HasRuleCards);
        if (!hasAnyRuleDeck) _deckRuleFilter = DeckRuleFilter.All;
        var filterBar = ShowDecksContext.Find("DeckRuleFilterBar");
        RemoveAllChildExcept(ShowDecksContext, hasAnyRuleDeck ? filterBar : null);
        if (!hasAnyRuleDeck && filterBar != null)
        {
            filterBar.gameObject.SetActive(false);
            Destroy(filterBar.gameObject);
            filterBar = null;
        }
        if (hasAnyRuleDeck)
        {
            if (filterBar == null)
                filterBar = DeckRuleFilterBar.Create(
                    ShowDecksContext,
                    _deckRuleFilter,
                    SwitchButtonText != null ? SwitchButtonText.font : null,
                    Local("否", "no") == "no",
                    value =>
                    {
                        if (_deckRuleFilter == value) return;
                        _deckRuleFilter = value;
                        DeckRuleFilterBar.SetCurrent(ShowDecksContext.Find("DeckRuleFilterBar")?.gameObject, value);
                        SetDeckList(_clientService.User.Decks);
                    }).transform;
            filterBar.gameObject.SetActive(true);
            filterBar.SetSiblingIndex(0);
            DeckRuleFilterBar.SetCurrent(filterBar.gameObject, _deckRuleFilter);
        }
        var button = Instantiate(AddDeckButtonPrefab);
        button.transform.SetParent(ShowDecksContext, false);
        //-----
        var visibleDecks = availableDecks.Where(x =>
            x.Id != "blacklist" &&
            (_deckRuleFilter == DeckRuleFilter.All ||
             (_deckRuleFilter == DeckRuleFilter.Rules && HasRuleCards(x)) ||
             (_deckRuleFilter == DeckRuleFilter.Standard && !HasRuleCards(x)))).ToList();
        visibleDecks.ForAll(x =>
        {
            if (_deckPrefabMap == null) Start();
            if (!GwentMap.CardMap.TryGetValue(x.Leader ?? string.Empty, out var leader))
            {
                Debug.LogWarning($"Deck '{x.Name}' ({x.Id}) is hidden because leader '{x.Leader}' is missing from CardMap.");
                return;
            }
            if (!_deckPrefabMap.TryGetValue(leader.Faction, out var deckPrefab) || deckPrefab == null)
            {
                Debug.LogWarning($"Deck '{x.Name}' ({x.Id}) is hidden because leader '{x.Leader}' has unsupported faction '{leader.Faction}'.");
                return;
            }
            var deck = Instantiate(deckPrefab);
            deck.transform.SetParent(ShowDecksContext, false);
            string leaderartid = leader.CardArtsId;
            var featureValid = DeckRuleEngine.Validate(x, ResolveRules(x), true).IsComplete;
            var legacyValid = !HasRuleCards(x) && (x.IsBasicDeck() || x.IsSpecialDeck());
            var showInfo = deck.GetComponent<DeckShowInfo>();
            showInfo.SetDeckInfo(x.Name, featureValid || legacyValid);
            showInfo.SetRuleInfo(x, _clientService.FeatureManifest, _translator);
            deck.GetComponent<DeckEditorMiniatures>().SetMiniatureArt(leaderartid);
            deck.GetComponent<EditorShowDeck>().Id = x.Id;
        });
        //----
        var count = visibleDecks.Count;
        // var height = (15 + 65 + 5 + 85) + (80 + 5) * count;//count <= 16 ? 780f : 
        // ShowDecksContext.sizeDelta = new Vector2(0, height);
        ShowDeckScroll.value = 1;
    }

    public async void ShowDeckRemoveClick(string Id)
    {
        // if (_nowEditorDeck.Id != "blacklist")
        // {
        if (await _globalUIService.YNMessageBox("PopupWindow_DeleteDeckTitle",
            string.Format(_translator.GetText("PopupWindow_DeleteDeckDesc"), _clientService.User.Decks.Single(x => x.Id == Id).Name)))
        {
            if (!(await _clientService.RemoveDeck(Id)))
            {
                await _globalUIService.YNMessageBox("PopupWindow_DeleteDeckErrorTitle",
                    string.Format(_translator.GetText("PopupWindow_DeleteDeckErrorDesc"), _clientService.User.Decks.Single(x => x.Id == Id).Name));
            }
            else
            {
                var i = _clientService.User.Decks.Select((item, index) => (item, index)).Single(x => x.item.Id == Id).index;
                _clientService.User.Decks.RemoveAt(i);
                SetDeckList(_clientService.User.Decks);
            }
        }
        //  }
    }

    public void ShowDeckEditorClick(string Id)
    {
        Debug.Log("点击了【" + _clientService.User.Decks.Single(x => x.Id == Id).Name + "】卡组的编辑按钮");
        var deck = _clientService.User.Decks.Single(x => x.Id == Id);
        if (!DeckRuleEngine.CanPlayerSelectRuleDeck(_clientService.FeatureManifest, deck)) return;
        _nowEditorDeck = deck;
        isSpecial = !HasRuleCards(deck) && (!deck.IsHalfBasicDeck()) && deck.IsHalfSpecialDeck();
        _nowSwitchLeaderId = deck.Leader;
        _nowSwitchFaction = GwentMap.CardMap[deck.Leader].Faction;
        //
        EditorStatus = EditorStatus.EditorDeck;
        ResetEditorCore();
        ShowCardsTitle.anchoredPosition = new Vector2(0, 150f);
        EditorCardsTitle.anchoredPosition = new Vector2(0, -63f);
        EditorBodyCore.SetActive(true);
        EditorBodyMian.SetActive(false);
    }
    //=============================================================================================================================
    //以上为展示卡牌相关的内容,以下为展示框选择相关内容
    public void SwitchDeckClick()
    {
        // Kept only for compatibility with the serialized scene event. New decks
        // enter custom rules solely by adding an explicit rule card.
        return;
    }

    public void AddDeckClick()
    {   //点击新建按钮后
        if (_clientService.User.Decks.Count >= 1000)
        {
            _globalUIService.YNMessageBox(_translator.GetText("PopupWindow_DeckLimitTitle"), _translator.GetText("PopupWindow_DeckLimitDesc"));
        }
        else
        {
            DOTween.To(() => ShowCardsTitle.anchoredPosition, x => ShowCardsTitle.anchoredPosition = x,
                        new Vector2(0, 150), 0.5f);//收回Title
            DOTween.To(() => LeftSwitchMenu.anchoredPosition, x => LeftSwitchMenu.anchoredPosition = x,
                new Vector2(-470, 0), 0.5f);//展开Left
            DOTween.To(() => RightSwitchMenu.anchoredPosition, x => RightSwitchMenu.anchoredPosition = x,
                new Vector2(468, 0), 0.5f);//展开Right
            /*
            Titile x:0 | Y:478.5 true    Y: 605 false
            Left y:0 | X:-470 true     X: -1700 false
            Right y:0 | X: 468 true     X: 1700 false*/
            EditorStatus = EditorStatus.SwitchFaction;
            SetSwitchList(((Faction[])Enum.GetValues(typeof(Faction)))
                .Where(x => x != Faction.All && x != Faction.Neutral)
                .Select(x => new CardStatus() { DeckFaction = x })
                .ToList());
            _nowSwitchLeaderId = null;
            _nowEditorDeck = null;
            DeckName.text = _translator.GetText("EditorMenu_DefaultDeckname");
        }

    }
    public void SetBlacklistClick()
    {
        // Kept as an empty UnityEvent compatibility target for older scenes.
        // DIY-AI no longer exposes or applies the blacklist feature.
    }
    public void SelectSwitchUICard(CardStatus card, bool isOver = true)
    {
        //悬停在卡牌上,显示卡牌信息...但是目前没有做
        // Debug.Log($"选中卡牌发生变化:  是否选中?:{isOver},卡牌名称:{card.Name},当前页面:{this.EditorStatus}");
        if (EditorStatus == EditorStatus.EditorDeck)
        {
            EditorArtCard.CurrentCore = card;
            EditorArtCard.gameObject.SetActive(isOver);
            LastHoveredCard=card.CardId;
        }
        else if (EditorStatus == EditorStatus.ShowCards)
        {
            ShowArtCard.CurrentCore = card;
            ShowArtCard.gameObject.SetActive(isOver);
            LastHoveredCard=card.CardId;
        }
        //Debug.Log("LAST HOVERED: "+LastHoveredCard);
    }
    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);
        switch(touch.phase)
        {
            case TouchPhase.Began:
                pressTime = 0;
                IsRightClickMobile = false;
                break;
            case TouchPhase.Stationary:
                pressTime += Time.deltaTime;
                if (pressTime > 1f)
                {
                    IsRightClickMobile = true;
                    pressTime = 0;
                }
                break;
            case TouchPhase.Moved:
                pressTime = 0;
                IsRightClickMobile = false;
                break;
            case TouchPhase.Ended:
                IsRightClickMobile = false;
                pressTime = 0;
                break;
            case TouchPhase.Canceled:
                IsRightClickMobile = false;
                pressTime = 0;
                break;
        }
#endif
        if (Input.GetMouseButtonDown(1) || IsRightClickMobile)
        {
            IsRightClickMobile = false;
            pressTime = 0;              

            if (EditorArtCard.gameObject.activeSelf || ShowArtCard.gameObject.activeSelf)
            {
                RightClickedCardID = LastHoveredCard;
                Debug.Log("Right Clicked ID: " + RightClickedCardID);
                SceneManager.LoadScene("RightClick", LoadSceneMode.Additive);
            }
        }
    }
            
    public void ClickSwitchUICard(CardStatus card)
    {
        if (EditorStatus == EditorStatus.SwitchFaction)
        {   //如果目前正在选择势力
            _nowSwitchFaction = card.DeckFaction;
            SetSwitchList(_cards.Where(x => x.Group == Group.Leader && x.Faction == _nowSwitchFaction).ToList());

            EditorStatus = EditorStatus.SwitchLeader;
        }
        else if (EditorStatus == EditorStatus.SwitchLeader)
        {   //选择了领袖
            _nowSwitchLeaderId = card.CardId;
            if (_nowEditorDeck != null) _nowEditorDeck.Leader = _nowSwitchLeaderId;
            else _nowEditorDeck = new DeckModel() { Leader = _nowSwitchLeaderId, Deck = new List<string>() };
            //收回...不过降下编辑的
            EditorBodyCore.SetActive(true);
            EditorBodyMian.SetActive(false);
            EditorStatus = EditorStatus.EditorDeck;
            ResetEditorCore();
            DOTween.To(() => EditorCardsTitle.anchoredPosition, x => EditorCardsTitle.anchoredPosition = x,
                    new Vector2(0, -63f), 0.5f);//降下Title,设定标题 ********
            DOTween.To(() => LeftSwitchMenu.anchoredPosition, x => LeftSwitchMenu.anchoredPosition = x,
                new Vector2(-1700, 0), 0.5f);//收回Left
            DOTween.To(() => RightSwitchMenu.anchoredPosition, x => RightSwitchMenu.anchoredPosition = x,
                new Vector2(1700, 0), 0.5f);//收回Right
        }
    }

    public async void SwitchReturn()
    {

        switch (EditorStatus)
        {
            case EditorStatus.SwitchLeader://选择领袖阶段,变回选择势力
                SetSwitchList(((Faction[])Enum.GetValues(typeof(Faction)))
                    .Where(x => x != Faction.All && x != Faction.Neutral)
                    .Select(x => new CardStatus() { DeckFaction = x })
                    .ToList());
                EditorStatus = EditorStatus.SwitchFaction;
                _nowSwitchLeaderId = null;
                _nowEditorDeck = null;
                DeckName.text = _translator.GetText("EditorMenu_DefaultDeckname");
                break;
            case EditorStatus.SwitchFaction://选择势力阶段,变为展示卡牌阶段
                OpenEditor(false);
                DOTween.To(() => ShowCardsTitle.anchoredPosition, x => ShowCardsTitle.anchoredPosition = x,
                    new Vector2(0, 478.5f), 0.5f);//降下Title,设定标题 ********
                DOTween.To(() => LeftSwitchMenu.anchoredPosition, x => LeftSwitchMenu.anchoredPosition = x,
                    new Vector2(-1700, 0), 0.5f);//收回Left
                DOTween.To(() => RightSwitchMenu.anchoredPosition, x => RightSwitchMenu.anchoredPosition = x,
                    new Vector2(1700, 0), 0.5f);//收回Right
                /*
                Titile x:0 | Y:478.5 true    Y: 605 false
                Left y:0 | X:-470 true     X: -1700 false
                Right y:0 | X: 468 true     X: 1700 false*/
                break;
            case EditorStatus.ShowCards://展示卡牌阶段,关闭编辑器
                MainUI.SetActive(true);
                EditorUI.SetActive(false);
                EditorStatus = EditorStatus.Close;
                break;
            case EditorStatus.EditorDeck:
                //###################################
                //需要补充,保存并且提交
                //###################################
                //后续处理
                // if (!_nowEditorDeck.IsBasicDeck())
                // {
                //     _ = _globalUIService.YNMessageBox("卡组不符合标准", "目前不支持提交不合标准的卡组,请调整后提交");
                //     break;
                // }
                // else
                // {
                // Saving a draft and leaving the editor never requires a complete
                // match-ready deck. Strict validation remains at matchmaking.
                _nowEditorDeck.Name = (DeckName.text == "" ? _translator.GetText("EditorMenu_DefaultDeckname") : DeckName.text);
                if (_nowEditorDeck.Id == "blacklist")
                {

                    _clientService.User.Blacklist = new BlacklistModel()
                    {
                        Blacklist = _nowEditorDeck.Deck.ToList(),
                    };
                    await _clientService.ModifyBlacklist(_clientService.User.Blacklist);

                }
                else if (_clientService.User.Decks.Any(x => x.Id == (_nowEditorDeck.Id == null ? "" : _nowEditorDeck.Id)))
                {
                    // if (await _globalUIService.YNMessageBox("是否修改卡组?", $"是否修改卡组 {DeckName.text}"))
                    // {
                    if (await _clientService.ModifyDeck(_nowEditorDeck.Id, _nowEditorDeck))
                    {
                        var i = _clientService.User.Decks.Select((item, index) => (item, index)).Single(x => x.item.Id == _nowEditorDeck.Id).index;
                        _clientService.User.Decks[i] = _nowEditorDeck;
                        ClientGlobalInfo.DefaultDeckId = _nowEditorDeck.Id;
                    }
                    else
                    {
                        if (!(await _globalUIService.YNMessageBox(_translator.GetText("PopupWindow_AddDeckErrorTitle"),
                            _translator.GetText("PopupWindow_AddDeckErrorDesc"))))
                        {
                            break;
                        }
                    }


                    // }
                }
                else
                {
                    // if (await _globalUIService.YNMessageBox("是否新建卡组?", $"是否新建卡组 {DeckName.text}"))
                    // {
                    _nowEditorDeck.Id = Guid.NewGuid().ToString();
                    if ((await _clientService.AddDeck(_nowEditorDeck)))
                    {   //如果添加卡组通过验证
                        //也在本地添加卡组
                        _clientService.User.Decks.Add(_nowEditorDeck);
                    }
                    else
                    {
                        if (!(await _globalUIService.YNMessageBox(_translator.GetText("PopupWindow_AddDeckErrorTitle"),
                            _translator.GetText("PopupWindow_AddDeckErrorDesc"))))
                        {
                            break;
                        }
                    }
                    // }
                }
                // }
                OpenEditor();
                break;
        }
    }

    public void SetSwitchList(IList<CardStatus> cards)
    {//选择列表
        RemoveAllChild(SwitchCardsContext);
        cards.ForAll(x =>
        {
            var card = Instantiate(SwitchCardPrefab).GetComponent<SwitchUICard>();
            card.CardShowInfo.setCurrentCore(x, true);
            card.transform.SetParent(SwitchCardsContext, false);
        });
        //------------------------------------------------------------------------//276
        var count = cards.Count;
        var height = (130 + (251 + 46.5f) * ((int)(count % 3 > 0 ? count / 3 + 1 : count / 3)) + 50f);//count <= 16 ? 780f : 
        SwitchCardsContext.sizeDelta = new Vector2(0, height > 1000 ? height : 1000);
        SwitchCardsHeightContext.sizeDelta = new Vector2(0, height);
        //ShowCardsContent.GetComponent<GridLayoutGroup>().padding.top = 104;
        SwitchCardsScroll.value = 1;
    }
    //=============================================================================================================================
    //以上为展示框选择,以下为编辑菜单相关
    public void DeckNameChanged(string name)
    {
        if (_nowEditorDeck != null)
            _nowEditorDeck.Name = name;
    }

    public async void ResetEditorCore()
    {//初始化
        _projectionHealthy = false;
        _acceptedProjection = null;
        _projectedCardStates.Clear();
        _availableCopiesSnapshot.Clear();
        EditorSearch.text = "";
        DeckName.text = (_nowEditorDeck.Name == null || _nowEditorDeck.Name == "") ? _translator.GetText("EditorMenu_DefaultDeckname") : _nowEditorDeck.Name;
        if (_nowEditorDeck.Id != "blacklist")
        {
            SwitchButtonText.text = _translator.GetText("EditorMenu_SwitchDeckButton");
            EditorHeadT.sprite = EditorSpriteHeadT[GetFactionIndex(_nowSwitchFaction)];
            EditorHeadB.sprite = EditorSpriteHeadB[GetFactionIndex(_nowSwitchFaction)];
        }
        else
        {
            SwitchButtonText.text = "";

        }
        //
        SetEditorDeck(_nowEditorDeck);
        EditorGroupButtons[0].isOn = true;
        AutoSetEditorCards();
        await RefreshProjectionForCurrentDeck();
    }

    public void ClickEditorListLeader(string id)
    {//点击了领袖   应该返回选择领袖,保留目前的卡组名称与卡牌
        EditorStatus = EditorStatus.SwitchLeader;
        SetSwitchList(_cards.Where(x => x.Group == Group.Leader && x.Faction == _nowSwitchFaction).ToList());
        DOTween.To(() => EditorCardsTitle.anchoredPosition, x => EditorCardsTitle.anchoredPosition = x,
                    new Vector2(0, 150), 0.5f);//收回Title
        DOTween.To(() => LeftSwitchMenu.anchoredPosition, x => LeftSwitchMenu.anchoredPosition = x,
            new Vector2(-470, 0), 0.5f);//展开Left
        DOTween.To(() => RightSwitchMenu.anchoredPosition, x => RightSwitchMenu.anchoredPosition = x,
            new Vector2(468, 0), 0.5f);//展开Right
                                       //Debug.Log("点击了领袖");
    }

    public async void ClickEditorListCard(string id)
    {//点击了卡牌   应该从卡组去除对应卡牌,并且更新显示
        var subIndex = _nowEditorDeck.Deck.Select((item, index) => (item, index)).First(x => x.item == id).index;
        var candidate = CloneDeck(_nowEditorDeck);
        candidate.Deck.RemoveAt(subIndex);
        if (DeckRuleEngine.IsRuleCard(id) &&
            _nowEditorDeck.Id != "blacklist" &&
            !await ProjectCandidate(candidate, "remove", id)) return;
        _nowEditorDeck.Deck = candidate.Deck;
        if (DeckRuleEngine.IsRuleCard(id))
        {
            SetEditorDeck(_nowEditorDeck);
            AutoSetEditorCards();
        }
        else
        {
            RefreshOrdinaryAvailabilityAfterEdit(id, 1);
            UpdateOrdinaryDeckRow(id);
            RefreshDeckCountersAndHeight(_nowEditorDeck);
        }
    }

    public async void ClickEditorUICoreCard(CardStatus card)
    {//点击了显示卡牌  应该判断是否应该添加卡牌,如果可以,添加并且更新显示,否则跳出消息提醒
        if (card == null || GetAvailableCopies(card) <= 0) return;
        var count = _nowEditorDeck.Deck.Where(x => x == card.CardId).Count();
        if (_nowEditorDeck.Id == "blacklist")
        {
            if (!(count >= 1 || (_nowEditorDeck.Deck.Count >= 2)))
            {   //如果超过上限,禁止加入卡牌
                _nowEditorDeck.Deck.Add(card.CardId);
                UpdateOrdinaryDeckRow(card.CardId);
                RefreshDeckCountersAndHeight(_nowEditorDeck);
                //**********************************************
                var c = GetAllChilds<EditorUICoreCard>(EditorCardsContext).Where(x => x.cardShowInfo.CurrentCore.CardId == card.CardId);
                c.ForAll(x => { x.Count--; });
            }

        }
        else if (isSpecial && !HasRuleCards(_nowEditorDeck))
        {
            if (!((card.Group == Group.Silver && count >= 1) ||
               ((card.Group == Group.Gold || card.Group == Group.Copper) && count >= 3) ||
               (_nowEditorDeck.Deck.Count >= 40) ||
               (card.Group == Group.Silver && _nowEditorDeck.Deck.Where(x => x.CardInfo().Group == Group.Silver).Count() >= 6) ||
               (card.Group == Group.Gold && _nowEditorDeck.Deck.Where(x => x.CardInfo().Group == Group.Gold).Count() >= 12)))
            {
                _nowEditorDeck.Deck.Add(card.CardId);
                AdjustAvailableCopiesSnapshot(card.CardId, -1);
                UpdateOrdinaryDeckRow(card.CardId);
                RefreshDeckCountersAndHeight(_nowEditorDeck);
                RefreshVisibleCardAvailability(card.CardId);
            }
        }
        else
        {
            var candidate = CloneDeck(_nowEditorDeck);
            candidate.Deck.Add(card.CardId);
            // Ordinary edits use the last server-authoritative rule snapshot
            // entirely locally.  Previously only rule-card additions projected
            // their candidate, so an event-only rule (for example Feast Echo)
            // accidentally bypassed the unchanged 4-gold / 6-silver limits.
            if (!DeckRuleEngine.IsRuleCard(card.CardId) &&
                !DeckRuleEngine.CanAddCard(_nowEditorDeck, card.CardId, CurrentRuleSnapshot()))
                return;
            if (DeckRuleEngine.IsRuleCard(card.CardId))
            {
                if (!PlayerRuleCardsEnabled) return;
                if (!HasRuleCards(_nowEditorDeck))
                {
                    var accepted = await _globalUIService.YNMessageBox(
                        Local("加入规则卡", "Add rule card"),
                        Local(
                            "加入规则卡可能改变组卡与匹配条件，实际匹配方式由服务器当前模式决定。是否继续？",
                            "Adding a rule card may change deck-building and matchmaking conditions. The active server mode decides actual compatibility. Continue?"));
                    if (!accepted) return;
                }
            }
            if (DeckRuleEngine.IsRuleCard(card.CardId) &&
                !await ProjectCandidate(candidate, "add", card.CardId)) return;
            _nowEditorDeck.Deck = candidate.Deck;
            if (DeckRuleEngine.IsRuleCard(card.CardId))
            {
                SetEditorDeck(_nowEditorDeck);
                AutoSetEditorCards();
            }
            else
            {
                RefreshOrdinaryAvailabilityAfterEdit(card.CardId, -1);
                UpdateOrdinaryDeckRow(card.CardId);
                RefreshDeckCountersAndHeight(_nowEditorDeck);
            }
        }
        //Debug.Log("点击了菜单卡");
    }

    public void EditorGroupClick()
    {//点击了品质筛选
        if (!EditorGroupButtons.Any(x => x.isOn)) return;
        var result = EditorGroupButtons.Select((item, index) => (item, index)).First(x => x.item.isOn).index;
        if (result == 4)
        {
            if (!PlayerRuleCardsEnabled) return;
            if (!_showRuleCards)
            {
                _showRuleCards = true;
                AutoSetEditorCards(resetScroll: true);
            }
            return;
        }
        _showRuleCards = false;
        var group = result == 0 ? Group.Leader : (result == 1 ? Group.Gold : (result == 2 ? Group.Silver : Group.Copper));
        if (_nowEditorGroup != group)
        {
            _nowEditorGroup = group;
            AutoSetEditorCards(resetScroll: true);
        }
        else AutoSetEditorCards(resetScroll: true);
    }

    public void EditorSearchChange(string value)
    {   //编辑卡牌中,搜索框改变
        _editorSearchMessage = value;
        AutoSetEditorCards(resetScroll: true);
    }

    public void AutoSetEditorCards(bool resetScroll = false)
    {   //按照筛选条件进行筛选
        // Returning from the deck editor clears the active deck before Unity has
        // finished dispatching InputField/Toggle callbacks from the old view.
        // Those late callbacks do not have anything to filter and must not inspect
        // the now-cleared deck.
        if (_nowEditorDeck == null || EditorStatus != EditorStatus.EditorDeck)
            return;

        var rules = CurrentRuleSnapshot();
        var leader = GwentMap.CardMap[_nowEditorDeck.Leader];
        var manifestRuleIds = new HashSet<string>(
            (_clientService.FeatureManifest?.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x.PlayerSelectable)
                .Select(x => x.Id),
            StringComparer.Ordinal);
        var visibleRuleIds = new HashSet<string>(
            (_clientService.FeatureManifest?.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x.PlayerSelectable &&
                    ((x.AllowedLeaderFactions?.Count ?? 0) == 0 ||
                     x.AllowedLeaderFactions.Contains(leader.Faction)))
                .Select(x => x.Id),
            StringComparer.Ordinal);
        var hasProjection = RuleSnapshotMatchesCurrentDeck();
        SetEditorCardInfo
        (
            //
            _cards
            .Where(x => _showRuleCards
                ? PlayerRuleCardsEnabled && DeckRuleEngine.IsRuleCard(x.CardId) && manifestRuleIds.Contains(x.CardId) &&
                  visibleRuleIds.Contains(x.CardId) &&
                  (!hasProjection || _projectedCardStates.ContainsKey(x.CardId))
                : !DeckRuleEngine.IsRuleCard(x.CardId))
            .Where(x =>
                (_nowEditorDeck.Id == "blacklist" && x.Group == Group.Gold) ||
                (_nowEditorDeck.Id != "blacklist" &&
                 !(isSpecial && SpecialBanningList.Contains(x.CardInfo().CardId)) &&
                 (_showRuleCards ||
                  (x.Group == Group.Leader
                    ? x.Faction == _nowSwitchFaction
                    : hasProjection
                        ? _projectedCardStates.TryGetValue(x.CardId, out var state) && state.ReasonCode != "card.not-allowed"
                        : DeckRuleEngine.IsCardSelectable(x.CardInfo(), leader, rules)))))
            .Where(x => ((_editorSearchMessage == "") ? true :
                (_translator.GetCardName(x.CardInfo().CardId).Contains(_editorSearchMessage, StringComparison.OrdinalIgnoreCase) ||
                 _translator.GetCardInfo(x.CardInfo().CardId).Contains(_editorSearchMessage, StringComparison.OrdinalIgnoreCase) ||
                 x.CardInfo().Strength.ToString().Contains(_editorSearchMessage) ||
                 x.Categories.Select(tag => _translator.GetText($"CardTag_{GwentMap.CategorieInfoMap[tag]}")).Any(text => text.Contains(_editorSearchMessage, StringComparison.OrdinalIgnoreCase))
                )))
            .Where(x => _showRuleCards || (_nowEditorGroup == Group.Leader ? x.Group != Group.Leader : x.Group == _nowEditorGroup))
            .ToList(),
            resetScroll
        );
    }

    public void SetEditorDeck(DeckModel deck)
    {
        RemoveAllChild(EditorCListContext);
        if (_nowEditorDeck.Id != "blacklist")
        {
            var factionIndex = GetFactionIndex(_nowSwitchFaction);
            var leader = Instantiate(EditorLeadersPrefab[factionIndex]).GetComponent<LeaderShow>();
            leader.SetLeader(_nowSwitchLeaderId);
            leader.GetComponent<EditorListLeader>().Id = _nowSwitchLeaderId;
            leader.transform.SetParent(EditorCListContext, false);
        }
        var selectedRules = deck.Deck
            .Where(DeckRuleEngine.IsRuleCard)
            .Distinct()
            .Select(x => GwentMap.CardMap[x])
            .ToList();
        if (selectedRules.Count > 0)
        {
            CreateRuleSectionTitle(selectedRules.Count);
            selectedRules.ForAll(CreateRuleCardRow);
            CreateDeckDivider();
        }

        deck.Deck.Where(x => !DeckRuleEngine.IsRuleCard(x)).Select(x => GwentMap.CardMap[x])
            .OrderByDescending(x => x.Group)
            .ThenByDescending(x => x.Strength)
            .GroupBy(x => x.CardId)
        .ForAll(x =>
        {
            var card = Instantiate(EditorListCardPrefab).GetComponent<ListCardShowInfo>();
            card.SetCardInfo(x.Key, x.Count());
            card.GetComponent<EditorListCard>().Id = x.Key;
            card.transform.SetParent(EditorCListContext, false);
        });
        RefreshDeckCountersAndHeight(deck);
    }

    private void RefreshDeckCountersAndHeight(DeckModel deck)
    {
        var selectedRuleCount = (deck?.Deck ?? new List<string>())
            .Where(DeckRuleEngine.IsRuleCard)
            .Distinct()
            .Count();
        var playableCards = (deck?.Deck ?? new List<string>()).Where(x => !DeckRuleEngine.IsRuleCard(x)).ToList();
        var rules = ResolveRules(deck);
        var validation = DeckRuleEngine.Validate(deck, rules, true);
        AllCount.text = playableCards.Count + FormatLimit(GetDeckMaximum(rules));
        UpdateMinimumDeckBadge(deck.Id == "blacklist" ? null : GetDeckMinimum(rules));
        bool valid = deck.Id == "blacklist" ||
                     (ProjectionMatchesCurrentDeck() ? _projectionHealthy && _acceptedProjection.IsComplete : validation.IsComplete) ||
                     (!HasRuleCards(deck) && isSpecial && deck.IsSpecialDeck());
        AllCount.color = valid ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        AllCountText.color = valid ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        if (_nowEditorDeck.Id == "blacklist")
        {
            CopperCount.text = $"{playableCards.Count(x => GwentMap.CardMap[x].Group == Group.Copper)}";
            GoldCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Gold).Count()}";
            SilverCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Silver).Count()}";
        }
        else
        {
            CopperCount.text = $"{playableCards.Count(x => GwentMap.CardMap[x].Group == Group.Copper)}{FormatLimit(GetGroupMaximum(rules, Group.Copper))}";
            if (isSpecial && !HasRuleCards(deck))
                GoldCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Gold).Count()}/12";
            else
                GoldCount.text = $"{playableCards.Count(x => GwentMap.CardMap[x].Group == Group.Gold)}{FormatLimit(GetGroupMaximum(rules, Group.Gold))}";
            SilverCount.text = $"{playableCards.Count(x => GwentMap.CardMap[x].Group == Group.Silver)}{FormatLimit(GetGroupMaximum(rules, Group.Silver))}";
        }
        //*****************
        //等待补充？？？
        //*****************
        var ordinaryCount = deck.Deck.Where(x => !DeckRuleEngine.IsRuleCard(x)).Distinct().Count();
        var ruleHeight = selectedRuleCount > 0 ? 34f + selectedRuleCount * 44.1f + 10f : 0f;
        var height = ((10 + 75 + 2.6f + (41.5f + 2.6f) * ordinaryCount) + ruleHeight + 5f);
        EditorCListContext.sizeDelta = new Vector2(0, height);
        //EditorCListScroll.value = 1;
    }

    private void UpdateOrdinaryDeckRow(string cardId)
    {
        if (string.IsNullOrWhiteSpace(cardId) || DeckRuleEngine.IsRuleCard(cardId)) return;
        var copies = _nowEditorDeck.Deck.Count(x => x == cardId);
        var existing = GetAllChilds<EditorListCard>(EditorCListContext)
            .FirstOrDefault(x => x.Id == cardId);

        if (copies <= 0)
        {
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
                Destroy(existing.gameObject);
            }
        }
        else if (existing == null)
        {
            var card = Instantiate(EditorListCardPrefab).GetComponent<ListCardShowInfo>();
            card.SetCardInfo(cardId, copies);
            card.GetComponent<EditorListCard>().Id = cardId;
            card.transform.SetParent(EditorCListContext, false);
        }
        else
        {
            var info = existing.GetComponent<ListCardShowInfo>();
            if (info.Count != null) info.Count.SetActive(copies > 1);
            if (info.CountText != null) info.CountText.text = $"x{copies}";
        }

        GetAllChilds<EditorListCard>(EditorCListContext)
            .Where(x => !DeckRuleEngine.IsRuleCard(x.Id) && x.gameObject.activeSelf)
            .OrderByDescending(x => GwentMap.CardMap[x.Id].Group)
            .ThenByDescending(x => GwentMap.CardMap[x.Id].Strength)
            .ThenBy(x => x.Id, StringComparer.Ordinal)
            .ForAll(x => x.transform.SetAsLastSibling());
    }

    private void CreateRuleSectionTitle(int count)
    {
        var header = new GameObject("RuleSectionTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline), typeof(LayoutElement));
        header.transform.SetParent(EditorCListContext, false);
        SetDeckListRowWidth(header.GetComponent<RectTransform>(), 32f);
        var layout = header.GetComponent<LayoutElement>();
        layout.minHeight = 32;
        layout.preferredHeight = 32;
        layout.preferredWidth = GetDeckListRowWidth();
        var background = header.GetComponent<Image>();
        background.color = new Color32(12, 36, 39, 238);
        background.raycastTarget = false;
        var outline = header.GetComponent<Outline>();
        outline.effectColor = new Color32(116, 88, 42, 190);
        outline.effectDistance = new Vector2(1, -1);

        var accent = new GameObject("Accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.transform.SetParent(header.transform, false);
        var accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = Vector2.zero;
        accentRect.anchorMax = new Vector2(0, 1);
        accentRect.pivot = new Vector2(0, .5f);
        accentRect.sizeDelta = new Vector2(4, 0);
        accentRect.anchoredPosition = Vector2.zero;
        accent.GetComponent<Image>().color = new Color32(220, 174, 78, 255);
        accent.GetComponent<Image>().raycastTarget = false;

        var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        labelObject.transform.SetParent(header.transform, false);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(14, 0);
        labelRect.offsetMax = new Vector2(-8, 0);
        var label = labelObject.GetComponent<Text>();
        label.font = SwitchButtonText != null ? SwitchButtonText.font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = 15;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleLeft;
        label.color = new Color32(218, 169, 76, 255);
        label.text = Local($"规则卡 · {count}", $"RULE CARDS · {count}");
        label.raycastTarget = false;
        var labelOutline = labelObject.AddComponent<Outline>();
        labelOutline.effectColor = new Color32(0, 12, 14, 220);
        labelOutline.effectDistance = new Vector2(1, -1);
    }

    private void CreateRuleCardRow(GwentCard rule)
    {
        var card = Instantiate(EditorListCardPrefab).GetComponent<ListCardShowInfo>();
        card.SetCardInfo(rule.CardId, 1);
        card.GetComponent<EditorListCard>().Id = rule.CardId;
        card.transform.SetParent(EditorCListContext, false);
        RuleCardListVisual.Apply(card.gameObject, SwitchButtonText != null ? SwitchButtonText.font : null, Local("否", "no") == "no");
    }

    private void CreateDeckDivider()
    {
        var divider = new GameObject("DeckCardsDivider", typeof(RectTransform), typeof(LayoutElement));
        divider.transform.SetParent(EditorCListContext, false);
        SetDeckListRowWidth(divider.GetComponent<RectTransform>(), 8f);
        divider.GetComponent<LayoutElement>().preferredHeight = 8;
        divider.GetComponent<LayoutElement>().preferredWidth = GetDeckListRowWidth();
        var line = new GameObject("Line", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        line.transform.SetParent(divider.transform, false);
        var lineRect = line.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0, .5f);
        lineRect.anchorMax = new Vector2(1, .5f);
        lineRect.sizeDelta = new Vector2(-12, 1);
        lineRect.anchoredPosition = Vector2.zero;
        line.GetComponent<Image>().color = new Color32(91, 105, 100, 90);
    }

    private float GetDeckListRowWidth()
        => Mathf.Max(310f, EditorCListContext != null ? EditorCListContext.rect.width - 16f : 310f);

    private void SetDeckListRowWidth(RectTransform rect, float height)
    {
        if (rect == null) return;
        rect.sizeDelta = new Vector2(GetDeckListRowWidth(), height);
    }

    private ResolvedDeckRuleSet ResolveRules(DeckModel deck)
    {
        var manifest = _clientService.FeatureManifest ?? new GameFeatureManifest { RulesetVersion = "offline-standard" };
        var selected = (deck?.Deck ?? new List<string>()).Where(DeckRuleEngine.IsRuleCard).Distinct(StringComparer.Ordinal);
        return DeckRuleEngine.Resolve(manifest.RuleCards, selected, manifest.RulesetVersion, manifest.CardPools);
    }

    private bool HasRuleCards(DeckModel deck)
        => (deck?.Deck ?? new List<string>()).Any(DeckRuleEngine.IsRuleCard);

    private int GetAvailableCopies(CardStatus card)
    {
        if (_nowEditorDeck == null) return 0;
        var existing = _nowEditorDeck.Deck.Count(x => x == card.CardId);
        if (_nowEditorDeck.Id == "blacklist") return Math.Max(0, 1 - existing);
        if (DeckRuleEngine.IsRuleCard(card.CardId))
        {
            if (existing > 0) return 0;
            if (RuleSnapshotMatchesCurrentDeck() && _projectedCardStates.TryGetValue(card.CardId, out var projectedRuleState))
                return projectedRuleState.Selectable ? 1 : 0;
            var candidate = CloneDeck(_nowEditorDeck);
            candidate.Deck.Add(card.CardId);
            var projection = DeckBuildingProjectionEngine.Project(
                _clientService.FeatureManifest ?? new GameFeatureManifest { RulesetVersion = "offline-standard" },
                new DeckBuildingProjectionRequest
                {
                    Action = "add",
                    CandidateCardId = card.CardId,
                    ConfirmNormalization = true,
                    Deck = candidate
                });
            return projection.IsValid &&
                (projection.NormalizedDeck?.Deck ?? new List<string>()).Contains(card.CardId)
                ? 1
                : 0;
        }
        if (isSpecial && !HasRuleCards(_nowEditorDeck))
            return Math.Max(0, (card.Group == Group.Silver ? 1 : 3) - existing);

        if (_availableCopiesSnapshot.TryGetValue(card.CardId, out var snapshot))
            return snapshot;

        var rules = CurrentRuleSnapshot();
        var probe = BuildLocalConstraintProbe(_nowEditorDeck, rules);
        var addable = 0;
        while (addable < DeckBuildingProjectionEngine.AbsoluteCopyLimit &&
               DeckRuleEngine.CanAddCard(probe, card.CardId, rules))
        {
            probe.Deck.Add(card.CardId);
            addable++;
        }
        _availableCopiesSnapshot[card.CardId] = addable;
        return addable;
    }

    private void AdjustAvailableCopiesSnapshot(string cardId, int delta)
    {
        if (string.IsNullOrWhiteSpace(cardId) || DeckRuleEngine.IsRuleCard(cardId)) return;
        if (!_availableCopiesSnapshot.TryGetValue(cardId, out var current)) return;
        _availableCopiesSnapshot[cardId] = Math.Max(0,
            Math.Min(DeckBuildingProjectionEngine.AbsoluteCopyLimit, current + delta));
    }

    // Keep ordinary clicks cheap and stable: update the clicked identity in
    // place, and only invalidate the wider visible snapshot when a shared
    // deck/group cap has just become binding (or has just been released).
    // This prevents both the old "every X64 counts down" bug and stale silver
    // cards remaining clickable after the sixth silver was selected.
    private void RefreshOrdinaryAvailabilityAfterEdit(string cardId, int snapshotDelta)
    {
        AdjustAvailableCopiesSnapshot(cardId, snapshotDelta);
        if (_nowEditorDeck == null || !GwentMap.CardMap.TryGetValue(cardId, out var changedCard) ||
            !GwentMap.CardMap.TryGetValue(_nowEditorDeck.Leader, out var leader))
        {
            RefreshVisibleCardAvailability(cardId);
            return;
        }

        var rules = CurrentRuleSnapshot();
        var ordinaryCards = _nowEditorDeck.Deck
            .Where(x => !DeckRuleEngine.IsRuleCard(x) && GwentMap.CardMap.ContainsKey(x))
            .Select(x => GwentMap.CardMap[x])
            .ToList();
        var sharedBoundaryChanged = false;
        foreach (var constraint in rules?.Constraints ?? new List<DeckConstraintDefinition>())
        {
            if (!constraint.Max.HasValue) continue;
            var kind = (constraint.Kind ?? "").ToLowerInvariant();
            int current;
            if (kind == "deck-size")
                current = ordinaryCards.Count;
            else if (kind == "card-count" && DeckRuleEngine.Matches(constraint.Filter, changedCard, leader))
                current = ordinaryCards.Count(x => DeckRuleEngine.Matches(constraint.Filter, x, leader));
            else
                continue;

            if (current >= constraint.Max.Value ||
                (snapshotDelta > 0 && current == constraint.Max.Value - 1))
            {
                sharedBoundaryChanged = true;
                break;
            }
        }

        if (sharedBoundaryChanged)
        {
            _availableCopiesSnapshot.Clear();
            RefreshVisibleCardAvailability();
        }
        else
        {
            RefreshVisibleCardAvailability(cardId);
        }
    }

    private ResolvedDeckRuleSet CurrentRuleSnapshot()
        => RuleSnapshotMatchesCurrentDeck()
            ? _acceptedProjection.ResolvedRules
            : ResolveRules(_nowEditorDeck);

    private bool RuleSnapshotMatchesCurrentDeck()
    {
        if (_acceptedProjection?.ResolvedRules == null || _nowEditorDeck == null) return false;
        if (!string.Equals(_acceptedProjection.RulesetVersion,
                _clientService.FeatureManifest?.RulesetVersion ?? "", StringComparison.Ordinal)) return false;
        var selected = _nowEditorDeck.Deck.Where(DeckRuleEngine.IsRuleCard)
            .Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal);
        var projected = (_acceptedProjection.ResolvedRules.AppliedRuleCards ?? new List<string>())
            .OrderBy(x => x, StringComparer.Ordinal);
        return selected.SequenceEqual(projected);
    }

    // Invalid legacy cards may remain in a saved draft, but they must not make
    // every otherwise legal card appear grey. Build availability from the legal
    // portion while keeping the actual draft untouched and visible on the left.
    private static DeckModel BuildLocalConstraintProbe(DeckModel deck, ResolvedDeckRuleSet rules)
    {
        var probe = new DeckModel
        {
            Id = deck?.Id,
            Name = deck?.Name ?? "",
            Leader = deck?.Leader ?? "",
            Deck = (deck?.Deck ?? new List<string>()).Where(DeckRuleEngine.IsRuleCard).Distinct().ToList()
        };
        foreach (var cardId in (deck?.Deck ?? new List<string>()).Where(x => !DeckRuleEngine.IsRuleCard(x)))
        {
            if (!GwentMap.CardMap.ContainsKey(cardId)) continue;
            if (DeckRuleEngine.CanAddCard(probe, cardId, rules)) probe.Deck.Add(cardId);
        }
        return probe;
    }

    private void RefreshVisibleCardAvailability(string cardId = null)
    {
        foreach (var card in GetAllChilds<EditorUICoreCard>(EditorCardsContext))
            if (card?.cardShowInfo?.CurrentCore != null &&
                (string.IsNullOrWhiteSpace(cardId) || card.cardShowInfo.CurrentCore.CardId == cardId))
                card.Count = GetAvailableCopies(card.cardShowInfo.CurrentCore);
    }

    private bool ProjectionMatchesCurrentDeck()
        => _acceptedProjection?.NormalizedDeck != null && _nowEditorDeck != null &&
           string.Equals(_acceptedProjection.NormalizedDeck.Leader, _nowEditorDeck.Leader, StringComparison.Ordinal) &&
           (_acceptedProjection.NormalizedDeck.Deck ?? new List<string>()).SequenceEqual(_nowEditorDeck.Deck ?? new List<string>());

    private async Task<bool> ProjectCandidate(
        DeckModel candidate,
        string action,
        string candidateCardId,
        bool requireComplete = false)
    {
        var projection = await RequestProjection(candidate, action, candidateCardId, false, true);
        if (projection == null) return false;

        if (projection.RequiresConfirmation)
        {
            var names = projection.RemovedCards
                .GroupBy(x => x.CardId)
                .Take(8)
                .Select(x => $"{_translator.GetCardName(x.Key)} ×{x.Count()}");
            var accepted = await _globalUIService.YNMessageBox(
                Local("卡组调整确认", "Confirm deck adjustment"),
                Local("这次调整会使部分卡牌不再合法。确认后将移除：\n", "This change makes some cards illegal. Confirm to remove:\n") + string.Join("\n", names));
            if (!accepted) return false;
            projection = await RequestProjection(candidate, action, candidateCardId, true, true);
            if (projection == null) return false;
        }

        if (!projection.IsValid || (requireComplete && !projection.IsComplete))
        {
            IEnumerable<DeckValidationIssue> issues = projection.Issues;
            if (requireComplete && !projection.IsComplete && !(issues?.Any() ?? false))
                issues = new[] { new DeckValidationIssue { Code = "count.min" } };
            await ShowRuleIssues(issues);
            return false;
        }

        if (action == "add" && !string.IsNullOrWhiteSpace(candidateCardId))
        {
            var previousCount = _nowEditorDeck?.Deck?.Count(x => x == candidateCardId) ?? 0;
            var projectedCount = projection.NormalizedDeck?.Deck?.Count(x => x == candidateCardId) ?? 0;
            if (projectedCount <= previousCount)
            {
                IEnumerable<DeckValidationIssue> rejectionIssues = projection.Issues.Count > 0
                    ? (IEnumerable<DeckValidationIssue>)projection.Issues
                    : new[] { new DeckValidationIssue { Code = "card.not-allowed", CardId = candidateCardId } };
                await ShowRuleIssues(rejectionIssues);
                return false;
            }
        }

        candidate.Leader = projection.NormalizedDeck.Leader;
        candidate.Deck = projection.NormalizedDeck.Deck.ToList();
        AcceptProjection(projection);
        return true;
    }

    private async Task RefreshProjectionForCurrentDeck()
    {
        if (_nowEditorDeck == null || _nowEditorDeck.Id == "blacklist" || string.IsNullOrWhiteSpace(_nowEditorDeck.Leader))
            return;
        var projection = await RequestProjection(_nowEditorDeck, "refresh", "", false, false);
        if (projection == null || EditorStatus != EditorStatus.EditorDeck || _nowEditorDeck == null) return;
        AcceptProjection(projection);
        SetEditorDeck(_nowEditorDeck);
        AutoSetEditorCards();
    }

    private async Task<DeckBuildingProjection> RequestProjection(
        DeckModel deck,
        string action,
        string candidateCardId,
        bool confirmNormalization,
        bool showFailure)
    {
        var revision = _projectionRevisionGate.BeginRequest();
        try
        {
            var projection = await _clientService.GetDeckBuildingProjection(new DeckBuildingProjectionRequest
            {
                Revision = revision,
                PreviousProjectionFingerprint = _acceptedProjection?.ProjectionFingerprint ?? "",
                PreviousPoolFingerprint = _acceptedProjection?.PoolFingerprint ?? "",
                Action = action ?? "refresh",
                CandidateCardId = candidateCardId ?? "",
                ConfirmNormalization = confirmNormalization,
                ClientFeatureLevel = 2,
                Deck = CloneDeck(deck)
            });
            if (!_projectionRevisionGate.TryAccept(revision, projection))
                return null;
            return projection;
        }
        catch (Exception e)
        {
            if (_projectionRevisionGate.IsLatest(revision)) _projectionHealthy = false;
            Debug.LogWarning($"Deck projection failed: {e.Message}");
            if (showFailure)
                await _globalUIService.YNMessageBox(
                    Local("暂时无法验证卡组", "Deck validation unavailable"),
                    Local("未能取得服务端最新组卡规则。为避免卡组损坏，本次修改未应用。", "The latest server deck rules could not be loaded. This edit was not applied."),
                    isOnlyYes: true);
            return null;
        }
    }

    private void AcceptProjection(DeckBuildingProjection projection)
    {
        if (projection == null) return;
        _acceptedProjection = projection;
        _projectionHealthy = projection.IsAuthoritative && string.IsNullOrWhiteSpace(projection.FailureCode) && !projection.RequiresConfirmation;
        _availableCopiesSnapshot.Clear();
        _projectedCardStates.Clear();
        foreach (var state in projection.CardStates ?? new List<DeckBuildingCardState>())
            _projectedCardStates[state.CardId] = state;
    }

    private Task<bool> ShowRuleIssues(IEnumerable<DeckValidationIssue> issues)
    {
        var issueList = (issues ?? Enumerable.Empty<DeckValidationIssue>()).ToList();
        var resolutionIssues = issueList.Where(IsRuleResolutionIssue).ToList();
        if (resolutionIssues.Count > 0) issueList = resolutionIssues;
        if (issueList.Count == 0)
            issueList.Add(new DeckValidationIssue { Code = "deck.invalid" });
        var text = string.Join("\n", issueList.Take(8).Select(RuleIssueText));
        var title = resolutionIssues.Count > 0
            ? Local("规则冲突", "Rule conflict")
            : Local("卡组不符合要求", "Deck requirements not met");
        return _globalUIService.YNMessageBox(title, text, isOnlyYes: true);
    }

    private static bool IsRuleResolutionIssue(DeckValidationIssue issue)
        => issue != null && (issue.Code == "rules.conflict" ||
                             issue.Code == "rules.requires" ||
                             issue.Code == "rules.priority-conflict" ||
                             issue.Code == "rules.unknown" ||
                             issue.Code == "rules.disabled" ||
                             issue.Code == "rules.constraint-invalid" ||
                             issue.Code == "rules.pool-unknown");

    private string RuleIssueText(DeckValidationIssue issue)
    {
        switch (issue.Code)
        {
            case "rules.conflict": return Local($"{_translator.GetCardName(issue.CardId)} 与 {_translator.GetCardName(issue.RelatedId)} 不能同时使用", $"{_translator.GetCardName(issue.CardId)} conflicts with {_translator.GetCardName(issue.RelatedId)}");
            case "rules.requires": return Local($"{_translator.GetCardName(issue.CardId)} 需要先选择 {_translator.GetCardName(issue.RelatedId)}", $"{_translator.GetCardName(issue.CardId)} requires {_translator.GetCardName(issue.RelatedId)}");
            case "rules.priority-conflict": return Local("规则卡对同一项设置给出了冲突结果", "Rule cards provide conflicting values for the same setting");
            case "rules.deck-building-effect-failed": return Local("一张规则卡的组卡逻辑执行失败，本次修改已取消", "A rule card's deck-building logic failed; this edit was cancelled");
            case "rules.normalization-cycle": return Local("规则组合导致卡组调整循环，请移除冲突规则", "The rule combination creates a deck-adjustment cycle");
            case "rules.normalization-limit": return Local("规则组合无法在安全次数内稳定，请移除冲突规则", "The rule combination did not stabilize within the safety limit");
            case "rules.unknown": return Local($"服务器未提供规则卡 {issue.CardId}", $"Rule card {issue.CardId} was not provided by the server");
            case "rules.faction": return Local("当前领袖势力不能使用这张规则卡", "This rule card is not available to the current leader faction");
            case "card.not-allowed": return Local($"{_translator.GetCardName(issue.CardId)} 不符合当前规则", $"{_translator.GetCardName(issue.CardId)} is not allowed by the current rules");
            case "card.denied": return Local($"{_translator.GetCardName(issue.CardId)} 被当前规则禁止", $"{_translator.GetCardName(issue.CardId)} is denied by the current rules");
            case "copies.max": return Local($"{_translator.GetCardName(issue.CardId)} 超过同名上限", $"{_translator.GetCardName(issue.CardId)} exceeds the copy limit");
            case "count.max": return Local("卡组超过当前规则的数量上限", "The deck exceeds a limit imposed by the current rules");
            case "count.min": return Local("卡组尚未达到当前规则要求的最低数量", "The deck has not reached the required minimum");
            case "deck.invalid": return Local("卡组尚未满足当前组卡要求", "The deck does not yet meet the current requirements");
            default: return issue.Code;
        }
    }

    private string Local(string zhCn, string en)
        => string.Equals(_translator.TextLocalization.ChosenLanguage?.Filename, "en", StringComparison.OrdinalIgnoreCase) ? en : zhCn;

    private static DeckModel CloneDeck(DeckModel deck) => new DeckModel
    {
        Id = deck?.Id,
        Name = deck?.Name ?? "",
        Leader = deck?.Leader ?? "",
        Deck = deck?.Deck?.ToList() ?? new List<string>()
    };

    private int? GetDeckMaximum(ResolvedDeckRuleSet rules)
    {
        if (ProjectionMatchesCurrentDeck())
            return _acceptedProjection.Limits?
                .Where(x => x.Kind == "deck-size" && x.Max.HasValue)
                .Select(x => x.Max).Min();
        return rules?.Constraints?.Where(x => x.Kind == "deck-size" && x.Max.HasValue).Select(x => x.Max).Min();
    }

    private int? GetDeckMinimum(ResolvedDeckRuleSet rules)
    {
        if (ProjectionMatchesCurrentDeck())
            return _acceptedProjection.Limits?
                .Where(x => x.Kind == "deck-size" && x.Min.HasValue)
                .Select(x => x.Min).Max();
        return rules?.Constraints?.Where(x => x.Kind == "deck-size" && x.Min.HasValue).Select(x => x.Min).Max();
    }

    private void UpdateMinimumDeckBadge(int? minimum)
    {
        var shouldShow = minimum.HasValue && minimum.Value != 25 && AllCountText != null;
        if (!shouldShow)
        {
            if (_minimumDeckBadge != null) _minimumDeckBadge.SetActive(false);
            return;
        }

        if (_minimumDeckBadge == null)
        {
            var parent = AllCountText.transform.parent;
            _minimumDeckBadge = new GameObject("DeckMinimumBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
            _minimumDeckBadge.transform.SetParent(parent, false);
            var rect = _minimumDeckBadge.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(.5f, .5f);
            rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(118, 0);
            rect.sizeDelta = new Vector2(92, 25);
            var image = _minimumDeckBadge.GetComponent<Image>();
            image.color = new Color32(15, 48, 50, 235);
            image.raycastTarget = false;
            var outline = _minimumDeckBadge.GetComponent<Outline>();
            outline.effectColor = new Color32(201, 154, 67, 205);
            outline.effectDistance = new Vector2(1, -1);

            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
            labelObject.transform.SetParent(_minimumDeckBadge.transform, false);
            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(4, 1);
            labelRect.offsetMax = new Vector2(-4, -1);
            var label = labelObject.GetComponent<Text>();
            label.font = AllCountText.font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 13;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color32(239, 230, 201, 255);
            label.raycastTarget = false;
            var labelOutline = labelObject.GetComponent<Outline>();
            labelOutline.effectColor = new Color32(0, 10, 12, 220);
            labelOutline.effectDistance = new Vector2(1, -1);
        }

        _minimumDeckBadge.SetActive(true);
        _minimumDeckBadge.GetComponentInChildren<Text>().text = Local($"最低 {minimum.Value}", $"MIN {minimum.Value}");
    }

    private int? GetGroupMaximum(ResolvedDeckRuleSet rules, Group group)
    {
        if (ProjectionMatchesCurrentDeck())
            return _acceptedProjection.Limits?
                .Where(x => x.Kind == "card-count" && x.Max.HasValue &&
                            (x.Groups?.Count ?? 0) == 1 && x.Groups[0] == group)
                .Select(x => x.Max).Min();
        return rules?.Constraints?
            .Where(x => x.Kind == "card-count" && x.Max.HasValue &&
                        (x.Filter?.Groups?.Count ?? 0) == 1 && x.Filter.Groups[0] == group)
            .Select(x => x.Max).Min();
    }

    private static string FormatLimit(int? maximum) => maximum.HasValue ? "/" + maximum.Value : "";


    public int GetFactionIndex(Faction faction)
    {
        return EditorFactionIndex.Select((item, index) => (item, index)).Single(x => x.item == faction).index;
    }

    public List<T> GetAllChilds<T>(Transform context)
    {
        var count = context.childCount;
        List<T> childs = new List<T>();
        for (var i = 0; i < count; i++)
        {
            if (context.GetChild(i).GetComponent<T>() != null)
            {
                childs.Add(context.GetChild(i).GetComponent<T>());
            }
        }
        return childs;
    }
    public async void CopyDeckCode()
    {
        var deckCode = _nowEditorDeck.CompressDeck();
        await _globalUIService.YNMessageBox(_translator.GetText("Copied"), _translator.GetText("Code") +": "+ deckCode, "PopupWindow_OkButton", isOnlyYes: true);
        Debug.Log(_nowEditorDeck.Name);
        GUIUtility.systemCopyBuffer = deckCode;
    }
    public async Task<bool> AddDeckFromCode(string deckCode, string name = "Default")
    {
        Debug.Log(deckCode);
        var deck = deckCode.DeCompressToDeck();
        if (!DeckRuleEngine.CanPlayerSelectRuleDeck(_clientService.FeatureManifest, deck))
        {
            await _globalUIService.YNMessageBox(
                Local("暂不可用", "Unavailable"),
                Local("服务器当前未开放玩家规则卡组。原有卡组数据不会被删除。", "Player rule-card decks are currently disabled by the server. Existing data is preserved."),
                isOnlyYes: true);
            return false;
        }
        deck.Name = string.IsNullOrWhiteSpace(name) ? _translator.GetCardName(deck.Leader.CardInfo().CardId) + " #" + (_clientService.User.Decks.Count + 1)
                                                    : (name.Length >= 20 ? name.Substring(0, 20) : name);
        var featureValidation = DeckRuleEngine.Validate(deck, ResolveRules(deck), false);
        var legacyDraft = !HasRuleCards(deck) && (deck.IsBasicDeck() || deck.IsSpecialDeck() || deck.IsHalfBasicDeck());
        if (!(featureValidation.IsValid || legacyDraft))
        {
            await _globalUIService.YNMessageBox("PopupWindow_AddDeckErrorTitle",
                                               "Code: " + deckCode, "PopupWindow_OkButton", isOnlyYes: true);
            return false;
        }
        if (await _clientService.HubConnection.InvokeAsync<bool>("AddDeck", deck))
        {
            _nowEditorDeck = deck;
            Debug.Log($"inner {_nowEditorDeck.Id }");
            _clientService.User.Decks.Add(_nowEditorDeck);
            await _globalUIService.YNMessageBox(_translator.GetText("Added"), _translator.GetText("Code") + ": " + deckCode, "PopupWindow_OkButton", isOnlyYes: true);
            return true;
        }
        else
        {
            await _globalUIService.YNMessageBox("PopupWindow_AddDeckErrorTitle",
                                                "Code: " + deckCode, "PopupWindow_OkButton", isOnlyYes: true);
            return false;
        }
    }
    public void OpenDeckCodeInput()
    {
        DeckCodeInputName.text = "";
        DeckCodeInputCode.text = "";
        DeckCodeInputBackGround.SetActive(true);
    }
    public async void ConfirmDeckCodeInput()
    {
        await AddDeckFromCode(DeckCodeInputCode.text, DeckCodeInputName.text);
        CloseDeckCodeInput();
        Debug.Log($"outer {_nowEditorDeck.Id }");
        DOTween.To(() => ShowCardsTitle.anchoredPosition, x => ShowCardsTitle.anchoredPosition = x,
            new Vector2(0, 478.5f), 0.5f);//降下Title,设定标题 ********
        DOTween.To(() => LeftSwitchMenu.anchoredPosition, x => LeftSwitchMenu.anchoredPosition = x,
            new Vector2(-1700, 0), 0.5f);//收回Left
        DOTween.To(() => RightSwitchMenu.anchoredPosition, x => RightSwitchMenu.anchoredPosition = x,
            new Vector2(1700, 0), 0.5f);//收回Right
        ShowDeckEditorClick(_nowEditorDeck.Id);
    }
    public void CloseDeckCodeInput()
    {
        DeckCodeInputBackGround.SetActive(false);
    }
}

internal sealed class CompactRuleDeckRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private EditorInfo _owner;
    private CardStatus _card;

    public void Initialize(EditorInfo owner, CardStatus card)
    {
        _owner = owner;
        _card = card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_owner != null && _card != null) _owner.SelectSwitchUICard(_card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_owner != null && _card != null) _owner.SelectSwitchUICard(_card, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_owner != null && _card != null) _owner.ClickEditorListCard(_card.CardId);
    }
}
