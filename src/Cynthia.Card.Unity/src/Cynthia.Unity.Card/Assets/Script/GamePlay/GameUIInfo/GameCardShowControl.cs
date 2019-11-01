using Alsein.Extensions;
using Alsein.Extensions.IO;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameCardShowControl : MonoBehaviour
{
    public GameObject UICardPrefab;
    public GameObject NullCardPrefab;
    public RectTransform CardsContent;
    public Text ShowCardMessage;
    public Scrollbar Scroll;
    public GameObject CardSelectUI;
    public GameEvent GameEvent;

    public ArtCard ArtCard;
    //-------
    public GameObject OpenButton;//显示卡牌
    public GameObject MulliganEndButton;
    public GameObject CloseButton;
    public GameObject AffirmButton;
    public GameObject HideButton;
    //---------------------------------
    public int NowMulliganCount { get; set; }
    public int NowMulliganTotal { get; set; }
    //------
    public int NowSelectTotal { get; set; }
    public IList<int> NowSelect { get; set; } = new List<int>();
    //---------------------------------
    private (bool, bool, bool, bool) UseButtonShow { get; set; }
    private bool IsUseMenuShow { get; set; }
    private string useCardTitle { get; set; }
    private UseCardShowType _nowUseMenuType;

    private MenuShowType _nowShowType = MenuShowType.None;
    //---------------------------------
    public IList<CardStatus> UseCardList = new List<CardStatus>();
    public IList<CardStatus> MyCemetery = new List<CardStatus>();
    public IList<CardStatus> MyDeck = new List<CardStatus>();
    public IList<CardStatus> EnemyCemetery = new List<CardStatus>();
    private int _nowIndex;
    //
    private ITubeInlet sender;
    private ITubeOutlet receiver;
    private void Awake() => (sender, receiver) = Tube.CreateSimplex();
    private bool IsAutoPlay { get => DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay; }
    //------------------------------------------------------------------------------------------
    public void OpenButtonClick()//显示卡牌
    {
        OpenNowUseMenu();
    }
    public async void MulliganEndButtonClick()//手牌调度完毕的按钮被点击
    {
        if (_nowUseMenuType != UseCardShowType.Mulligan)
        {
            return;
        }
        await sender.SendAsync<int>(-1);
    }
    public void CloseButtonClick()//关闭
    {
        CardSelectUI.SetActive(false);
    }
    public async void AffirmButtonClick()//确认
    {
        if (_nowUseMenuType != UseCardShowType.Select)
        {
            return;
        }
        await sender.SendAsync(NowSelect);
    }
    public void HideButtonClick()//隐藏卡牌
    {
        IsUseMenuShow = false;
        CardSelectUI.SetActive(false);
    }
    //------------------------------------------------------------------------------------------
    public void SelectCard(int index)
    {
        //光标移动到某张卡牌上
        _nowIndex = index;
        if (index == -1)
        {
            //光标离开某张卡牌
            ArtCard.gameObject.SetActive(false);
        }
        else
        {
            if (_nowShowType == MenuShowType.UseCard)
            {
                ArtCard.CurrentCore = UseCardList[index];
            }
            else if (_nowShowType == MenuShowType.EnemyCemetery)
            {
                ArtCard.CurrentCore = EnemyCemetery[index];
            }
            else if (_nowShowType == MenuShowType.MyCemetery)
            {
                ArtCard.CurrentCore = MyCemetery[index];
            }
            else if (_nowShowType == MenuShowType.MyDeck)
            {
                ArtCard.CurrentCore = MyDeck[index];
            }
            ArtCard.gameObject.SetActive(true);
        }
    }
    public async void ClickCard(int index)
    {
        //点击了卡牌
        switch (_nowUseMenuType)
        {
            case UseCardShowType.Mulligan:
                if (IsUseMenuShow)
                    await sender.SendAsync<int>(index);
                break;
            //如果在选卡
            case UseCardShowType.Select:
                //如果目前展示的并非选卡页面,返回
                if (!IsUseMenuShow || NowSelect.Count >= NowSelectTotal)
                    break;
                //获取到点击的这张卡
                var card = CardsContent.transform.GetChild(index).GetComponent<SelectUICard>();
                //如果这张卡是已选中状态,则将他设定为未选中
                if (card.IsSelect)
                {
                    card.IsSelect = false;
                    //从下标列表中删除该项
                    var i = NowSelect.IndexOf(index);
                    NowSelect.RemoveAt(i);
                }
                else
                {
                    card.IsSelect = true;
                    NowSelect.Add(index);
                    if (NowSelect.Count >= NowSelectTotal)
                    {
                        await sender.SendAsync(NowSelect);
                    }
                }
                break;
            case UseCardShowType.None:
                break;
            default:
                break;
        }
    }

    public void ShowMyCemetery()
    {
        if (MyCemetery == null || MyCemetery.Count == 0)
            return;
        ShowCardMessage.text = "我方墓地";
        _nowShowType = MenuShowType.MyCemetery;
        SetCardInfo(MyCemetery);
        SelectCard(-1);
        CardSelectUI.SetActive(true);
        SetButtonShow(IsCloseShow: true);
        IsUseMenuShow = false;
    }
    public void ShowEnemyCemetery()
    {
        if (EnemyCemetery == null || EnemyCemetery.Count == 0)
            return;
        ShowCardMessage.text = "敌方墓地";
        _nowShowType = MenuShowType.EnemyCemetery;
        SetCardInfo(EnemyCemetery);
        SelectCard(-1);
        CardSelectUI.SetActive(true);
        SetButtonShow(IsCloseShow: true);
        IsUseMenuShow = false;
    }

    public void ShowMyDeck()
    {
        Debug.Log(MyDeck?.Count());
        if (MyDeck == null || MyDeck.Count() == 0)
            return;
        ShowCardMessage.text = "我方卡组(原始状态,顺序)";
        _nowShowType = MenuShowType.MyDeck;
        SetCardInfo(MyDeck);
        SelectCard(-1);
        CardSelectUI.SetActive(true);
        SetButtonShow(IsCloseShow: true);
        IsUseMenuShow = false;
    }
    //------------------------------------------------------------------------------------------------
    //调度开始
    public void MulliganStart(IList<CardStatus> cards, int total)//调度界面
    {

        NowMulliganCount = 0;
        NowMulliganTotal = total;
        useCardTitle = $"选择1张卡重抽。[{NowMulliganCount}/{NowMulliganTotal}]";
        UseCardList = cards;
        OpenButton.SetActive(true);//打开显示按钮
                                   //IsMulliganEndShow,IsCloseShow,IsAffirmShow,IsHideShow
        UseButtonShow = (true, false, false, true);
        OpenNowUseMenu();
    }
    //调度结束
    public void OperationEnd()
    {
        _nowUseMenuType = UseCardShowType.None;
        useCardTitle = "错误";
        NowSelect = new List<int>();
        UseCardList = new List<CardStatus>();
        OpenButton.SetActive(false);//打开
        CardSelectUI.SetActive(false);
    }
    //更新信息(需要更改),动画之类的
    public void MulliganData(int index, CardStatus card)
    {
        UseCardList[index] = card;
        var mCard = CardsContent.GetChild(index).GetComponent<CardShowInfo>();
        mCard.CurrentCore = card;
        //mCard.SetCard();
        //--------------------------
        mCard = GameEvent.MyHand.transform.GetChild(index).GetComponent<CardShowInfo>();
        mCard.CurrentCore = card;
        SelectCard(_nowIndex);
        //mCard.SetCard();
    }
    //获取调度信息
    public async Task GetMulliganInfo(LocalPlayer player)
    {
        _nowUseMenuType = UseCardShowType.Mulligan;
        if (IsAutoPlay) await sender.SendAsync<int>(-1);///////////自动调度22222222222222
        var result = await receiver.ReceiveAsync<int>();
        _nowUseMenuType = UseCardShowType.None;
        if (result != -1)
            NowMulliganCount++;
        useCardTitle = $"选择1张卡重抽。[{NowMulliganCount}/{NowMulliganTotal}]";
        if (IsUseMenuShow)
            ShowCardMessage.text = useCardTitle;
        //Debug.Log("发送调度消息");
        if (result == -1)
        {
            OperationEnd();
        }
        await player.SendAsync(UserOperationType.MulliganInfo, result);
    }
    //-----------------------------------------
    public void OpenNowUseMenu()
    {
        //将存起来的标题和卡牌赋值
        ShowCardMessage.text = useCardTitle;
        _nowShowType = MenuShowType.UseCard;
        SetCardInfo(UseCardList);
        SetButtonShow(UseButtonShow);
        IsUseMenuShow = true;
        SelectCard(-1);
        CardSelectUI.SetActive(true);
    }
    //------------------------------------------------------------------------------------------------
    //选择卡牌
    public async Task SelectMenuCards(MenuSelectCardInfo info, LocalPlayer player)
    {
        if (IsAutoPlay) await sender.SendAsync<IList<int>>///////////自动选卡22222222222222
          (0.To(info.SelectCount - 1).Mess().Take(info.SelectCount).ToList());
        else
        {
            //这里是正常的逻辑
            //设置标题
            useCardTitle = info.Title;
            //设置卡组列表
            UseCardList = info.SelectList;
            OpenButton.SetActive(true);//打开显示按钮
            //IsMulliganEndShow,IsCloseShow,IsAffirmShow,IsHideShow
            UseButtonShow = (false, false, info.IsCanOver, true);
            //现在选择卡牌的列表
            NowSelectTotal = info.SelectCount;
            NowSelect = new List<int>();//清空
            OpenNowUseMenu();
            _nowUseMenuType = UseCardShowType.Select;
        }
        var result = await receiver.ReceiveAsync<IList<int>>();
        OperationEnd();
        await player.SendAsync(UserOperationType.SelectMenuCardsInfo, result);
        //NowUseMenuType = UseCardShowType.None;
        //NowSelect = new List<int>();//清空
    }
    //------------------------------------------------------------------------------------------------
    public void SetCardInfo(IList<CardStatus> cards)
    {
        var count = cards.Count;
        RemoveAllChild();
        for (var i = 0; i < count; i++)
        {
            var card = Instantiate(UICardPrefab).GetComponent<CardShowInfo>();
            card.CurrentCore = cards[i];
            //card.SetCard();
            card.transform.SetParent(CardsContent, false);
        }
        var nullcount = count <= 10 ? 10 - count : (count % 5 == 0 ? 0 : 5 - count % 5);
        for (var i = 0; i < nullcount; i++)
        {
            var card = Instantiate(NullCardPrefab);
            card.transform.SetParent(CardsContent, false);
        }
        //------------------------------------------------------------------------
        var height = count <= 10 ? 780f : (108f + 276 * (count % 5 > 0 ? count / 5 + 1 : count / 5));
        CardsContent.sizeDelta = new Vector2(0, height);
        if (count <= 10)
            CardsContent.GetComponent<GridLayoutGroup>().padding.top = 190;
        else
            CardsContent.GetComponent<GridLayoutGroup>().padding.top = 130;
        Scroll.value = 1;
        if (_nowShowType == MenuShowType.UseCard && NowSelect.Any())
        {
            NowSelect.ForAll(x =>
            {
                CardsContent.transform.GetChild(x).GetComponent<SelectUICard>().IsSelect = true;
            });
        }
    }
    public void SetButtonShow(bool IsMulliganEndShow = false, bool IsCloseShow = false, bool IsAffirmShow = false, bool IsHideShow = false)
    {
        MulliganEndButton.SetActive(IsMulliganEndShow);
        CloseButton.SetActive(IsCloseShow);
        AffirmButton.SetActive(IsAffirmShow);
        HideButton.SetActive(IsHideShow);
    }
    public void SetButtonShow((bool, bool, bool, bool) ButtonShow)
    {
        //调度结束,关闭,确认,隐藏
        //展示墓地固定[关闭]
        var (IsMulliganEndShow, IsCloseShow, IsAffirmShow, IsHideShow) = ButtonShow;
        MulliganEndButton.SetActive(IsMulliganEndShow);
        CloseButton.SetActive(IsCloseShow);
        AffirmButton.SetActive(IsAffirmShow);
        HideButton.SetActive(IsHideShow);
    }
    public void RemoveAllChild()
    {
        for (var i = CardsContent.childCount - 1; i >= 0; i--)
        {
            Destroy(CardsContent.GetChild(i).gameObject);
        }
        CardsContent.DetachChildren();
    }
}
