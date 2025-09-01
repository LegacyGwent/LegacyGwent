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


    public void MatchMenuClick()
    {
        if (_client.User.Decks.Count() <= 0)
        {
            _UIService.YNMessageBox(_translator.GetText("PopupWindow_NoDeckTitle"),
                _translator.GetText("PopupWindow_NoDeckDesc"), isOnlyYes: true);
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
    }
    public void NormalMatchMenuClick()
    {
        IsRankMatch = false;
        MatchMenuClick();
    }
    public void RankMatchMenuClick()
    {
        IsRankMatch = true;
        // MatchMenuClick();
        ResetMatch();
        MainUI.SetActive(false);
        MatchUI.SetActive(true);
        Debug.Log("1");
        // ResetTextMenus.ForAll(x => x.TextReset());
        MainMenu_MatchTitle.text = _translator.GetText("MainMenu_MatchTitle_Rank");
        Debug.Log(_translator.GetText("MainMenu_MatchTitle_Rank"));
        MatchPasswordObject.SetActive(false);
        BlacklistObject.SetActive(false);
        Debug.Log("3");
        Debug.Log(MainMenu_MatchTitle.text);

    }
    public void ResetMatch()
    {
        Debug.Log("重置");
        if (!_client.User.Decks.Any(x => x.Id == ClientGlobalInfo.DefaultDeckId))
        {
            ClientGlobalInfo.DefaultDeckId = _client.User.Decks.First().Id;
        }
        SetDeck(_client.User.Decks.Single(x => x.Id == ClientGlobalInfo.DefaultDeckId), ClientGlobalInfo.DefaultDeckId);
        SetDeckList(_client.User.Decks);
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
            //如果是基础卡组（包括店店卡组）
            if (_client.User.Decks.Single(x => x.Id == CurrentDeckId).IsBasicDeck())
            {
                var password = IsRankMatch ? "rank" : (MatchPassword.text).Replace("special", "");
                var setBlacklist = IsRankMatch ? 0 : usingBlacklist;
                _ = _client.NewMatchOfPassword(CurrentDeckId, password, setBlacklist);
            }
            //如果不是基础卡组和乱斗卡组，停止匹配
            else if (!_client.User.Decks.Single(x => x.Id == CurrentDeckId).IsSpecialDeck())
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
        catch
        {
            SceneManager.LoadScene("LoginScene");
            _client.ClientState = ClientState.Standby;
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
    public void SetDeckList(IList<DeckModel> decks)
    {
        var count = DecksContext.childCount;
        // Debug.Log($"数量为:{count}");
        for (var i = count - 1; i >= 0; i--)
        {
            // Debug.Log($"消除{i}");
            Destroy(DecksContext.GetChild(i).gameObject);
        }
        // Debug.Log($"完成消除,脱离");
        DecksContext.DetachChildren();
        // Debug.Log("完成脱离,生成新实例");
        Debug.Log(decks.Select(x => x.Name).Join(","));
        decks.ForAll(x =>
        {
            var deck = Instantiate(DeckPrefabs[GetFactionIndex(GwentMap.CardMap[x.Leader].Faction)]);
            string leaderartid = GwentMap.CardMap[x.Leader].CardArtsId;
            deck.GetComponent<DeckShowInfo>().SetDeckInfo(x.Name, x.IsBasicDeck() || x.IsSpecialDeck());
            deck.GetComponent<DeckEditorMiniatures>().SetMiniatureArt(leaderartid);
            deck.GetComponent<SwitchMatchDeck>().SetId(DecksContext.childCount);
            deck.transform.SetParent(DecksContext, false);
        });
        // Debug.Log("生成完毕,设置高");
        var height = decks.Count * 83 + 35;
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
        var cards = deck.Deck.Select(x => GwentMap.CardMap[x]);
        cards.OrderByDescending(x => x.Group).ThenByDescending(x => x.Strength).GroupBy(x => x.Name).ForAll(x =>
            {
                var card = Instantiate(CardPrefab);
                card.GetComponent<ListCardShowInfo>().SetCardInfo(x.First().CardId, x.Count());
                card.transform.SetParent(CardsContext, false);
            });
        CopperCount.text = cards.Where(x => x.Group == Group.Copper).Count().ToString();
        SilverCount.text = $"{cards.Where(x => x.Group == Group.Silver).Count().ToString()}/6";
        GoldCount.text = $"{cards.Where(x => x.Group == Group.Gold).Count().ToString()}/4";
        AllCount.text = $"{deck.Deck.Count()}";
        AllCount.color = (deck.IsBasicDeck() || deck.IsSpecialDeck() || (deck.IsBlacklist() && deck.Id == "blacklist")) ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        AllCountText.color = (deck.IsBasicDeck() || deck.IsSpecialDeck() || (deck.IsBlacklist() && deck.Id == "blacklist")) ? ClientGlobalInfo.NormalColor : ClientGlobalInfo.ErrorColor;
        HeadT.sprite = HeadTSprite[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
        HeadB.sprite = HeadBSprite[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
        //////////////////////////////////////////////////
        var height = ((41.5f + 3f) * CardsContext.childCount) + 8f + 38f;
        CardsContext.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, height);
    }
    public int GetFactionIndex(Faction faction)
    {
        return FactionIndex.Indexed().Single(x => x.Value == faction).Key;
    }
    public void ReturnButtonClick()
    {
        ClientGlobalInfo.IsToMatch = false;
    }
}
