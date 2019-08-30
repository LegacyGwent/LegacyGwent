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

public class MatchInfo : MonoBehaviour
{
    public ArtCard ShowArtCard;
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
    //-------------------------------------------
    public GameObject MainUI;
    public GameObject MatchUI;
    public MainMenuEffect[] ResetTextMenus;
    //-------------------------------------------
    public string CurrentDeckId { get; private set; }
    public bool IsDoingMatch { get; private set; }

    private GwentClientService _client { get => DependencyResolver.Container.Resolve<GwentClientService>(); }
    private GlobalUIService _UIService { get => DependencyResolver.Container.Resolve<GlobalUIService>(); }

    public void MatchMenuClick()
    {
        if (_client.User.Decks.Count() <= 0)
        {
            _UIService.YNMessageBox("当前没有卡组", "当前没有卡组,请添加卡组后再进行匹配");
        }
        else
        {
            ResetMatch();
            MainUI.SetActive(false);
            MatchUI.SetActive(true);
            ResetTextMenus.ForAll(x => x.TextReset());
        }
    }
    public void ResetMatch()
    {
        if (!_client.User.Decks.Any(x => x.Id == GlobalState.DefaultDeckId)) GlobalState.DefaultDeckId = _client.User.Decks.First().Id;
        SetDeck(_client.User.Decks.Single(x => x.Id == GlobalState.DefaultDeckId), GlobalState.DefaultDeckId);
        SetDeckList(_client.User.Decks);
    }
    public void ShowMatch()/////待编辑
    {
        ReturnButton.SetActive(false);
        SwitchButton.SetActive(false);
        MatchMessage.text = "寻找对手中";
        MatchButtonText.text = "停止匹配";
        MatchPassword.readOnly = true;
    }
    public void ShowStopMatch()/////待编辑
    {
        ReturnButton.SetActive(true);
        SwitchButton.SetActive(true);
        MatchMessage.text = "牌组就绪";
        MatchButtonText.text = "开始战斗";
        MatchPassword.readOnly = false;
    }
    public async void MatchButtonClick()/////点击匹配按钮的话
    {
        //如果正在进行匹配
        if (IsDoingMatch)
        {
            //停止匹配
            await _client.StopMatch();
            return;
        }
        if (!_client.User.Decks.Single(x => x.Id == CurrentDeckId).IsBasicDeck())
        {
            await _UIService.YNMessageBox("该卡组无法用于匹配", "该卡组无法用于该匹配,请重新编辑或切换卡组。");
            return;
        }
        //否则尝试开始匹配(目前不关注匹配结果)
        _ = _client.MatchOfPassword(CurrentDeckId, MatchPassword.text);

        // else if (!await _client.Match(CurrentDeckId))
        // {
        //     //如果发生错误的话,匹配失败
        //     Debug.Log("发送未知错误,匹配失败");
        // }

        //将状态和显示都变成正在匹配
        IsDoingMatch = true;
        ShowMatch();

        //等待匹配的结果,如果是true代表成功匹配
        if (await _client.MatchResult())
        {
            //进入了游戏
            Debug.Log("成功匹配,进入游戏");
            GlobalState.IsToMatch = false;
            SceneManager.LoadScene("GamePlay");
            return;
        }
        else
        {
            //否则代表取消了匹配
            Debug.Log("成功停止匹配");
            IsDoingMatch = false;
            ShowStopMatch();
        }
    }
    public void SwitchDeckOpen()
    {
        ReturnButton.SetActive(false);
        SwitchButton.SetActive(false);
        MatchButton.SetActive(false);
        CloseButton.SetActive(true);
        MatchMessage.text = "选择牌组";
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
        MatchMessage.text = "牌组就绪!";
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
        ResetMatch();
        IsDoingMatch = false;
    }
    public void SetDeckList(IList<DeckModel> decks)
    {
        var count = DecksContext.childCount;
        for (var i = count - 1; i >= 0; i--)
        {
            Destroy(DecksContext.GetChild(i).gameObject);
        }
        DecksContext.DetachChildren();
        decks.ForAll(x =>
        {
            var deck = Instantiate(DeckPrefabs[GetFactionIndex(GwentMap.CardMap[x.Leader].Faction)]);
            deck.GetComponent<DeckShowInfo>().SetDeckInfo(x.Name, x.IsBasicDeck());
            deck.GetComponent<SwitchMatchDeck>().SetId(DecksContext.childCount);
            deck.transform.SetParent(DecksContext, false);
        });
        var height = decks.Count * 83 + 35;
        DecksContext.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(370, height > 800 ? height : 800);
    }

    public void SetMatchArtCard(CardStatus card, bool isOver = true)
    {

        ShowArtCard.CurrentCore = card;
        ShowArtCard.gameObject.SetActive(isOver);
    }

    public void SetDeck(DeckModel deck, string id)
    {
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
        DeckIcon.sprite = FactionIcon[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
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
        AllCount.color = deck.IsBasicDeck() ? GlobalState.NormalColor : GlobalState.ErrorColor;
        AllCountText.color = deck.IsBasicDeck() ? GlobalState.NormalColor : GlobalState.ErrorColor;
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
        GlobalState.IsToMatch = false;
    }
}

