using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public static class GameFunctionalExtensions
    {
        public static GwentCard CardInfo(this GameCard card) => GwentMap.CardMap[card.Status.CardId];
        public static GwentCard CardInfo(this CardStatus card) => GwentMap.CardMap[card.CardId];
        public static GwentCard CardInfo(this string cardId) => GwentMap.CardMap[cardId];
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
        public static CardStatus CreateBackCard(this CardStatus card)=>new CardStatus(){IsCardBack = true, DeckFaction = card.DeckFaction};
        public static int CardPoint(this GameCard card) => card.Status.HealthStatus + card.Status.Strength;
        public static CardLocation Mirror(this CardLocation location)=>new CardLocation(){RowPosition = location.RowPosition.Mirror(),CardIndex = location.CardIndex};
        public static Task MoveToCardStayFirst(this GameCard card, bool isToEnemyStay = false, bool isShowToEnemy = true)//移动到卡牌移动区末尾
        {
            var game = card.Effect.Game;
            return game.ShowCardMove(new CardLocation() { RowPosition = (isToEnemyStay ? RowPosition.EnemyStay : RowPosition.MyStay), CardIndex = 0 }, card, isShowToEnemy);
        }
        public static IEnumerable<(int health, GameCard card)> SelectToHealth(this IEnumerable<GameCard> card)
        {
            return card.Select(x => (health: x.Status.Strength + x.Status.HealthStatus, card: x));
        }
        public static IEnumerable<GameCard> WhereAllHighest(this IEnumerable<GameCard> card)
        {
            //大到小
            if (card == null && card.Count() == 0) return card;
            var hight = card.SelectToHealth().OrderByDescending(x => x.health).First().health;
            return card.SelectToHealth().OrderByDescending(x => x.health).Where(x => x.health >= hight).Select(x => x.card);
        }
        public static IEnumerable<GameCard> WhereAllLowest(this IEnumerable<GameCard> card)
        {
            if (card == null && card.Count() == 0) return card;
            var low = card.SelectToHealth().OrderBy(x => x.health).First().health;
            return card.SelectToHealth().OrderBy(x => x.health).Where(x => x.health <= low).Select(x => x.card);
        }
        public static IEnumerable<CardLocation> CardsPartToLocation(this GameCardsPart part)
        {
            var locations = part.MyRow1Cards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.MyRow1}).
            Concat(part.MyRow2Cards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.MyRow2})).
            Concat(part.MyRow3Cards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.MyRow3})).
            Concat(part.EnemyRow1Cards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.EnemyRow1})).
            Concat(part.EnemyRow2Cards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.EnemyRow2})).
            Concat(part.EnemyRow3Cards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.EnemyRow3})).
            Concat(part.MyHandCards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.MyHand})).
            Concat(part.MyStayCards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.MyStay})).
            Concat(part.EnemyHandCards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.EnemyHand})).
            Concat(part.EnemyStayCards.Select(x=>new CardLocation(){CardIndex=x,RowPosition=RowPosition.EnemyStay}));
            if(part.IsSelectMyLeader)
                locations.Append(new CardLocation(){CardIndex=0,RowPosition=RowPosition.MyLeader});
            if(part.IsSelectEnemyLeader)
                locations.Append(new CardLocation(){CardIndex=0,RowPosition=RowPosition.EnemyLeader});
            return locations;
        }
    }
}