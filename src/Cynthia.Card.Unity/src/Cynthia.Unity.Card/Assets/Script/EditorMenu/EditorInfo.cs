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
        BlacklistButtonText.text = _translator.GetText("EditorMenu_BlacklistButton");
        //---------------------------------------------------------------------------
    }

    public void SetEditorCardInfo(IList<CardStatus> cards)
    {   //设置已有卡牌
        var pagenum = 30;
        EditorCardsScroll.value = 1;
        RemoveAllChild(EditorCardsContext);
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
                var card = Instantiate(EditorMenuCardPrefab).GetComponent<EditorUICoreCard>();
                card.cardShowInfo.setCurrentCore(x, true);
                var canAdd = 0;
                if (_nowEditorDeck.Id == "blacklist")
                    canAdd = 1;
                else if (!isSpecial)
                    canAdd = (x.Group == Group.Copper ? 3 : 1);
                else
                    canAdd = ((x.Group == Group.Gold || x.Group == Group.Copper) ? 3 : 1);
                card.Count = (canAdd - _nowEditorDeck.Deck.Where(c => c == x.CardId).Count());
                card.transform.SetParent(EditorCardsContext, false);
            });
        }
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
        };
        EditorCardsScroll.onValueChanged.AddListener(_editorCardScrollEvent);
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

    public void OpenEditor(bool IsMoveLeftRight = true)
    {
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

    public void SetDeckList(IList<DeckModel> decks)
    {
        //设置已有卡组
        RemoveAllChild(ShowDecksContext);
        var button = Instantiate(AddDeckButtonPrefab);
        button.transform.SetParent(ShowDecksContext, false);
        //-----
        decks.ForAll(x =>
        {
            if (_deckPrefabMap == null) Start();
            if (x.Id != "blacklist")
            {
                var deck = Instantiate(_deckPrefabMap[GwentMap.CardMap[x.Leader].Faction]);
                string leaderartid = GwentMap.CardMap[x.Leader].CardArtsId;
                deck.GetComponent<DeckShowInfo>().SetDeckInfo(x.Name, x.IsBasicDeck() || x.IsSpecialDeck() || (x.IsBlacklist() && x.Id == "blacklist"));
                deck.GetComponent<DeckEditorMiniatures>().SetMiniatureArt(leaderartid);
                deck.GetComponent<EditorShowDeck>().Id = x.Id;
                deck.transform.SetParent(ShowDecksContext, false);
            }
        });
        //----
        var count = decks.Count();
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
        _nowEditorDeck = deck;
        isSpecial = (!deck.IsHalfBasicDeck()) && deck.IsHalfSpecialDeck();
        _nowSwitchLeaderId = deck.Leader;
        _nowSwitchFaction = GwentMap.CardMap[deck.Leader].Faction;
        //
        ResetEditorCore();
        ShowCardsTitle.anchoredPosition = new Vector2(0, 150f);
        EditorCardsTitle.anchoredPosition = new Vector2(0, -63f);
        EditorBodyCore.SetActive(true);
        EditorBodyMian.SetActive(false);
        EditorStatus = EditorStatus.EditorDeck;
    }
    //=============================================================================================================================
    //以上为展示卡牌相关的内容,以下为展示框选择相关内容
    public void SwitchDeckClick()
    {
        if (_nowEditorDeck.Id == "blacklist")
            return;
        var decks = _nowEditorDeck.Deck;
        isSpecial = !isSpecial;
        if (!isSpecial)
        {
            var gold = decks.Where(x => GwentMap.CardMap[x].Group == Group.Gold).Distinct().ToList().Take(4).ToList();
            var silver = decks.Where(x => GwentMap.CardMap[x].Group == Group.Silver).ToList();
            var copper = decks.Where(x => GwentMap.CardMap[x].Group == Group.Copper).ToList();
            gold.AddRange(silver);
            gold.AddRange(copper);
            _nowEditorDeck.Deck = gold;
        }
        else
        {
            var gold = decks.Where(x => (!SpecialBanningList.Contains(GwentMap.CardMap[x].CardId)) && (GwentMap.CardMap[x].Group == Group.Gold)).ToList();
            var silver = decks.Where(x => GwentMap.CardMap[x].Group == Group.Silver).ToList();
            var copper = decks.Where(x => GwentMap.CardMap[x].Group == Group.Copper).Distinct().ToList();
            gold.AddRange(silver);
            gold.AddRange(copper);
            _nowEditorDeck.Deck = gold;
        }

        SetEditorDeck(_nowEditorDeck);
        //=============================================================================================================================
        AutoSetEditorCards();
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
    {   //点击新建按钮后
        if (_clientService.User.Decks.Count >= 1000)
        {
            _globalUIService.YNMessageBox(_translator.GetText("PopupWindow_DeckLimitTitle"), _translator.GetText("PopupWindow_DeckLimitDesc"));
        }
        else
        {
            /*
            Titile x:0 | Y:478.5 true    Y: 605 false
            Left y:0 | X:-470 true     X: -1700 false
            Right y:0 | X: 468 true     X: 1700 false*/
            EditorStatus = EditorStatus.EditorDeck;
            _nowSwitchLeaderId = null;
            _nowEditorDeck = new DeckModel() { Leader = _nowSwitchLeaderId, Deck = _clientService.User.Blacklist == null ? new List<string>() : _clientService.User.Blacklist.Blacklist, Id = "blacklist" };

            EditorBodyCore.SetActive(true);
            EditorBodyMian.SetActive(false);
            ResetEditorCore();

            DeckName.text = _translator.GetText("EditorMenu_DefaultDeckname");
        }

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
            if (EditorArtCard.gameObject.activeSelf || ShowArtCard.gameObject.activeSelf)
            {
                RightClickedCardID = LastHoveredCard;
                Debug.Log("Right Clicked ID: "+RightClickedCardID);
                //RighClickActive=true;
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
            ResetEditorCore();
            EditorStatus = EditorStatus.EditorDeck;
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

    public void ResetEditorCore()
    {//初始化
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

    public void ClickEditorListCard(string id)
    {//点击了卡牌   应该从卡组去除对应卡牌,并且更新显示
        var subIndex = _nowEditorDeck.Deck.Select((item, index) => (item, index)).First(x => x.item == id).index;
        _nowEditorDeck.Deck.RemoveAt(subIndex);
        SetEditorDeck(_nowEditorDeck);
        //**************************************************
        var c = GetAllChilds<EditorUICoreCard>(EditorCardsContext).Where(x => x.cardShowInfo.CurrentCore.CardId == id);
        c.ForAll(x => { x.Count++; });
    }

    public void ClickEditorUICoreCard(CardStatus card)
    {//点击了显示卡牌  应该判断是否应该添加卡牌,如果可以,添加并且更新显示,否则跳出消息提醒
        var count = _nowEditorDeck.Deck.Where(x => x == card.CardId).Count();
        if (_nowEditorDeck.Id == "blacklist")
        {
            if (!(count >= 1 || (_nowEditorDeck.Deck.Count >= 2)))
            {   //如果超过上限,禁止加入卡牌
                _nowEditorDeck.Deck.Add(card.CardId);
                SetEditorDeck(_nowEditorDeck);
                //**********************************************
                var c = GetAllChilds<EditorUICoreCard>(EditorCardsContext).Where(x => x.cardShowInfo.CurrentCore.CardId == card.CardId);
                c.ForAll(x => { x.Count--; });
            }

        }
        else if (!isSpecial)
        {
            if (!((card.Group == Group.Copper && count >= 3) ||
                (card.Group != Group.Copper && count >= 1) ||
                (_nowEditorDeck.Deck.Count >= 40) ||
                (card.Group == Group.Silver && _nowEditorDeck.Deck.Where(x => x.CardInfo().Group == Group.Silver).Count() >= 6) ||
                (card.Group == Group.Gold && _nowEditorDeck.Deck.Where(x => x.CardInfo().Group == Group.Gold).Count() >= 4)))
            {   //如果超过上限,禁止加入卡牌
                _nowEditorDeck.Deck.Add(card.CardId);
                SetEditorDeck(_nowEditorDeck);
                //**********************************************
                var c = GetAllChilds<EditorUICoreCard>(EditorCardsContext).Where(x => x.cardShowInfo.CurrentCore.CardId == card.CardId);
                c.ForAll(x => { x.Count--; });
            }
        }
        else
        {
            if (!((card.Group == Group.Silver && count >= 1) ||
               ((card.Group == Group.Gold || card.Group == Group.Copper) && count >= 3) ||
               (_nowEditorDeck.Deck.Count >= 40) ||
               (card.Group == Group.Silver && _nowEditorDeck.Deck.Where(x => x.CardInfo().Group == Group.Silver).Count() >= 6) ||
               (card.Group == Group.Gold && _nowEditorDeck.Deck.Where(x => x.CardInfo().Group == Group.Gold).Count() >= 12)))
            {
                _nowEditorDeck.Deck.Add(card.CardId);
                SetEditorDeck(_nowEditorDeck);
                //**********************************************
                var c = GetAllChilds<EditorUICoreCard>(EditorCardsContext).Where(x => x.cardShowInfo.CurrentCore.CardId == card.CardId);
                c.ForAll(x => { x.Count--; });
            }
        }
        //Debug.Log("点击了菜单卡");
    }

    public void EditorGroupClick()
    {//点击了品质筛选
        if (!EditorGroupButtons.Any(x => x.isOn)) return;
        var result = EditorGroupButtons.Select((item, index) => (item, index)).First(x => x.item.isOn).index;
        var group = result == 0 ? Group.Leader : (result == 1 ? Group.Gold : (result == 2 ? Group.Silver : Group.Copper));
        if (_nowEditorGroup != group)
        {
            _nowEditorGroup = group;
            AutoSetEditorCards();
        }
    }

    public void EditorSearchChange(string value)
    {   //编辑卡牌中,搜索框改变
        _editorSearchMessage = value;
        AutoSetEditorCards();
    }

    public void AutoSetEditorCards()
    {   //按照筛选条件进行筛选
        SetEditorCardInfo
        (
            //
            _cards
            .Where(x => ((_nowEditorDeck.Id == "blacklist" && x.Group == Group.Gold) || (_nowEditorDeck.Id != "blacklist" && (!(isSpecial && SpecialBanningList.Contains(x.CardInfo().CardId)) && ((x.Faction == Faction.Neutral) || (x.Faction == _nowSwitchFaction))))))
            .Where(x => ((_editorSearchMessage == "") ? true :
                (_translator.GetCardName(x.CardInfo().CardId).Contains(_editorSearchMessage, StringComparison.OrdinalIgnoreCase) ||
                 _translator.GetCardInfo(x.CardInfo().CardId).Contains(_editorSearchMessage, StringComparison.OrdinalIgnoreCase) ||
                 x.CardInfo().Strength.ToString().Contains(_editorSearchMessage) ||
                 x.Categories.Select(tag => _translator.GetText($"CardTag_{GwentMap.CategorieInfoMap[tag]}")).Any(text => text.Contains(_editorSearchMessage, StringComparison.OrdinalIgnoreCase))
                )))
            .Where(x => _nowEditorGroup == Group.Leader ? x.Group != Group.Leader : x.Group == _nowEditorGroup)
            .ToList()
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
        deck.Deck.Select(x => GwentMap.CardMap[x])
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
        AllCount.text = _nowEditorDeck.Deck.Count().ToString();
        bool valid = deck.Id == "blacklist" || (deck.IsSpecialDeck() || deck.IsBasicDeck());
        AllCount.color = valid ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        AllCountText.color = valid ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        if (_nowEditorDeck.Id == "blacklist")
        {
            CopperCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Copper).Count()}";
            GoldCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Gold).Count()}";
            SilverCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Silver).Count()}";
        }
        else
        {
            CopperCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Copper).Count()}";
            if (isSpecial)
                GoldCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Gold).Count()}/12";
            else
                GoldCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Gold).Count()}/4";
            SilverCount.text = $"{_nowEditorDeck.Deck.Where(x => GwentMap.CardMap[x].Group == Group.Silver).Count()}/6";
        }
        //*****************
        //等待补充？？？
        //*****************
        var count = deck.Deck.Distinct().Count();
        var height = ((10 + 75 + 2.6f + (41.5f + 2.6f) * count) + 5f);
        EditorCListContext.sizeDelta = new Vector2(0, height);
        //EditorCListScroll.value = 1;
    }


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
        await _globalUIService.YNMessageBox("Copied! 复制成功", "Code: " + deckCode, "PopupWindow_OkButton", isOnlyYes: true);
        Debug.Log(_nowEditorDeck.Name);
        GUIUtility.systemCopyBuffer = deckCode;
    }
    public async Task<bool> AddDeckFromCode(string deckCode, string name = "Default")
    {
        Debug.Log(deckCode);
        var deck = deckCode.DeCompressToDeck();
        deck.Name = string.IsNullOrWhiteSpace(name) ? _translator.GetCardName(deck.Leader.CardInfo().CardId) + " #" + (_clientService.User.Decks.Count + 1)
                                                    : (name.Length >= 20 ? name.Substring(0, 20) : name);
        if (!(deck.IsBasicDeck() || deck.IsSpecialDeck() || deck.IsHalfBasicDeck()))
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
            await _globalUIService.YNMessageBox("Added! 添加成功", "Code: " + deckCode, "PopupWindow_OkButton", isOnlyYes: true);
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
