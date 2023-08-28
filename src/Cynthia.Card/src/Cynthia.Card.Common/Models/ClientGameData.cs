using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions;

namespace Cynthia.Card
{
    public class ClientGameData
    {
        public int MyRow1Point { get; set; }
        public int MyRow2Point { get; set; }
        public int MyRow3Point { get; set; }
        public int EnemyRow1Point { get; set; }
        public int EnemyRow2Point { get; set; }
        public int EnemyRow3Point { get; set; }
        public bool IsEnemyPlayerMulligan { get; set; }//对方是否调度中
        public bool IsMyPlayerMulligan { get; set; }//我方是否调度中
        public bool IsMyPlayerPass { get; set; }//我方pass
        public bool IsEnemyPlayerPass { get; set; }//对手pass
        public bool IsMyLeader { get; set; }//我方领袖是否使用
        public bool IsEnemyLeader { get; set; }//敌方领袖是否使用
        public IList<CardStatus> MyLeader { get; set; } = new List<CardStatus>();//我方领袖是?
        public IList<CardStatus> EnemyLeader { get; set; } = new List<CardStatus>();//敌方领袖是?
        public string EnemyName { get; set; }//对手名称
        public string MyName { get; set; }//对手名称
        public int MyHandCount { get; set; }//我方手牌数量
        public int EnemyHandCount { get; set; }//敌方手牌数量
        public int MyCemeteryCount { get; set; }//我方墓地数量
        public int EnemyCemeteryCount { get; set; }//敌方墓地数量
        public int EnemyDeckCount { get; set; }//对手剩余卡组数量
        public int MyDeckCount { get; set; }//我方剩余卡组数量
        public int MyWinCount { get; set; }//我方剩余卡组数量
        public int EnemyWinCount { get; set; }//我方剩余卡组数量
        public IList<CardStatus> MyHandCard { get; set; } = new List<CardStatus>();//我方手牌(数量)
        public IList<CardStatus> EnemyHandCard { get; set; } = new List<CardStatus>();//敌方手牌(数量)
        public IList<CardStatus> MyStay { get; set; } = new List<CardStatus>();//我方悬牌
        public IList<CardStatus> EnemyStay { get; set; } = new List<CardStatus>();//敌方悬牌
        public IList<CardStatus>[] MyPlace { get; set; } = new List<CardStatus>[3] { new List<CardStatus>(), new List<CardStatus>(), new List<CardStatus>() };//我方场地
        public IList<CardStatus>[] EnemyPlace { get; set; } = new IList<CardStatus>[3] { new List<CardStatus>(), new List<CardStatus>(), new List<CardStatus>() };//敌方场地
        public IList<CardStatus> MyCemetery { get; set; } = new List<CardStatus>();//我方墓地
        public IList<CardStatus> EnemyCemetery { get; set; } = new List<CardStatus>();//敌方墓地

        public IList<CardStatus> MyDeck { get; set; } = new List<CardStatus>();

        public RowStatus[] MyRowStatus { get; set; } = new RowStatus[3];
        public RowStatus[] EnemyRowStatus { get; set; } = new RowStatus[3];

        public bool IsMyTurn { get; set; }

        //------------------------------
        public IList<CardStatus> GetRow(RowPosition position)
        {
            switch (position)
            {
                case RowPosition.MyStay:
                    return MyStay;
                case RowPosition.EnemyStay:
                    return EnemyStay;
                case RowPosition.MyHand:
                    return MyHandCard;
                case RowPosition.EnemyHand:
                    return EnemyHandCard;
                case RowPosition.MyCemetery:
                    return MyCemetery;
                case RowPosition.EnemyCemetery:
                    return EnemyCemetery;
                case RowPosition.MyLeader:
                    return MyLeader;
                case RowPosition.EnemyLeader:
                    return EnemyLeader;
                case RowPosition.MyRow1:
                    return MyPlace[0];
                case RowPosition.MyRow2:
                    return MyPlace[1];
                case RowPosition.MyRow3:
                    return MyPlace[2];
                case RowPosition.EnemyRow1:
                    return EnemyPlace[0];
                case RowPosition.EnemyRow2:
                    return EnemyPlace[1];
                case RowPosition.EnemyRow3:
                    return EnemyPlace[2];
                default:
                    return null;
            }
        }
        public CardStatus GetCard(CardLocation location)
        {
            return GetRow(location.RowPosition)[location.CardIndex];
        }

        //--------------------
        //新纪元系列(选择卡牌,选择场地卡牌,选择行和更新卡牌信息)

        public void SetCard(CardLocation location, CardStatus card)
        {
            var row = GetRow(location.RowPosition);
            row[location.CardIndex] = card;
        }
        public void CreateCard(CardStatus card, CardLocation location)
        {
            if (!location.RowPosition.IsOnRow())
            {
                return;
            }
            var row = GetRow(location.RowPosition);
            row.Insert(location.CardIndex, card);
        }

        //-------------------------------------------------------------------------------------------
        //更新数据的方法们
        public void SetMulliganInfo(GameInfomation gameInfomation)
        {
            IsMyPlayerMulligan = gameInfomation.IsMyPlayerMulligan;
            IsEnemyPlayerMulligan = gameInfomation.IsEnemyPlayerMulligan;
        }
        public void SetAllInfo(GameInfomation gameInfomation)//更新全部数据
        {
            SetGameInfo(gameInfomation);
            SetCardsInfo(gameInfomation);
        }
        public void SetMyCemeteryInfo(IList<CardStatus> myCemetery)
        {
            MyCemetery = myCemetery;
        }
        public void SetEnemyCemeteryInfo(IList<CardStatus> enemyCemetery)
        {
            EnemyCemetery = enemyCemetery;
        }
        public void SetMyDeckInfo(IList<CardStatus> myDeck)
        {
            MyDeck = myDeck;
        }
        //--
        public void SetGameInfo(GameInfomation gameInfomation)//更新数值+胜场数据
        {
            MyRow1Point = gameInfomation.MyRow1Point;
            MyRow2Point = gameInfomation.MyRow2Point;
            MyRow3Point = gameInfomation.MyRow3Point;
            EnemyRow1Point = gameInfomation.EnemyRow1Point;
            EnemyRow2Point = gameInfomation.EnemyRow2Point;
            EnemyRow3Point = gameInfomation.EnemyRow3Point;
            IsMyPlayerPass = gameInfomation.IsMyPlayerPass;
            IsEnemyPlayerPass = gameInfomation.IsEnemyPlayerPass;
            MyWinCount = gameInfomation.MyWinCount;
            EnemyWinCount = gameInfomation.EnemyWinCount;
            EnemyName = gameInfomation.EnemyName;
            MyName = gameInfomation.MyName;
            MyDeckCount = gameInfomation.MyDeckCount;
            EnemyDeckCount = gameInfomation.EnemyDeckCount;
            MyHandCount = gameInfomation.MyHandCount;
            EnemyHandCount = gameInfomation.EnemyHandCount;
            MyCemeteryCount = gameInfomation.MyCemeteryCount;
            EnemyCemeteryCount = gameInfomation.EnemyCemeteryCount;
        }
        public void SetCardsInfo(GameInfomation gameInfomation)//更新卡牌类型数据
        {
            IsMyLeader = gameInfomation.IsMyLeader;
            IsEnemyLeader = gameInfomation.IsEnemyLeader;
            MyLeader = new List<CardStatus>() { gameInfomation.MyLeader };
            EnemyLeader = new List<CardStatus>() { gameInfomation.EnemyLeader };
            MyHandCard = gameInfomation.MyHandCard.ToList();
            MyStay = gameInfomation.MyStay.ToList();
            EnemyStay = gameInfomation.EnemyStay.ToList();
            EnemyHandCard = gameInfomation.EnemyHandCard.ToList();
            MyPlace = gameInfomation.MyPlace.Select(x => x.ToList()).ToArray();
            EnemyPlace = gameInfomation.EnemyPlace.Select(x => x.ToList()).ToArray();
            MyCemetery = gameInfomation.MyCemetery.ToList();
            EnemyCemetery = gameInfomation.EnemyCemetery.ToList();
        }
        public void SetCoinInfo(bool isBlueCoin)
        {
            IsMyTurn = isBlueCoin;
        }
        public void SetPointInfo(GameInfomation gameInfomation)
        {
            MyRow1Point = gameInfomation.MyRow1Point;
            MyRow2Point = gameInfomation.MyRow2Point;
            MyRow3Point = gameInfomation.MyRow3Point;
            EnemyRow1Point = gameInfomation.EnemyRow1Point;
            EnemyRow2Point = gameInfomation.EnemyRow2Point;
            EnemyRow3Point = gameInfomation.EnemyRow3Point;
        }
        public void SetCountInfo(GameInfomation gameInfomation)
        {
            MyDeckCount = gameInfomation.MyDeckCount;
            MyHandCount = gameInfomation.MyHandCount;
            MyCemeteryCount = gameInfomation.MyCemeteryCount;
            EnemyDeckCount = gameInfomation.EnemyDeckCount;
            EnemyHandCount = gameInfomation.EnemyHandCount;
            EnemyCemeteryCount = gameInfomation.EnemyCemeteryCount;
        }
        public void SetPassInfo(GameInfomation gameInfomation)
        {
            IsMyPlayerPass = gameInfomation.IsMyPlayerPass;
            IsEnemyPlayerPass = gameInfomation.IsEnemyPlayerPass;
        }
        public void SetWinCountInfo(GameInfomation gameInfomation)
        {
            MyWinCount = gameInfomation.MyWinCount;
            EnemyWinCount = gameInfomation.EnemyWinCount;
        }
        public void SetNameInfo(GameInfomation gameInfomation)
        {
            MyName = gameInfomation.MyName;
            EnemyName = gameInfomation.EnemyName;
        }
        //-------------------------------------------------------------------------------------------
        public void CardMove(MoveCardInfo info)//卡牌移动
        {
            var card = default(CardStatus);
            if (!info.Source.RowPosition.IsOnRow())
            {
                card = info.Card;
            }
            else
            {
                card = GetCard(info.Source);
                CardRemove(info.Source);
            }

            if (info.Target.RowPosition.IsOnRow())
            {
                var targetRow = GetRow(info.Target.RowPosition);
                targetRow.Insert(info.Target.CardIndex, card);
            }
        }

        public void CardRemove(CardLocation location)
        {
            var list = GetRow(location.RowPosition);
            list.Remove(GetCard(location));
        }

        public void CardRemove(RowPosition row, CardStatus item)
        {
            var list = GetRow(row);
            list.Remove(item);
        }
        //----------------------------------
        public void ShowWeatherApply(RowPosition row, RowStatus type)
        {
            if (row.IsMyRow())
            {
                MyRowStatus[row.MyRowToIndex()] = type;
            }
            else
            {
                EnemyRowStatus[row.Mirror().MyRowToIndex()] = type;
            }
        }

        public void ShowCardBreakEffect(CardLocation location, CardBreakEffectType type)
        {
            CardRemove(location);
        }

        public void ShowCardsToCemetery(GameCardsPart cards)
        {
            cards.MyHandCards.OrderByDescending(x => x).ForAll(x => MyHandCard.RemoveAt(x));
            cards.EnemyHandCards.OrderByDescending(x => x).ForAll(x => EnemyHandCard.RemoveAt(x));
            cards.MyStayCards.OrderByDescending(x => x).ForAll(x => MyStay.RemoveAt(x));
            cards.EnemyStayCards.OrderByDescending(x => x).ForAll(x => EnemyStay.RemoveAt(x));
            cards.MyRow1Cards.OrderByDescending(x => x).ForAll(x => MyPlace[0].RemoveAt(x));
            cards.MyRow2Cards.OrderByDescending(x => x).ForAll(x => MyPlace[1].RemoveAt(x));
            cards.MyRow3Cards.OrderByDescending(x => x).ForAll(x => MyPlace[2].RemoveAt(x));
            cards.EnemyRow1Cards.OrderByDescending(x => x).ForAll(x => EnemyPlace[0].RemoveAt(x));
            cards.EnemyRow2Cards.OrderByDescending(x => x).ForAll(x => EnemyPlace[1].RemoveAt(x));
            cards.EnemyRow3Cards.OrderByDescending(x => x).ForAll(x => EnemyPlace[2].RemoveAt(x));
        }

        public int MyPoint
        {
            get => MyRow1Point + MyRow2Point + MyRow3Point;
        }
        public int EnemyPoint
        {
            get => EnemyRow1Point + EnemyRow2Point + EnemyRow3Point;
        }

        public bool IsMustWin
        {
            get
            {
                if (EnemyWinCount == 1)
                {
                    return true;
                }
                return false;
            }
        }
    }
}