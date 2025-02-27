using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card.Client;
using Autofac;
using Cynthia.Card;
using System.Linq;
using Alsein.Extensions;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Script.Localization;

public class GameEvent : MonoBehaviour
{
    private LocalizationService translator;
    static public bool RighClickActive;
    public static string RightClickedCardID;
    public ArtCard ShowCard;
    public float ArrowsZ = -6f;
    //可被拖上(6排,以及我方墓地)
    public CardsPosition[] AllCardsPosition;
    public CanDrop[] AllCanDrop;
    public CanDrop EnemyRow1;
    public CanDrop EnemyRow2;
    public CanDrop EnemyRow3;
    public CanDrop MyRow1;
    public CanDrop MyRow2;
    public CanDrop MyRow3;
    public CanDrop MyPlance;
    public CanDrop EnemyPlance;
    public CanDrop MyCemetery;//我方墓地可以被拖上
    public ShowWeather ShowWeather;

    public GameCardShowControl GameCardShowControl;
    //三个定位点
    public Transform EnemyCemetery;
    public Transform MyDeck;
    public Transform EnemyDeck;

    public CardsPosition MyHand;
    public CardsPosition EnemyHand;

    public CardsPosition MyStayCards;
    public CardsPosition EnemyStayCards;

    public PassCoin PassCoin;
    public LeaderCard MyLeader;
    public LeaderCard EnemyLeader;

    public PassCoin Coin;
    //-----------------------------
    //<><><><><><><><><><><><><><><
    private Transform _arrowSource;
    private Transform _arrowTaget;
    public TempArrows TempArrows;
    //状态(类型)
    public GameOperationType NowOperationType { get; set; }//当前的√(有效
    //(选卡)
    public PlaceSelectCardsInfo SelectPlaceCardsInfo;//一些选择的信息√(有效
    public IList<CardLocation> NowSelectCards;//当前选择的卡牌
    //(选排)
    public IList<RowPosition> CanSelectRow;//可以被选中的排√(有效
    //放置信息
    public CardStatus CurrentPlayCard;//当前放置的卡牌
    public static CanDrop DropTaget;//一个目前模式可以选中的,鼠标悬停上去的排
    private CardShowInfo _selectModeCard;//选择模式中的焦距
    private CardMoveInfo _selectCard;//共有焦距效果
    private CardMoveInfo _dragCard;
    //当前处理的卡牌
    private CardMoveInfo _myUseCard;
    private CardMoveInfo _enemyUseCard;
    //是否鼠标停留在硬币上
    private bool IsSelectCoin;
    private bool IsOnCoin;
    private bool IsAutoPlay { get => DependencyResolver.Container.Resolve<GwentClientService>().IsAutoPlay; }
    //<><><><><><><><><><><><><><><
    //-----------------------------
    public GameObject CardPrefab;
    public GameObject NumberPrefab;
    //管道...
    private ITubeInlet sender;
    private ITubeOutlet receiver;
    public GameObject ShowMyCemeteryButton;
    public GameObject SurrenderButton;

    private GlobalUIService _uiService;
    public RopeController ropeController;
    public bool IsMobileClickDown = false;
    private float pressTime = 0; // time pressed on mobile
    private bool IsRightClickMobile = false;
    private void Awake()
    {
        (sender, receiver) = Tube.CreateSimplex();
        _uiService = DependencyResolver.Container.Resolve<GlobalUIService>();
    }
    //状态信息
    //最开始
    private void Start()
    {
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
        RighClickActive = false;
        NowOperationType = GameOperationType.None;

#if UNITY_ANDROID || UNITY_IOS
        ShowMyCemeteryButton.SetActive(true);
        SurrenderButton.SetActive(true);
#else
        ShowMyCemeteryButton.SetActive(false);
        SurrenderButton.SetActive(false);
#endif
        //某些信息,目前只是用来测试
        // var sc = GetCard(new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = 0 }).CardShowInfo.CurrentCore = new CardStatus("11210200");
        //sc.IsCountdown = true;
        //sc.Armor = 5;
        //GetCard(new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = 0 }).CardShowInfo.SetCard();
    }
    //设置当前场地显示
    private CardUseInfo _currentPlace;
    public CardUseInfo CurrentPlace
    {
        get => _currentPlace;
        set
        {
            //if (_currentPlace == value) return;
            _currentPlace = value;
            EnemyRow1.IsCanDrop = false;
            EnemyRow2.IsCanDrop = false;
            EnemyRow3.IsCanDrop = false;
            EnemyPlance.IsCanDrop = false;
            MyRow1.IsCanDrop = false;
            MyRow2.IsCanDrop = false;
            MyRow3.IsCanDrop = false;
            MyPlance.IsCanDrop = false;
            MyCemetery.IsCanDrop = false;
            switch (_currentPlace)
            {
                case CardUseInfo.ReSet:
                    return;
                case CardUseInfo.MyPlace:
                    MyPlance.IsCanDrop = true;
                    break;
                case CardUseInfo.EnemyPlace:
                    EnemyPlance.IsCanDrop = true;
                    break;
                case CardUseInfo.AnyPlace:
                    EnemyPlance.IsCanDrop = true;
                    MyPlance.IsCanDrop = true;
                    break;
                case CardUseInfo.MyRow:
                    MyRow1.IsCanDrop = true;
                    MyRow2.IsCanDrop = true;
                    MyRow3.IsCanDrop = true;
                    break;
                case CardUseInfo.EnemyRow:
                    EnemyRow1.IsCanDrop = true;
                    EnemyRow2.IsCanDrop = true;
                    EnemyRow3.IsCanDrop = true;
                    break;
                case CardUseInfo.AnyRow:
                    EnemyRow1.IsCanDrop = true;
                    EnemyRow2.IsCanDrop = true;
                    EnemyRow3.IsCanDrop = true;
                    MyRow1.IsCanDrop = true;
                    MyRow2.IsCanDrop = true;
                    MyRow3.IsCanDrop = true;
                    break;
            }
            MyCemetery.IsCanDrop = true;
        }

    }
    //当前正在拖拽的卡牌
    public CardMoveInfo DragCard
    {
        get => _dragCard;
        set
        {
            if (_dragCard == value) return;
            if (!RighClickActive)
            {
                if (_dragCard != null)//放弃当前所拖
                {
                    _dragCard.IsDrag = false;
                    _dragCard.ZPosition += 2f;
                }
                _dragCard = value;
                if (value == null) return;
                _dragCard.IsDrag = true;
                _dragCard.ZPosition -= 2f;
            }
        }
    }
    //当前选择
    public CardMoveInfo SelectCard
    {
        get => _selectCard;
        set
        {
            if (_selectCard == value) return;
            if (_selectCard != null)
            {
                //将之前选中的复原
                //_selectCard.transform.localScale = new Vector3(1, 1, 1);
                _selectCard.CardShowInfo.ScaleTo(1);
                _selectCard.ZPosition += 1f;//此复原有问题
            }
            _selectCard = value;
            if (value == null)
            {
                ShowCard.gameObject.SetActive(false);
                return;
            }
            if (!_selectCard.IsTem)
            {
                ShowCard.CurrentCore = _selectCard.CardShowInfo.CurrentCore;
                ShowCard.gameObject.SetActive(true);
            }
            if (!_selectCard.IsCanSelect || _selectCard.IsOn || _selectCard.IsStay || _selectCard.CardShowInfo.IsDead || _selectCard.IsTem)
            {
                _selectCard = null;
                return;
            }
            //_selectCard.transform.localScale = new Vector3(1.05f, 1.05f, 1);
            _selectCard.CardShowInfo.ScaleTo(1.1f);
            _selectCard.ZPosition -= 1f;
        }
    }
    public CardShowInfo SelectModeCard
    {
        get => _selectModeCard;
        set
        {
            //if (_selectModeCard == value) return;
            if (_selectModeCard != null)//如果旧不是空,收尾
            {
                var afterLocation = GetLocation(_selectModeCard.transform);
                //如果已经被选中,移动开不会造成影响
                if (NowSelectCards == null || NowSelectCards.Where(x => afterLocation.CardIndex == x.CardIndex && afterLocation.RowPosition == x.RowPosition).Count() == 0)
                    _selectModeCard.SetSelect(false, false);//之后还需要更多判断,已经选中的不取消,以及范围
                if (SelectPlaceCardsInfo.Range > 0)
                {
                    GetRangeCards(GetLocation(_selectModeCard.transform), SelectPlaceCardsInfo.Range).ForAll(x =>
                     {
                         var l = GetLocation(x.transform);
                         if (NowSelectCards == null || NowSelectCards.Where(item => l.CardIndex == item.CardIndex && l.RowPosition == item.RowPosition).Count() == 0)
                             x.SetSelect(false, false);
                     });
                }
            }
            _selectModeCard = value;
            if (_selectModeCard == null) return;
            var location = GetLocation(_selectModeCard.transform);//获得坐标
            var isLight = NowSelectCards == null || NowSelectCards.Where(x => location.CardIndex == x.CardIndex && location.RowPosition == x.RowPosition).Count() == 0;
            var isHave = GetPartRow(SelectPlaceCardsInfo.CanSelect, location.RowPosition).Where(x => x == location.CardIndex).Count() != 0;
            if (isHave)//如果是合法目标
            {
                _selectModeCard.SetSelect(true, true, isLight);
                if (SelectPlaceCardsInfo.Range > 0)
                {
                    GetRangeCards(GetLocation(_selectModeCard.transform), SelectPlaceCardsInfo.Range).ForAll(x =>
                    {
                        var l = GetLocation(x.transform);
                        if (NowSelectCards == null || NowSelectCards.Where(item => l.CardIndex == item.CardIndex && l.RowPosition == item.RowPosition).Count() == 0)
                            x.SetSelect(true, false, isLight);
                    });
                }
            }
            else
            {
                _selectModeCard = null;
            }
        }
    }
    //---------------------------------------------------------------------------------
    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][
    //Unity事件
    //鼠标落下
    // #if UNITY_ANDROID
    //     private void ontouchstart()
    //     {
    //         DownEffect();
    //     }
    //     private void ontouchend()
    //     {
    //         UpEffect();
    //     }
    // #endif
    // #if UNITY_STANDALONE_WIN
    private void OnMouseDown()
    {
        if (!RighClickActive)
        {
            DownEffect();
        }
    }
    private async void OnMouseUp()
    {
        if (!RighClickActive)
        {
            await UpEffect();
        }
    }
    // #endif
    private void DownEffect()
    {
#if UNITY_ANDROID || UNITY_IOS
        IsMobileClickDown = true;
        Update();
        IsMobileClickDown = false;
#endif
        switch (NowOperationType)
        {
            case GameOperationType.GetPassOrGrag:
                //拖或者pass
                if (IsSelectCoin)//硬币判断
                {
                    IsSelectCoin = false;
                    IsOnCoin = true;//按住了！
                }
                //将选中的卡牌变为拖动状态,如果可以被拖动(并取消选中效果
                if (SelectCard == null || !SelectCard.IsCanDrag || SelectCard.IsStay) return;
                DragCard = SelectCard;
                CurrentPlace = DragCard.CardUseInfo;
                CloseAllRowMaxCanDrop();
                SelectCard = null;
                break;
            case GameOperationType.SelectCards:
                break;
            case GameOperationType.SelectRow:
                break;
            case GameOperationType.PlayCard:
                break;
            case GameOperationType.None:
                break;
            default:
                break;
        }
    }
    //鼠标抬起
    private async Task UpEffect()
    {
        switch (NowOperationType)
        {
            case GameOperationType.GetPassOrGrag:
                //拖或者pass
                CurrentPlace = CardUseInfo.ReSet;
                if (IsOnCoin)//此方法会被改到其他地方(按住)
                {   //直接pass信息并结束
                    await sender.SendAsync<RoundInfo>(new RoundInfo() { IsPass = true });
                    IsOnCoin = false;
                    IsSelectCoin = false;
                    return;
                }
                if (DragCard == null) return;//没有拖拽的话,就没有什么效果
                if (DropTaget != null)
                {
                    var dropTaget = DropTaget;
                    var dragCard = DragCard;
                    dragCard.IsStay = true;
                    if (dropTaget.Id == RowPosition.MyCemetery)
                    {
#if UNITY_ANDROID || UNITY_IOS
                        if (!await _uiService.YNMessageBox("确认弃牌?", "正在试图丢弃一张牌,是否确认?"))
                        {
                            dragCard.IsStay = false;
                            break;
                        }
#endif
                    }
                    //----------------------------------------------------------------------------------
                    //回应服务器的请求
                    if (GetIndex(dragCard.transform) == -1) MyLeader.IsCardCanUse = false;
                    await sender.SendAsync<RoundInfo>
                    (
                        new RoundInfo()
                        {
                            HandCardIndex = GetIndex(dragCard.transform),
                            CardLocation = new CardLocation()
                            {
                                RowPosition = dropTaget.Id,
                                CardIndex = GetDropIndex(GetRelativePosition(dropTaget), dropTaget.CardsPosition)
                            },
                            IsPass = false,
                        }
                    );
                    ResetAllTem();//
                    //-----------------------------------------------------------------------------------
                    DropTaget = null;
                }
                DragCard = null;
                break;
            case GameOperationType.SelectCards:
                if (SelectModeCard != null)
                {
                    var location = GetLocation(SelectModeCard.transform);
                    for (var i = NowSelectCards.Count - 1; i >= 0; i--)
                    {
                        var item = NowSelectCards[i];
                        if (item.CardIndex == location.CardIndex && item.RowPosition == location.RowPosition)
                        {//如果包含这个元素的话...
                            NowSelectCards.RemoveAt(i);//删除
                            return;
                        }
                    }
                    //如果不包含,添加,判断,是否发送
                    NowSelectCards.Add(location);
                    if (NowSelectCards.Count == SelectPlaceCardsInfo.SelectCount)
                    {//选中单位符合目标数量,发送
                        await sender.SendAsync<IList<CardLocation>>(NowSelectCards);
                    }
                }
                break;
            case GameOperationType.SelectRow:
                if (DropTaget != null)
                {
                    await sender.SendAsync<RowPosition>(DropTaget.Id);
                }
                break;
            case GameOperationType.PlayCard://松开之后,如果有目标发送未知
                if (DropTaget != null)
                {
                    //----------------------------------------------------------------------------------
                    //回应服务器的请求
                    await sender.SendAsync<CardLocation>
                    (
                        new CardLocation()
                        {
                            RowPosition = DropTaget.Id,
                            CardIndex = GetDropIndex(GetRelativePosition(DropTaget), DropTaget.CardsPosition)
                        }
                    );
                    ResetAllTem();
                }
                break;
            case GameOperationType.None:
                break;
            default:
                break;
        }
    }
    //右键点击之类的
    private void OnMouseOver()
    {
// #if UNITY_ANDROID || UNITY_IOS
//         if (Input.GetMouseButtonDown(0))
// #else
//         if (Input.GetMouseButtonDown(1))
// #endif
        {
            if (!RighClickActive)
            {
                var items = GetMouseAllRaycast();
                var trueitem = items.Select(x => x.GetComponent<CanRightOn>()).Where(x => x != null);
                if (trueitem.Count() == 0)
                    return;
                switch (trueitem.First().Type)
                {
                    case RightOnType.MyCemetery:
                        GameCardShowControl.ShowMyCemetery();
                        break;
                    case RightOnType.EnemyCemetery:
                        GameCardShowControl.ShowEnemyCemetery();
                        break;
                    case RightOnType.MyDeck:
                        Debug.Log("点了卡组");
                        GameCardShowControl.ShowMyDeck();
                        break;
                    case RightOnType.Card:
                        Debug.Log("右键点击了卡牌");
                        var card = trueitem.First();
                        Debug.Log("卡牌On?:" + card.GetComponent<CardMoveInfo>().IsOn);
#if !UNITY_ANDROID && !UNITY_IOS                           
                            RightClickedCardID = card.GetComponent<CardShowInfo>().CurrentCore.CardId;
                            if (!string.IsNullOrEmpty(RightClickedCardID))
                            {
                                RighClickActive = true;
                                SceneManager.LoadScene("RightClick", LoadSceneMode.Additive);
                            }
#elif UNITY_ANDROID || UNITY_IOS
                        if(IsRightClickMobile)
                        {            
                            RightClickedCardID = card.GetComponent<CardShowInfo>().CurrentCore.CardId;
                            if (!string.IsNullOrEmpty(RightClickedCardID))
                            {
                                DragCard = null;
                                CurrentPlace = CardUseInfo.ReSet;
                                CurrentPlayCard = null;
                                ResetAllTem();
                                RighClickActive = true;
                                SceneManager.LoadScene("RightClick", LoadSceneMode.Additive);
                            }
                        }
#endif
                        break;
                    default:
                        break;
                }
            }
        }
    }
    //每一帧
    private void Update()
    {
        // check if surrender
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SendSurrender();
        }
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
                if (pressTime > 0.75f && DropTaget == null)
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
        # endif
        var onObjects = GetMouseAllRaycast();//获取鼠标穿透的所有物体
        //画箭
        DrawArrows();
        //----------------------------------------------
        //无论什么时候,都可以选中卡牌,但是拖动时不能
        var selectCard = default(CardMoveInfo);
        if (DragCard == null)
        {
            var cards = onObjects.Where(x => x.GetComponent<CardMoveInfo>() != null);//获取物体集合中的所有卡牌
            selectCard = cards.Count() == 0 ? null : cards.OrderBy(x => x.transform.position.z).First().GetComponent<CardMoveInfo>();//选中第一张卡
        }
        var rows = onObjects.Where(x => x.GetComponent<CanDrop>() != null && x.GetComponent<CanDrop>().IsCanDrop);
        var dropTaget = rows.Count() != 0 ? rows.First().GetComponent<CanDrop>() : null;
        var allRows = onObjects.Where(x => x.GetComponent<CanDrop>() != null);
        //----------------------------------------------
        switch (NowOperationType)
        {
            case GameOperationType.GetPassOrGrag:
                //拖或者pass
                //if (IsOnCoin)按住硬币会执行的
                if (DragCard == null)
                {
                    if (!(IsMobileClickDown && allRows.Count() > 0))
                    {
                        SelectCard = selectCard;
                    }
                    //如果没有在拖拽的卡牌
                    if (SelectCard == null && PassCoin.IsCanUse)
                    {
                        var coin = onObjects.Where(x => x.GetComponent<PassCoin>() != null);
                        if (coin.Count() != 0 && coin.First().GetComponent<PassCoin>().IsCanUse)//当鼠标移动到硬币上的时候
                        {
                            if (!IsOnCoin)//如果没有按在硬币上
                            {
#if UNITY_ANDROID || UNITY_IOS
#if UNITY_EDITOR
                                if (Input.GetMouseButtonDown(0))
#else
                                if (Input.touchCount > 0)
#endif
                                {
                                    IsSelectCoin = true;//选中的硬币
                                    IsOnCoin = true;//按住硬币
                                }
                                else
                                {
                                    IsSelectCoin = false;
                                    IsOnCoin = false;
                                }
#else
                                IsSelectCoin = true;//选中的硬币
#endif
                            }
                        }
                        else//如果没有移动在硬币上
                        {
                            IsSelectCoin = false;//取消选中硬币
                            IsOnCoin = false;//取消按住硬币
                        }
                    }
                }
                else
                {
                    //如果正在拖拽的话..
                    if (dropTaget != null)
                    {
                        if (!dropTaget.IsRowDrop || dropTaget.CardsPosition == null)
                        {//如果不是行拖拽区域,则没有悬停效果
                            DropTaget = dropTaget;
                            return;
                        }
                        //筛选,如果单位满了则无法放置
                        dropTaget = (dropTaget.CardsPosition.MaxCards + (dropTaget.CardsPosition.IsTem() ? 1 : 0)) <= dropTaget.CardsPosition.GetTrueCardCount() ? null : dropTaget;//数量没满的第一个
                    }
                    //悬停效果处理,先把老排清空效果,然后赋值
                    if (DropTaget != dropTaget && DropTaget != null && DropTaget.CardsPosition != null) DropTaget.CardsPosition.RemoveAllTemp();
                    DropTaget = dropTaget;
                    if (DropTaget != null)
                    {   //如果新的排不是空,设定预设卡
                        var index = GetDropIndex(GetRelativePosition(DropTaget), DropTaget.CardsPosition);
                        DropTaget.CardsPosition.AddTemCard(DragCard.CardShowInfo.CurrentCore, index);
                    }
                }
                break;
            case GameOperationType.SelectCards:
                //选卡模式的话,(如果移动到卡牌上)
                SelectCard = selectCard;
                SelectModeCard = selectCard?.CardShowInfo;
                _arrowTaget = SelectModeCard?.transform;//箭头
                if (SelectCard != null)
                {
                    //Debug.Log(SelectCard);
                }
                break;
            case GameOperationType.SelectRow:
                //选排的情况下
                //DropTaget 旧效果去除
                DropTaget = dropTaget;
                _arrowTaget = DropTaget?.transform;
                //新效果添加
                break;
            case GameOperationType.PlayCard:
                //如果正在拖拽的话..
                if (dropTaget != null)
                {
                    if (!dropTaget.IsRowDrop || dropTaget.CardsPosition == null)
                    {//如果不是行拖拽区域,则没有悬停效果
                        DropTaget = dropTaget;
                        return;
                    }
                    //筛选,如果单位满了则无法放置
                    dropTaget = (dropTaget.CardsPosition.MaxCards + (dropTaget.CardsPosition.IsTem() ? 1 : 0)) <= dropTaget.CardsPosition.GetTrueCardCount() ? null : dropTaget;//数量没满的第一个
                }
                //悬停效果处理,先把老排清空效果,然后赋值
                if (DropTaget != dropTaget && DropTaget != null && DropTaget.CardsPosition != null) DropTaget.CardsPosition.RemoveAllTemp();
                DropTaget = dropTaget;
                if (DropTaget != null)
                {   //如果新的排不是空,设定预设卡
                    var index = GetDropIndex(GetRelativePosition(DropTaget), DropTaget.CardsPosition);
                    DropTaget.CardsPosition.AddTemCard(CurrentPlayCard, index);
                }
                break;
            case GameOperationType.None:
                SelectCard = selectCard;
                break;
            default:
                break;
        }
    }
    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][
    //----------------------------------------------------------------------------------------------
    //一些小方法
    // public CardMoveInfo GetCard(CardLocation location)
    // {//通过卡牌坐标获取客户端卡牌的引用
    //     if (location.RowPosition == RowPosition.MyLeader)
    //         return MyLeader.TrueCard.GetComponent<CardMoveInfo>();
    //     if (location.RowPosition == RowPosition.EnemyLeader)
    //         return EnemyLeader.TrueCard.GetComponent<CardMoveInfo>();
    //     var row = AllCardsPosition.Single(x => x.Id == location.RowPosition);
    //     var ti = location.CardIndex;
    //     for(var i = 0;i<=ti;i++)
    //     {
    //         if (row.transform.GetChild(i).GetComponent<CardShowInfo>().IsDead)
    //         {
    //             ti++;
    //         }
    //     }
    //     return row.transform.GetChild(ti).GetComponent<CardMoveInfo>();
    // }
    public CardMoveInfo GetCard(CardLocation location)
    {//通过卡牌坐标获取客户端卡牌的引用
        if (location.RowPosition == RowPosition.MyLeader)
            return MyLeader.TrueCard.GetComponent<CardMoveInfo>();
        if (location.RowPosition == RowPosition.EnemyLeader)
            return EnemyLeader.TrueCard.GetComponent<CardMoveInfo>();
        var row = AllCardsPosition.Single(x => x.Id == location.RowPosition);
        var ti = location.CardIndex;
        //-------------------------------------
        // var count = row.transform.childCount;
        // var message = $"总数:{count},目标:{location.CardIndex}|";
        // for (var i = 0; i < count; i++)
        // {
        //     var card = row.transform.GetChild(i).GetComponent<CardShowInfo>();
        //     message += $"+下标:{i},名称:{card.CurrentCore.Name},是否死亡:{card.IsDead},是否零时:{card.CardMoveInfo.IsTem}+";
        // }
        // message += "|";
        //-------------------------------------
        // Debug.Log(location.RowPosition + "" + location.CardIndex);
        // Debug.Log($"{location.RowPosition}最初选中:{ti}");
        for (var i = 0; i <= ti; i++)
        {
            if (row.transform.GetChild(i).GetComponent<CardShowInfo>().IsDead || row.transform.GetChild(i).GetComponent<CardMoveInfo>().IsTem)
            {
                ti++;
            }
        }
        // Debug.Log($"{location.RowPosition}最终选中:{ti}");
        return row.transform.GetChild(ti).GetComponent<CardMoveInfo>();
    }

    public CardLocation GetLocation(Transform card)
    {//根据客户端卡牌引用,返回对应坐标
        var isLeader = card.parent.GetComponent<LeaderCard>();
        if (isLeader != null)
        {
            return new CardLocation()
            {
                RowPosition = isLeader.Id,
                CardIndex = 0
            };
        }
        return new CardLocation()
        {
            RowPosition = card.parent.GetComponent<CardsPosition>().Id,
            CardIndex = GetIndex(card.transform)
        };
    }
    public IEnumerable<CardShowInfo> GetRangeCards(CardLocation location, int Range)
    {   //获得某个地方的卡牌,左右两边的卡牌
        if (Range > 0)
        {
            //找到需要的那一排
            var row = AllCardsPosition.Single(x => x.Id == location.RowPosition);
            for (var i = location.CardIndex - Range; i <= location.CardIndex + Range; i++)
            {
                //判断不合格的话,不返回
                if (i == location.CardIndex || i < 0 || i > row.transform.childCount - 1) continue;
                yield return row.transform.GetChild(i).GetComponent<CardShowInfo>();
            }
        }
    }
    //获得当前鼠标穿过的所有物体
    private IEnumerable<GameObject> GetMouseAllRaycast()
    {
        var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        var ray = new Ray(ray1.origin, ray1.direction * 100000);
        var items = Physics.RaycastAll(ray).Select(x => x.collider.gameObject);
        if (items.Any(x => "MessageBoxBgCardsSelectGameResult".Contains(x.name)))
        {
            return new GameObject[] { };
        }
        return items;
    }
    //获得某个物体在父物体中的位置
    // private int GetIndex(Transform obj)
    // {
    //     var p = obj.parent;
    //     if (p.gameObject.GetComponent<LeaderCard>() != null)
    //         return -1;
    //     var count = p.childCount;
    //     for (var i = 0; i < count; i++)
    //     {
    //         if (p.GetChild(i) == obj)
    //             return i;
    //     }
    //     return 0;
    // }
    private int GetIndex(Transform obj)
    {
        var p = obj.parent;
        if (p.gameObject.GetComponent<LeaderCard>() != null)
            return -1;
        var count = p.childCount;
        var ec = 0;
        for (var i = 0; i < count; i++)
        {
            if (p.GetChild(i).GetComponent<CardMoveInfo>().IsTem || p.GetChild(i).GetComponent<CardShowInfo>().IsDead)
                ec++;
            if (p.GetChild(i) == obj)
                return i - ec;
        }
        return 0;
    }
    public IList<int> GetPartRow(GameCardsPart part, RowPosition position)
    {//返回Part中指定排的集合
        switch (position)
        {
            case RowPosition.MyHand: return part.MyHandCards;
            case RowPosition.MyRow1: return part.MyRow1Cards;
            case RowPosition.MyRow2: return part.MyRow2Cards;
            case RowPosition.MyRow3: return part.MyRow3Cards;
            case RowPosition.MyStay: return part.MyStayCards;
            case RowPosition.MyLeader: return part.IsSelectMyLeader ? new List<int>() { 0 } : new List<int>();
            case RowPosition.EnemyHand: return part.EnemyHandCards;
            case RowPosition.EnemyRow1: return part.EnemyRow1Cards;
            case RowPosition.EnemyRow2: return part.EnemyRow2Cards;
            case RowPosition.EnemyRow3: return part.EnemyRow3Cards;
            case RowPosition.EnemyStay: return part.EnemyStayCards;
            case RowPosition.EnemyLeader: return part.IsSelectEnemyLeader ? new List<int>() { 0 } : new List<int>();
            default: return null;
        }
    }
    private Vector3 GetRelativePosition(CanDrop dropTaget)
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);
        return dropTaget.transform.InverseTransformPoint(hit.point);
    }
    private int GetDropIndex(Vector3 position, CardsPosition container)
    {
        if (container == null)
            return -1;
        var count = container.GetCardCount();
        // count -= container.IsTem() ? 1 : 0;
        // count -= container.DeadCount();
        // Debug.Log(container.DeadCount());
        var index = (position.x + container.XSize * (count - 1) / 2);
        index /= container.XSize;
        index += 1;
        index = index < 0 ? 0 : index;
        index = (int)(index > count ? count : index);
        var result = 0;
        for (var i = 0; i < index; i++)
        {
            var card = container.transform.GetChild(i).GetComponent<CardMoveInfo>();
            if (!(card.IsTem || card.CardShowInfo.IsDead))
            {
                result++;
            }
        }
        // Debug.Log(result);
        return result;
    }
    public IEnumerable<int> HandCanPlay()//当前可以打出的手牌
    {
        var result = 0.To(MyHand.GetTrueCardCount() - 1);//0-数量-1
        if (MyLeader.IsCardCanUse)
        {
            if (MyHand.GetTrueCardCount() > 0)
                return result.Append(-1);//
            else
                return (-1).To(-1);
        }
        return result;
    }
    public IEnumerable<CardLocation> CardCanPlay(CardUseInfo range)//当前卡牌可以放置的所有位置(不包含弃牌)
    {
        var result = new List<CardLocation>();
        switch (range)
        {
            case CardUseInfo.AnyPlace:
                result.Add(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.SpecialPlace });
                break;
            case CardUseInfo.MyPlace:
                result.Add(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.SpecialPlace });
                break;
            case CardUseInfo.EnemyPlace:
                result.Add(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.SpecialPlace });
                break;
            case CardUseInfo.MyRow:
                if (AllCardsPosition.Single(x => x.Id == RowPosition.MyRow1).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.MyRow1).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.MyRow2).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.MyRow2).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow2, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.MyRow3).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.MyRow3).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow3, CardIndex = x }));
                break;
            case CardUseInfo.EnemyRow:
                if (AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow1).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow1).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow1, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow2).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow2).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow2, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow3).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow3).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow3, CardIndex = x }));
                break;
            case CardUseInfo.AnyRow:
                if (AllCardsPosition.Single(x => x.Id == RowPosition.MyRow1).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.MyRow1).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow1, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.MyRow2).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.MyRow2).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow2, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.MyRow3).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.MyRow3).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.MyRow3, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow1).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow1).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow1, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow2).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow2).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow2, CardIndex = x }));
                if (AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow3).IsCanPlay)
                    result.AddRange(0.To(AllCardsPosition.Single(x => x.Id == RowPosition.EnemyRow3).GetTrueCardCount())
                    .Select(x => new CardLocation() { RowPosition = RowPosition.EnemyRow3, CardIndex = x }));
                break;
            case CardUseInfo.ReSet:
            default:
                break;
        }
        return result;
    }
    //---------------------------------------------------------------------------------------------------------
    //一些动画
    public void DrawArrows(Transform source, Transform taget)
    {
        //实现,画线从来源到目标!
        //如果来源为空,不需要画线
        if (source == null)
        {
            TempArrows.gameObject.SetActive(false);
            return;
        }
        var trueTaget = default(Vector3);
        var trueSource = new Vector3(source.position.x, source.position.y, ArrowsZ);
        var isLight = default(bool);
        if (taget != null)
        {
            trueTaget = new Vector3(taget.position.x, taget.position.y, ArrowsZ);
            isLight = true;
        }
        else
        {
            var mTaget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            trueTaget = new Vector3(mTaget.x, mTaget.y, ArrowsZ);
            isLight = false;
        }
        //这里需要根据三个参数来实现箭头
        TempArrows.gameObject.SetActive(true);
        TempArrows.SetArrows(trueSource, trueTaget, isLight);
    }
    public void DrawArrows() => DrawArrows(_arrowSource, _arrowTaget);
    public void ShowNumber(CardLocation location, int number, NumberType type = NumberType.Normal)
    {
        var p = default(Transform);
        switch (location.RowPosition)
        {
            case RowPosition.MyDeck:
                p = MyDeck;
                break;
            case RowPosition.MyCemetery:
                p = MyCemetery.transform;
                break;
            case RowPosition.EnemyDeck:
                p = EnemyDeck;
                break;
            case RowPosition.EnemyCemetery:
                p = EnemyCemetery;
                break;
            default:
                p = GetCard(location).transform;
                break;
        }
        ShowNumber(p, number, type);
    }
    public void ShowNumber(Transform location, int number, NumberType type = NumberType.Normal)
    {
        var obj = Instantiate(NumberPrefab, location).GetComponent<CardText>();
        obj.transform.localPosition = new Vector3(0, 0, -0.2f);
        obj.SetNumber(number, type);
        obj.Show();
    }
    public void ShowBullet(CardLocation source, CardLocation taget, BulletType type)
    {
        //Debug.Log($"请脑补一条弹道,他的出发点是:{source.RowPosition}排,第{source.CardIndex}个单位,到{taget.RowPosition}排,第{taget.RowPosition}个单位,弹道特效类型为:{type}");
        //*****************************************************************
        //等待补充,从一个地方发射到另一个地方,用某个类型的弹道样式
        //*****************************************************************
    }
    public void ShowCardIconEffect(CardLocation location, CardIconEffectType type)
    {
        //*****************************************************************
        //等待补充,展示图标的特效
        //*****************************************************************
    }
    public void ShowCardBreakEffect(CardLocation location, CardBreakEffectType type)
    {
        //等待补充,展示卡牌破坏的特效(在CardShowInfo)
        //Debug.Log($"破坏卡牌,强制移出,卡牌坐标为{location.RowPosition}排,第{location.CardIndex}个");
        GetCard(location).CardShowInfo.ShowCardBreak(type);
    }
    public void ShowWeatherApply(RowPosition row, RowStatus type)
    {
        if (!row.IsOnPlace()) return;
        //*****************************************************************
        //等待补充,天气动画
        //****************************************************************
        ShowWeather.SetWeather(row, type);
    }
    //------------------------------------------------------------------------------------------------
    //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>
    //这里是所有回应服务端的方法(由服务端触发)
    public async Task SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, LocalPlayer player)
    {
        //Debug.Log(rowPart.Join("+"));
        //预处理
        rowPart.ForAll(x =>
        {
            AllCanDrop.Single(row => row.Id == x).IsCanDrop = true;//点亮
        });
        CanSelectRow = rowPart;
        _arrowSource = selectCard != null ? GetCard(selectCard).transform : null;//设定选择方,空则为null
        NowOperationType = GameOperationType.SelectRow;//当前的状态是!选择排!
        //得到讯息
        ///////////自动选排22222222222222
        if (IsAutoPlay)
        {
            await sender.SendAsync(rowPart.Mess().First());
        }
        Debug.Log(ropeController.remainingTime.ToString());
        var result = await TimeLimitHandler.ReceiveWithTimeLimitAsync<RowPosition>(receiver, async () =>
        {
            await sender.SendAsync(rowPart.Mess().First());
        }, ropeController.remainingTime);
        Debug.Log(_arrowSource);
        if (_arrowSource != null)
            _arrowSource.GetComponent<CardShowInfo>().SpecialAudioPlay();
        //收尾工作
        _arrowSource = null;
        _arrowTaget = null;//箭头处理
        DropTaget = null;//不需要的东西都清空
        NowOperationType = GameOperationType.None;//收工了
        CurrentPlace = CardUseInfo.ReSet;//熄灭
                                         //发送讯息
        await player.SendAsync(UserOperationType.RoundOperate, result);

    }
    public async Task SelectPlaceCards(PlaceSelectCardsInfo info, LocalPlayer player)
    {
        //预处理
        AllCardGray(true);//全卡灰
        PartCardGray(info.CanSelect, false);//选卡亮
        if (info.SelectCard != null)
            CardGray(info.SelectCard, false);//亮!
        SelectPlaceCardsInfo = info;
        NowSelectCards = new List<CardLocation>();//清空一下现在选中的牌
        _arrowSource = info.SelectCard != null ? GetCard(info.SelectCard).transform : null;//设定选择方,空则为null
        NowOperationType = GameOperationType.SelectCards;//当前的状态!选择卡牌!
        //得到讯息
        ///////////自动选卡22222222222222
        if (IsAutoPlay)
        {
            await sender.SendAsync<IList<CardLocation>>
            (info.CanSelect.CardsPartToLocation().Mess().Take(info.SelectCount).ToList());
        }
        Debug.Log($"剩余时间还有{ropeController.remainingTime}");
        var result = await TimeLimitHandler.ReceiveWithTimeLimitAsync<IList<CardLocation>>(receiver, async () =>
        {
            await sender.SendAsync<IList<CardLocation>>(info.CanSelect.CardsPartToLocation().Mess().Take(info.SelectCount).ToList());
        }, ropeController.remainingTime);
        Debug.Log(_arrowSource);
        if (_arrowSource != null)
            _arrowSource.GetComponent<CardShowInfo>().SpecialAudioPlay();
        //收尾工作 
        _arrowSource = null;
        _arrowTaget = null;//箭头处理
        NowOperationType = GameOperationType.None;//收了
        NowSelectCards = null;
        SelectModeCard = null;
        AllCardsPosition.ForAll(row => row.GetCards().ForAll(card => card.CardShowInfo.SetSelect(false, false)));
        AllCardGray(false);//全卡亮
                           //发送讯息
        await player.SendAsync(UserOperationType.RoundOperate, result);

    }
    //让玩家使用一个卡牌,或者pass
    public async Task GetPlayerDrag(LocalPlayer player)//（RoundStart）
    {
        ropeController.StartRopeTimer();
        //预处理
        PassCoin.IsCanUse = true;//硬币可用
        MyHand.CardsCanDrag(true);
        MyLeader.SetCanDrag(true);
        NowOperationType = GameOperationType.GetPassOrGrag;//状态!拖或者pass!
        //得到讯息
        ///////////自动出牌22222222222222
        var stayPlayCardIndex = HandCanPlay().Mess().First();
        if (IsAutoPlay)
        {
            //HandCanPlay().Mess().ForAll(x=>Debug.Log(x));////
            var card = default(CardUseInfo);
            if (stayPlayCardIndex == -1)
            {
                card = GetCard(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.MyLeader }).CardUseInfo;
                MyLeader.IsCardCanUse = false;
            }
            else
            {
                card = GetCard(new CardLocation() { CardIndex = stayPlayCardIndex, RowPosition = RowPosition.MyHand }).CardUseInfo;
            }
            var cardCanUse = CardCanPlay(card);
            await sender.SendAsync(new RoundInfo()
            {
                IsPass = false,
                HandCardIndex = stayPlayCardIndex,
                CardLocation = (cardCanUse.Count() == 0 ? new CardLocation() { CardIndex = 0, RowPosition = RowPosition.MyCemetery } :
                (
                    cardCanUse.GroupBy(x => x.RowPosition).Mess().First().Mess().First()
                ))
            });
        }
        Debug.Log($"剩余时间还有{ropeController.remainingTime}");
        var result = await TimeLimitHandler.ReceiveWithTimeLimitAsync<RoundInfo>(receiver, async () =>
        {
            var card = default(CardUseInfo);
            if (stayPlayCardIndex == -1)
            {
                MyLeader.IsCardCanUse = false;
            }
            var cardCanUse = CardCanPlay(card);
            await sender.SendAsync(new RoundInfo()
            {
                IsPass = false,
                HandCardIndex = stayPlayCardIndex,
                CardLocation = new CardLocation() { CardIndex = 0, RowPosition = RowPosition.MyCemetery }
            });
        }, ropeController.remainingTime);
        //事后
        NowOperationType = GameOperationType.None;//了
        PassCoin.IsCanUse = false;//硬币不可用
        MyHand.CardsCanDrag(false);
        MyLeader.SetCanDrag(false);
        //发送讯息
        await player.SendAsync(UserOperationType.RoundOperate, result);
    }
    public async Task GetPlayCard(CardLocation location, LocalPlayer player)
    {
        //location 增加高光
        CurrentPlayCard = GetCard(location).CardShowInfo.CurrentCore;
        CurrentPlace = GetCard(location).CardUseInfo;
        CloseAllRowMaxCanDrop();
        NowOperationType = GameOperationType.PlayCard;//放置牌模式
        DropTaget = null;

        //得到讯息
        ///////////自动选位22222222222222
        if (IsAutoPlay)
        {
            await sender.SendAsync(CardCanPlay(CurrentPlace).Mess().First());
        }
        Debug.Log($"剩余时间还有{ropeController.remainingTime}");
        var result = await TimeLimitHandler.ReceiveWithTimeLimitAsync<CardLocation>(receiver, async () =>
        {
            await sender.SendAsync(CardCanPlay(CurrentPlace).Mess().First());
        }, ropeController.remainingTime);
        NowOperationType = GameOperationType.None;
        CurrentPlace = CardUseInfo.ReSet;
        CurrentPlayCard = null;
        //location 取消高光

        //发送讯息
        await player.SendAsync(UserOperationType.PlayCardInfo, result);
    }
    //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>
    //-------------------------------------------------------------------------------------------------
    //以下为给服务端调用的方法
    //将某个卡牌丢到墓地或者卡组
    public void CardMoveBack(CardMoveInfo card, Transform taget)
    {
        var source = card.transform.parent.gameObject.GetComponent<CardsPosition>();
        card.transform.SetParent(taget);
        card.SetResetPoint(Vector3.zero);
        //card.transform.localScale = Vector3.one;
        card.CardShowInfo.ScaleTo(1);
        card.IsCanSelect = false;
        Destroy(card.gameObject, 0.8f);
        if (source != null)
            source.ResetCards();
    }
    public void CardMove(MoveCardInfo info)//卡牌移动
    {
        var soureCard = default(CardMoveInfo);
        var tagetRow = default(CardsPosition);
        //几个特殊的移动原始位置

        switch (info.Source.RowPosition)
        {
            case RowPosition.MyLeader:
                if (MyLeader.TrueCard == null) return;
                soureCard = MyLeader.TrueCard.GetComponent<CardMoveInfo>();
                break;
            case RowPosition.EnemyLeader:
                if (EnemyLeader.TrueCard == null) return;
                soureCard = EnemyLeader.TrueCard.GetComponent<CardMoveInfo>();
                break;
            case RowPosition.MyDeck:
                soureCard = Instantiate(CardPrefab, MyDeck).GetComponent<CardMoveInfo>();
                break;
            case RowPosition.EnemyDeck:
                soureCard = Instantiate(CardPrefab, EnemyDeck).GetComponent<CardMoveInfo>();
                break;
            case RowPosition.MyCemetery:
                soureCard = Instantiate(CardPrefab, MyCemetery.transform).GetComponent<CardMoveInfo>();
                break;
            case RowPosition.EnemyCemetery:
                soureCard = Instantiate(CardPrefab, EnemyCemetery).GetComponent<CardMoveInfo>();
                break;
            case RowPosition.SpecialPlace://这是个错误值
                break;
            default:
                soureCard = GetCard(info.Source);
                break;
        }
        //以上是来源卡牌
        if (soureCard == null) return;
        if (info.Card != null)
        {//如果卡牌有额外信息,直接替换掉当前选中的卡牌(例如从牌库,或者手牌抽出的卡牌)
            soureCard.CardShowInfo.CurrentCore = info.Card;
            //soureCard.CardShowInfo.SetCard();
        }
        soureCard.IsStay = false;
        //------------------------------------------
        //移动到
        switch (info.Target.RowPosition)
        {
            case RowPosition.MyDeck:
                CardMoveBack(soureCard, MyDeck);
                break;
            case RowPosition.EnemyDeck:
                CardMoveBack(soureCard, EnemyDeck);
                break;
            case RowPosition.MyCemetery:
                CardMoveBack(soureCard, MyCemetery.transform);
                break;
            case RowPosition.EnemyCemetery:
                CardMoveBack(soureCard, EnemyCemetery.transform);
                break;
            case RowPosition.MyLeader:
            case RowPosition.EnemyLeader:
            case RowPosition.SpecialPlace:
                return;
            default:
                tagetRow = AllCardsPosition.Single(x => x.Id == info.Target.RowPosition);
                tagetRow.AddCard(soureCard, tagetRow.GetTrueIndex(info.Target.CardIndex));
                break;
        }
    }
    public void CreateCard(CardStatus card, CardLocation location)
    {
        if (location.RowPosition.IsOnRow())
        {
            var row = AllCardsPosition.Single(x => x.Id == location.RowPosition);
            row.CreateCardTo(card, location.CardIndex);
        }
        else
        {
            //白雾一样的,在看不见的地方创造,和没有一样
        }
    }
    public void SetCard(CardLocation location, CardStatus card)
    {
        // Debug.Log("刷新了卡牌设置");
        // Debug.Log($"卡牌名称是:{card.Name},生命状态是:{card.HealthStatus}");
        var clientCard = GetCard(location);
        clientCard.CardShowInfo.CurrentCore = card;
        //clientCard.CardShowInfo.SetCard();
    }
    public void CardOn(CardLocation location)//卡牌抬起
    {
        var card = GetCard(location);
        card.IsOn = true;
    }
    public void CardDown(CardLocation location)//卡牌落下
    {
        // Debug.Log($"受到卡牌落下的请求,Index:{location.CardIndex}");
        MyLeader.AutoSet();
        EnemyLeader.AutoSet();
        //上面这两行偷懒了,或许以后会改
        var card = GetCard(location);
        card.IsOn = false;
    }
    //展示将一些卡丢到墓地
    public void ShowCardsToCemetery(GameCardsPart cards)
    {
        cards.MyRow1Cards.Select(x => MyRow1.CardsPosition.transform.GetChild(x).GetComponent<CardMoveInfo>())
        .ForAll(x => CardMoveBack(x, MyCemetery.transform));
        cards.MyRow2Cards.Select(x => MyRow2.CardsPosition.transform.GetChild(x).GetComponent<CardMoveInfo>())
        .ForAll(x => CardMoveBack(x, MyCemetery.transform));
        cards.MyRow3Cards.Select(x => MyRow3.CardsPosition.transform.GetChild(x).GetComponent<CardMoveInfo>())
        .ForAll(x => CardMoveBack(x, MyCemetery.transform));
        cards.EnemyRow1Cards.Select(x => EnemyRow1.CardsPosition.transform.GetChild(x).GetComponent<CardMoveInfo>())
        .ForAll(x => CardMoveBack(x, EnemyCemetery.transform));
        cards.EnemyRow2Cards.Select(x => EnemyRow2.CardsPosition.transform.GetChild(x).GetComponent<CardMoveInfo>())
        .ForAll(x => CardMoveBack(x, EnemyCemetery.transform));
        cards.EnemyRow3Cards.Select(x => EnemyRow3.CardsPosition.transform.GetChild(x).GetComponent<CardMoveInfo>())
        .ForAll(x => CardMoveBack(x, EnemyCemetery.transform));
    }
    //设置硬币
    public void SetCoinInfo(bool isBlue)
    {
        AudioManager.Instance.PlayAudio("Coin", AudioType.Effect, AudioPlayMode.PlayOneShoot);
        Coin.IsMyRound = isBlue;
    }
    //告诉玩家回合结束
    public void RoundEnd()
    {
        //等待修改,处理回合结束相关
        //PassCoin.IsMyRound = false;
    }
    //强制刷新掉所有的辅助临时卡(暂时没必要)
    public void ResetAllTem()
    {
        EnemyRow1.CardsPosition.RemoveAllTemp();
        EnemyRow2.CardsPosition.RemoveAllTemp();
        EnemyRow3.CardsPosition.RemoveAllTemp();
        MyRow1.CardsPosition.RemoveAllTemp();
        MyRow2.CardsPosition.RemoveAllTemp();
        MyRow3.CardsPosition.RemoveAllTemp();
    }
    //设定场上所有卡(灰不灰)
    public void AllCardGray(bool isGray)
    {
        var card = default(CardShowInfo);
        if (MyLeader.TrueCard != null)
        {
            card = MyLeader.TrueCard.GetComponent<CardShowInfo>();
            card.IsGray = isGray;
        }
        if (EnemyLeader.TrueCard != null)
        {
            card = EnemyLeader.TrueCard.GetComponent<CardShowInfo>();
            card.IsGray = isGray;
        }
        AllCardsPosition.ForAll(x => x.SetAllCardGray(isGray));//将所有的卡牌灰化
    }

    public void CloseAllRowMaxCanDrop()
    {
        for (var i = 0; i < AllCanDrop.Length; i++)
        {
            AllCanDrop[i].IfMaxDotDrop();
        }
    }

    //设定场上部分卡(灰不灰)
    public void PartCardGray(GameCardsPart part, bool isGray)
    {
        var card = default(CardShowInfo);
        if (part.IsSelectMyLeader && MyLeader.TrueCard != null)
        {
            card = MyLeader.TrueCard.GetComponent<CardShowInfo>();
            card.IsGray = isGray;
        }
        if (part.IsSelectEnemyLeader && EnemyLeader.TrueCard != null)
        {
            card = EnemyLeader.TrueCard.GetComponent<CardShowInfo>();
            card.IsGray = isGray;
        }
        AllCardsPosition.ForAll(x => x.SetPartCardGray(GetPartRow(part, x.Id), isGray));
    }

    //设定场上单卡(灰不灰)
    public void CardGray(CardLocation location, bool isGray)
    {
        var card = GetCard(location);
        card.CardShowInfo.IsGray = isGray;
    }

    public async void SendSurrender() // 发出投降信息
    {
        if (!await _uiService.YNMessageBox(translator.GetText("PopupWindow_Surrender_title"), translator.GetText("PopupWindow_Surrender_content")))
        {
            return;
        }
        await DependencyResolver.Container.Resolve<GwentClientService>().Surrender();
        if (SceneManager.GetSceneByName("RightClick").isLoaded)
        {
            SceneManager.UnloadSceneAsync("RightClick");
        }
    }
}
