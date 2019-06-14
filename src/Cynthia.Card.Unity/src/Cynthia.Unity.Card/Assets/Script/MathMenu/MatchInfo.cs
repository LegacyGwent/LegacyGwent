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
    public Image HeadT;
    public Image HeadB;
    public Sprite[] HeadTSprite;
    public Sprite[] HeadBSprite;

    public Text DeckName;
    public Transform DeckNameBackground;
    public Transform DeckIcon;
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
    void Start()
    {
        ResetMatch();
        IsDoingMatch = false;
        //_client = DependencyResolver.Container.Resolve<GwentClientService>();
        //_UIService = DependencyResolver.Container.Resolve<GlobalUIService>();
    }
    public void ResetMatch()
    {
        if (_client.User.Decks.Count() <= GlobalState.DefaultDeckIndex) GlobalState.DefaultDeckIndex = 0;
        SetDeck(_client.User.Decks[GlobalState.DefaultDeckIndex], _client.User.Decks[GlobalState.DefaultDeckIndex].Id);
        SetDeckList(_client.User.Decks);
    }
    public void Match()/////待编辑
    {
        ReturnButton.SetActive(false);
        SwitchButton.SetActive(false);
        MatchMessage.text = "寻找对手中";
        MatchButtonText.text = "停止匹配";
    }
    public void StopMatch()/////待编辑
    {
        ReturnButton.SetActive(true);
        SwitchButton.SetActive(true);
        MatchMessage.text = "牌组就绪";
        MatchButtonText.text = "开始战斗";
    }
    public async void MatchButtonClick()/////待编辑
    {
        if (IsDoingMatch)
        {
            await _client.StopMatch();
            return;
        }
        //开始匹配
        if (!await _client.Match(CurrentDeckId))
        {
            Debug.Log("发送未知错误,匹配失败");
        }
        IsDoingMatch = true;
        Match();
        if (await _client.MatchResult())
        {
            Debug.Log("成功匹配,进入游戏");
            GlobalState.IsToMatch = false;
            SceneManager.LoadScene("GamePlay");
            return;
        }
        Debug.Log("成功停止匹配");
        IsDoingMatch = false;
        StopMatch();
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
        DeckIcon.GetComponent<Image>().sprite = FactionIcon[GetFactionIndex(GwentMap.CardMap[deck.Leader].Faction)];
        //////////////////////////////////////////////////
        var leader = Instantiate(LaderPrefab);
        leader.GetComponent<LeaderShow>().SetLeader(deck.Leader);
        leader.transform.SetParent(CardsContext, false);
        var cards = deck.Deck.Select(x => GwentMap.CardMap[x]);
        cards.OrderByDescending(x => x.Group).ThenByDescending(x => x.Strength).GroupBy(x => x.Name).ForAll(x =>
            {
                var card = Instantiate(CardPrefab);
                card.GetComponent<ListCardShowInfo>().SetCardInfo(x.First().Strength, x.Key, x.Count(), x.First().Group);
                card.transform.SetParent(CardsContext, false);
            });
        CopperCount.text = cards.Where(x => x.Group == Group.Copper).Count().ToString();
        SilverCount.text = $"{cards.Where(x => x.Group == Group.Silver).Count().ToString()}/6";
        GoldCount.text = $"{cards.Where(x => x.Group == Group.Gold).Count().ToString()}/4";
        AllCount.text = $"{deck.Deck.Count()}";
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

