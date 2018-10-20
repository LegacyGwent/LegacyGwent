using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cynthia.Card;

namespace Cynthia.Card
{
    public interface IGwentServerGame
    {
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
        Faction[] PlayersFaction { get; set; }//玩家们的势力
        bool[] IsPlayersPass { get; set; }//玩家是否已经pass
        bool[] IsPlayersMulligan { get; set; }//玩家是否调度完毕
        int Player1Index { get; }//玩家1的坐标
        int Player2Index { get; }//玩家2的坐标
        Task<bool> Play();
        Task<bool> PlayerRound();
        Task RoundPlayCard(int playerIndex, RoundInfo cardInfo);//哪一位玩家,打出第几张手牌,打到了第几排,第几列
        IList<GameCard> LogicDrawCard(int playerIndex, int count);//或许应该播放抽卡动画和更新数值
        //封装的抽卡
        Task<(List<GameCard>, List<GameCard>)> DrawCard(int player1Count, int player2Count);
        //封装的调度
        Task MulliganCard(int playerIndex, int count);
        Task<List<GameCard>> DrawCardAnimation(int myPlayerIndex, int myPlayerCount, int enemyPlayerIndex, int enemyPlayerCount);
        //-------------------------------------------------------------------------------------------------------------------------
        //下面是发送数据包,或者进行一些初始化信息
        //根据当前信息,处理游戏结果
        //将某个列表中的元素,移动到另一个列表的某个位置,然后返回被移动的元素     
        GameCard LogicCardMove(IList<GameCard> soure, int soureIndex, IList<GameCard> taget, int tagetIndex);
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
        //Task GetCardFrom(int playerIndex, RowPosition getPosition, RowPosition taget, int index, CardStatus cardInfo);
        //Task SetCardTo(int playerIndex, RowPosition rowIndex, int cardIndex, RowPosition tagetRowIndex, int tagetCardIndex);
        //动画系
        Task SendCardOn(int playerIndex, CardLocation location);
        Task SendCardDown(int playerIndex, CardLocation location);
        Task SendCardMove(int playerIndex, MoveCardInfo info);
        Task ShowCardMove(CardLocation tage, GameCard card);//移动
        Task ShowCardDown(GameCard card);//落下(收到天气陷阱,或者其他卡牌)
        Task ShowCardOn(GameCard card);//抬起
        //-----------------------
        Task SendSetCard(int playerIndex, GameCard card);//更新卡牌
        Task ShowSetCard(GameCard card);//更新卡牌
        //------------------------
        Task SendCardNumberChange(int playerIndex, GameCard card, int num, NumberType type);//展示卡牌数字的变化
        Task ShowCardNumberChange(GameCard card, int num, NumberType type);//展示卡牌数字的变化
        //
        Task SendBullet(int playerIndex, GameCard source, GameCard taget, BulletType type);
        Task ShowBullet(GameCard source, GameCard taget, BulletType type);
        //
        Task SendCardIconEffect(int playerIndex, GameCard card, CardIconEffectType type);
        Task ShowCardIconEffect(GameCard card, CardIconEffectType type);
        //
        Task SendCardBreakEffect(int playerIndex, GameCard card, CardBreakEffectType type);
        Task ShowCardBreakEffect(GameCard card, CardBreakEffectType type);
        //
        //----------------------------------------------------------------------------------------------
        //交互系
        Task<IList<int>> GetSelectMenuCards(int playerIndex, MenuSelectCardInfo info);//返回点击列表卡牌的顺序
        Task<IList<CardLocation>> GetSelectPlaceCards(int playerIndex, PlaceSelectCardsInfo info);//返回点击场上卡牌的顺序
        Task<RowPosition> GetSelectRow(int playerIndex, CardLocation selectCard, IList<RowPosition> rowPart);//返回选中的牌
        Task<CardLocation> GetPlayCard(int playerIndex, GameCard card);//告诉玩家要打出什么牌,获取玩家选择的位置
        //----------------------------------------------------------------
        //方便的封装效果
        GameCardsPart GetGameCardsPart(int playerIndex, Func<GameCard, bool> Sizer, bool isContainHand = false);
        int GameCardsPartCount(GameCardsPart part);
        GameCardsPart MirrorGameCardsPart(GameCardsPart part);
        Task SendGameResult(TwoPlayer player);
        void ToCemeteryInfo(GameCard card);
        Task SendBigRoundEndToCemetery();
        RowPosition ListToRow(int myPlayerIndex, IList<GameCard> list);
        IList<GameCard> RowToList(int myPlayerIndex, RowPosition row);
        int AnotherPlayer(int playerIndex);
        CardLocation GetCardLocation(int playerIndex, GameCard card);
        GameCard GetCard(int playerIndex, CardLocation location);
        Task Debug(string msg);
    }
}