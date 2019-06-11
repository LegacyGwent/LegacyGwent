using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public static class GameFunctionalExtensions
    {
        public static IEnumerable<GwentCard> SelectCard(this IEnumerable<GwentCard> cards, Func<GwentCard, bool> sizer, bool isDistinct = false)
        {
            if (isDistinct)
                return cards.Where(sizer).Distinct();
            return cards.Where(sizer);

        }
        public static IList<GwentCard> GetMyBaseDeck(this GameCard card, Func<GwentCard, bool> sizer = null, bool isDistinct = false)
        {
            sizer = sizer ?? (x => true);
            //是否去除重复项,筛选器
            return card.Effect.Game.PlayerBaseDeck[card.PlayerIndex].Deck.SelectCard(sizer, isDistinct).ToList();
        }
        public static IList<GwentCard> GetEnemyBaseDeck(this GameCard card, Func<GwentCard, bool> sizer = null, bool isDistinct = false)
        {
            sizer = sizer ?? (x => true);
            //是否去除重复项,筛选器
            return card.Effect.Game.PlayerBaseDeck[card.Effect.Game.AnotherPlayer(card.PlayerIndex)].Deck.SelectCard(sizer, isDistinct).ToList();
        }
        public static IList<GwentCard> GetBaseDeck(this GameCard card, Func<GwentCard, bool> sizer = null, bool isDistinct = false)
        {
            sizer = sizer ?? (x => true);
            //是否去除重复项,筛选器
            return card.Effect.Game.PlayerBaseDeck[card.Effect.Game.AnotherPlayer(card.PlayerIndex)].Deck
                .Concat(card.Effect.Game.PlayerBaseDeck[card.PlayerIndex].Deck)
                .SelectCard(sizer, isDistinct).ToList();
        }
        public static GameDeck ToGameDeck(this DeckModel deck)
        {
            var result = new GameDeck();
            result.Id = deck.Id;
            result.Name = deck.Name;
            result.Leader = deck.Leader.CardInfo();
            result.Deck = deck.Deck.Select(x => x.CardInfo()).ToList();
            return result;
        }
        public static Task<int> CreateAndMoveStay(this GameCard card, params string[] cards)
        {
            return card.Effect.Game.CreateAndMoveStay(card.PlayerIndex, cards);
        }
        public static Task<int> CreateAndMoveStay(this GameCard card, IList<string> cards)
        {
            return card.Effect.Game.CreateAndMoveStay(card.PlayerIndex, cards.ToArray());
        }
        public static async Task<int> GetMenuSwitch(this GameCard card, params (string title, string message)[] cards)
        {
            return (await card.GetMenuSwitch(1, cards)).Single();
        }
        public static Task<IList<int>> GetMenuSwitch(this GameCard card, int switchCount, params (string title, string message)[] cards)
        {
            var cardList = cards.Select(x => new CardStatus(card.Status.CardId) { Name = x.title, Info = x.message }).ToList();
            return card.Effect.Game.GetSelectMenuCards(card.PlayerIndex, cardList, selectCount: switchCount, title: "选择一个选项");
        }
        public static GwentCard CardInfo(this GameCard card) => GwentMap.CardMap[card.Status.CardId];
        public static GwentCard CardInfo(this CardStatus card) => GwentMap.CardMap[card.CardId];
        public static GwentCard CardInfo(this string cardId) => GwentMap.CardMap[cardId];
        public static CardLocation GetLocation(this GameCard card, int playerIndex)
        {
            return card.Effect.Game.GetCardLocation(playerIndex, card);
        }
        public static CardLocation GetLocation(this GameCard card)
        {
            return card.Effect.Game.GetCardLocation(card.PlayerIndex, card);
        }
        public static IList<RowPosition> GetRow(this TurnType type)
        {
            var result = new List<RowPosition>();
            if (type == TurnType.My || type == TurnType.All)
            {
                result.Add(RowPosition.MyRow1);
                result.Add(RowPosition.MyRow2);
                result.Add(RowPosition.MyRow3);
            }
            if (type == TurnType.Enemy || type == TurnType.All)
            {
                result.Add(RowPosition.EnemyRow1);
                result.Add(RowPosition.EnemyRow2);
                result.Add(RowPosition.EnemyRow3);
            }
            return result;
        }
        public static IList<GameCard> GetRangeCard(this GameCard card, int range, GetRangeType type = GetRangeType.CenterAll)
        {//按照从左到右的顺序,选中卡牌
            var rowList = card.GetRowList();
            var centerIndex = card.GetRowIndex();
            var result = new List<GameCard>();
            for (var i = centerIndex - range; i <= centerIndex + range; i++)
            {
                if ((i >= 0 && (i < rowList.Count())) &&
                ((i < centerIndex && type.IsLeft()) ||
                (i == centerIndex && type.IsCenter()) ||
                (i > centerIndex && type.IsRight())))
                {
                    result.Add(rowList[i]);
                }
            }
            return result;
        }
        public static bool IsLeft(this GetRangeType type)
        {
            switch (type)
            {
                case GetRangeType.CenterAll:
                case GetRangeType.HollowAll:
                case GetRangeType.CenterLeft:
                case GetRangeType.HollowLeft:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsCenter(this GetRangeType type)
        {
            switch (type)
            {
                case GetRangeType.CenterAll:
                case GetRangeType.CenterLeft:
                case GetRangeType.CenterRight:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsRight(this GetRangeType type)
        {
            switch (type)
            {
                case GetRangeType.CenterAll:
                case GetRangeType.HollowAll:
                case GetRangeType.CenterRight:
                case GetRangeType.HollowRight:
                    return true;
                default:
                    return false;
            }
        }
        public static IList<GameCard> GetRowList(this GameCard card)
        {//按照从左到右的顺序,选中卡牌
            return card.Effect.Game.RowToList(card.PlayerIndex, card.Status.CardRow);
        }
        public static int GetRowIndex(this GameCard card)
        {//按照从左到右的顺序,选中卡牌
            return card.Effect.Game.RowToList(card.PlayerIndex, card.Status.CardRow).IndexOf(card);
        }
        public static bool IsShowBack(this GameCard card, int playerIndex)
        {
            if (!card.Status.CardRow.IsOnRow()) return true;
            if (card.PlayerIndex == playerIndex && card.Status.CardRow.IsOnPlace() && card.Status.Conceal)
                return true;
            if (card.PlayerIndex != playerIndex)
            {
                if (card.Status.CardRow.IsInHand() && !card.Status.IsReveal)
                    return true;
                if (card.Status.CardRow.IsOnPlace() && card.Status.Conceal)
                    return true;
            }
            return false;
        }
        public static bool TagetIsShowBack(this GameCard card, CardLocation taget, int tagetPlayerIndex, int LookplayerIndex)
        {
            if (!taget.RowPosition.IsOnRow()) return true;
            if (tagetPlayerIndex == LookplayerIndex && taget.RowPosition.IsOnPlace() && card.Status.Conceal)
                return true;
            if (tagetPlayerIndex != LookplayerIndex)
            {
                if (taget.RowPosition.IsInHand() && !card.Status.IsReveal)
                    return true;
                if (taget.RowPosition.IsOnPlace() && card.Status.Conceal)
                    return true;
            }
            return false;
        }
        public static CardStatus CreateBackCard(this CardStatus card) => new CardStatus() { IsCardBack = true, DeckFaction = card.DeckFaction };
        public static int CardPoint(this GameCard card) => card.Status.HealthStatus + card.Status.Strength;
        public static CardLocation Mirror(this CardLocation location) => new CardLocation() { RowPosition = location.RowPosition.Mirror(), CardIndex = location.CardIndex };
        public static SelectModeType Mirror(this SelectModeType selectMode)
        {
            switch (selectMode)
            {
                case SelectModeType.All:
                    return SelectModeType.All;
                case SelectModeType.AllHand:
                    return SelectModeType.AllHand;
                case SelectModeType.AllRow:
                    return SelectModeType.AllRow;
                case SelectModeType.Enemy:
                    return SelectModeType.My;
                case SelectModeType.EnemyHand:
                    return SelectModeType.MyHand;
                case SelectModeType.EnemyRow:
                    return SelectModeType.MyRow;
                case SelectModeType.My:
                    return SelectModeType.Enemy;
                case SelectModeType.MyHand:
                    return SelectModeType.EnemyHand;
                case SelectModeType.MyRow:
                    return SelectModeType.EnemyRow;
                default:
                    return SelectModeType.All;
            }
        }
        public static Task MoveToCardStayFirst(this GameCard card, bool isToEnemyStay = false, bool isShowToEnemy = true)//移动到卡牌移动区末尾
        {   //将卡牌移动到最开头
            var game = card.Effect.Game;
            return game.ShowCardMove(new CardLocation() { RowPosition = (isToEnemyStay ? RowPosition.EnemyStay : RowPosition.MyStay), CardIndex = 0 }, card, isShowToEnemy);
        }
        public static IEnumerable<(int health, GameCard card)> SelectToHealth(this IEnumerable<GameCard> card)
        {   //将所有卡牌的有效战力列出来
            return card.Select(x => (health: x.Status.Strength + x.Status.HealthStatus, card: x));
        }
        public static (int health, GameCard card) ToHealth(this GameCard card)
        {   //将所有卡牌的有效战力列出来
            return (health: card.Status.Strength + card.Status.HealthStatus, card: card);
        }
        public static IEnumerable<GameCard> WhereAllHighest(this IEnumerable<GameCard> card)
        {
            //大到小
            if (card == null || card.Count() == 0) return card;
            var hight = card.SelectToHealth().OrderByDescending(x => x.health).First().health;
            return card.SelectToHealth().OrderByDescending(x => x.health).Where(x => x.health >= hight).Select(x => x.card);
        }
        public static IEnumerable<GameCard> WhereAllLowest(this IEnumerable<GameCard> card)
        {
            if (card == null || card.Count() == 0) return card;
            var low = card.SelectToHealth().OrderBy(x => x.health).First().health;
            return card.SelectToHealth().OrderBy(x => x.health).Where(x => x.health <= low).Select(x => x.card);
        }
        public static IEnumerable<CardLocation> CardsPartToLocation(this GameCardsPart part)
        {
            var locations = part.MyRow1Cards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.MyRow1 }).
            Concat(part.MyRow2Cards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.MyRow2 })).
            Concat(part.MyRow3Cards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.MyRow3 })).
            Concat(part.EnemyRow1Cards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.EnemyRow1 })).
            Concat(part.EnemyRow2Cards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.EnemyRow2 })).
            Concat(part.EnemyRow3Cards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.EnemyRow3 })).
            Concat(part.MyHandCards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.MyHand })).
            Concat(part.MyStayCards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.MyStay })).
            Concat(part.EnemyHandCards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.EnemyHand })).
            Concat(part.EnemyStayCards.Select(x => new CardLocation() { CardIndex = x, RowPosition = RowPosition.EnemyStay }));
            if (part.IsSelectMyLeader)
                locations.Append(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.MyLeader });
            if (part.IsSelectEnemyLeader)
                locations.Append(new CardLocation() { CardIndex = 0, RowPosition = RowPosition.EnemyLeader });
            return locations;
        }
    }
}