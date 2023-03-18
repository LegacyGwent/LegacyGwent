using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cynthia.Card;

namespace Cynthia.Card
{
    public interface IGwentServerGame
    {
        Pipeline OperactionList { get; }
        GameResult TempGameResult { get; set; }
        Task AddTask(params Func<Task>[] task);
        int RowMaxCount { get; set; }
        Random RNG { get; }
        IList<(int PlayerIndex, GameCard CardId)> HistoryList { get; set; }
        GameDeck[] PlayerBaseDeck { get; set; }
        Player[] Players { get; set; }//玩家数据传输/
        //bool[] IsPlayersLeader { get; set; }//玩家领袖是否可用/
        IList<GameCard>[] PlayersLeader { get; set; }//玩家领袖是?/
        TwoPlayer GameRound { get; set; }//谁的的回合----
        int RoundCount { get; set; }//有效比分的回合数
        int CurrentRoundCount { get; set; }//当前小局
        int[] PlayersWinCount { get; set; }//玩家胜利场数/
        int[][] PlayersRoundResult { get; set; }//三局r1 r2 r3
        IList<GameCard>[] PlayersDeck { get; set; }//玩家卡组/
        IList<GameCard>[] PlayersHandCard { get; set; }//玩家手牌/
        IList<GameCard>[][] PlayersPlace { get; set; }//玩家场地/
        IList<GameCard>[] PlayersCemetery { get; set; }//玩家墓地/
        IList<GameCard>[] PlayersStay { get; set; }//玩家悬牌
        // RowStatus[][] GameRowStatus { get; set; }//玩家天气
        GameRow[][] GameRowEffect { get; set; }//每排的特效
        Faction[] PlayersFaction { get; set; }//玩家们的势力
        bool[] IsPlayersPass { get; set; }//玩家是否已经pass
        bool[] IsPlayersMulligan { get; set; }//玩家是否调度完毕
        int Player1Index { get; }//玩家1的坐标
        int Player2Index { get; }//玩家2的坐标
        (int? PlayerIndex, int HPoint) WhoHeight { get; }
        int TurnCardPlayedNum { get; set; }//本回合打出牌的数量
        Task<GameResult> Play();
        Task<bool> PlayerRound();
        Task RoundPlayCard(int playerIndex, RoundInfo cardInfo);//哪一位玩家,打出第几张手牌,打到了第几排,第几列
        Task<IList<GameCard>> LogicDrawCard(int playerIndex, int count, Func<GameCard, bool> filter = null);//或许应该播放抽卡动画和更新数值
        //封装的抽卡
        Task<(List<GameCard>, List<GameCard>)> DrawCard(int player1Count, int player2Count, Func<GameCard, bool> filter = null);
        Task<List<GameCard>> PlayerDrawCard(int playerIndex, int count = 1, Func<GameCard, bool> filter = null);
        //封装的调度
        Task MulliganCard(int playerIndex, int count);
        Task<List<GameCard>> DrawCardAnimation(int myPlayerIndex, int myPlayerCount, int enemyPlayerIndex, int enemyPlayerCount, Func<GameCard, bool> filter = null);
        //-------------------------------------------------------------------------------------------------------------------------
        //下面是发送数据包,或者进行一些初始化信息
        //根据当前信息,处理游戏结果
        //将某个列表中的元素,移动到另一个列表的某个位置,然后返回被移动的元素     
        Task<GameCard> LogicCardMove(GameCard source, IList<GameCard> target, int tagetIndex, bool autoUpdateCemetery = true, bool autoUpdateDeck = true);
        Task GameOverExecute();
        //----------------------------------------------------------------------------------------------
        //自动向玩家推送更新消息
        Task SetAllInfo();
        Task SetCemeteryInfo(int playerIndex);
        Task SetGameInfo();
        Task SetCardsInfo();
        Task SetPointInfo();
        Task SetCountInfo();
        Task SetPassInfo();
        Task SetMulliganInfo();
        Task SetWinCountInfo();
        Task SetNameInfo();
        //---------------------------------------------------------------------------------------------
        //Task GetCardFrom(int playerIndex, RowPosition getPosition, RowPosition target, int index, CardStatus cardInfo);
        //Task SetCardTo(int playerIndex, RowPosition rowIndex, int cardIndex, RowPosition tagetRowIndex, int tagetCardIndex);
        //动画系
        Task ShowWeatherApply(int playerIndex, RowPosition row, RowStatus type);
        Task ShowCardMove(CardLocation tage, GameCard card, bool refresh = true, bool refreshPoint = false, bool isShowEnemyBack = false, bool autoUpdateCemetery = true, bool autoUpdateDeck = true);//移动
        Task SendCardMove(int playerIndex, MoveCardInfo info);
        Task ShowCardDown(GameCard card);//落下(收到天气陷阱,或者其他卡牌)
        Task SendCardDown(int playerIndex, CardLocation location);
        Task ShowCardOn(GameCard card);//抬起
        Task SendCardOn(int playerIndex, CardLocation location);
        //-----------------------
        Task<GameCard> CreateCard(string CardId, int playerIndex, CardLocation position, Action<CardStatus> setting = null);
        Task<int> CreateAndMoveStay(int playerIndex, string[] cards, int createCount = 1, bool isCanOver = false, string title = "选择生成一张卡");
        Task SendSetCard(int playerIndex, GameCard card);//更新卡牌
        Task ShowSetCard(GameCard card);//更新卡牌
        //------------------------
        Task SendCardNumberChange(int playerIndex, GameCard card, int num, NumberType type);//展示卡牌数字的变化
        Task ShowCardNumberChange(GameCard card, int num, NumberType type);//展示卡牌数字的变化
        //
        Task SendBullet(int playerIndex, GameCard source, GameCard target, BulletType type);
        Task ShowBullet(GameCard source, GameCard target, BulletType type);
        //
        Task SendCardIconEffect(int playerIndex, GameCard card, CardIconEffectType type);
        Task ShowCardIconEffect(GameCard card, CardIconEffectType type);
        //
        Task SendCardBreakEffect(int playerIndex, GameCard card, CardBreakEffectType type);
        Task ShowCardBreakEffect(GameCard card, CardBreakEffectType type);
        //
        //----------------------------------------------------------------------------------------------
        //交互系
        Task<IList<GameCard>> GetSelectMenuCards(int playerIndex, IList<GameCard> selectList, int selectCount = 1, string title = "选择一张卡牌", bool isEnemyBack = false, bool isCanOver = true);//返回点击列表卡牌的顺序
        Task<IList<int>> GetSelectMenuCards(int playerIndex, IList<CardStatus> selectList, int selectCount = 1, bool isCanOver = false, string title = "选择一张卡牌");//返回点击列表卡牌的顺序
        Task<IList<int>> GetSelectMenuCards(int playerIndex, MenuSelectCardInfo info);//返回点击列表卡牌的顺序
        Task<IList<GameCard>> GetSelectPlaceCards(GameCard card, int count = 1, bool isEnemySwitch = false, Func<GameCard, bool> filter = null, SelectModeType selectMode = SelectModeType.AllRow, CardType selectType = CardType.Unit, int range = 0, bool isHasConceal = false);
        Task<IList<CardLocation>> GetSelectPlaceCards(int playerIndex, PlaceSelectCardsInfo info);//返回点击场上卡牌的顺序
        Task<RowPosition> GetSelectRow(int playerIndex, GameCard card, IList<RowPosition> rowPart);//返回选中的牌
        Task<CardLocation> GetPlayCard(GameCard card, bool isAnother = false);//告诉玩家要打出什么牌,获取玩家选择的位置
        //----------------------------------------------------------------
        //方便的封装效果
        GameCardsPart GetGameCardsPart(int playerIndex, Func<GameCard, bool> filter, SelectModeType selectMode = SelectModeType.All);
        int GameCardsPartCount(GameCardsPart part);
        GameCardsPart MirrorGameCardsPart(GameCardsPart part);
        Task SendGameResult(int playerIndex, GameStatus coerceResult = GameStatus.None);
        Task SendBigRoundEndToCemetery();
        Task SetCemeteryInfo();
        RowPosition ListToRow(int myPlayerIndex, IList<GameCard> list);
        IList<GameCard> RowToList(int myPlayerIndex, RowPosition row, bool isHasDead = false, bool isHasConceal = false);
        int AnotherPlayer(int playerIndex);
        int GetPlayersPoint(int playerIndex);
        CardLocation GetCardLocation(int playerIndex, GameCard card);
        CardLocation GetRandomCanPlayLocation(int playerIndex, bool isAtEnd = false, RowPosition onlyRow = RowPosition.None);
        CardLocation GetCardLocation(GameCard card);
        GameCard GetCard(int playerIndex, CardLocation location);
        IList<GameCard> GetAllCard(int playerIndex, bool isContaiDead = false, bool isHasConceal = false);
        Task Debug(string msg);
        Task MessageBox(string msg);
        Task ClientDelay(int millisecondsDelay, int? playerIndex = null);
        Task<TEvent> SendEvent<TEvent>(TEvent @event) where TEvent : Event;
        CardEffect CreateEffectInstance(string effectId, GameCard targetCard);
        Task SendOperactionList();
    }
}