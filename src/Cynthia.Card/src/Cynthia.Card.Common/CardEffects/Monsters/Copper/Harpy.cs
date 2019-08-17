using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24030")]//鹰身女妖
    public class Harpy : CardEffect, IHandlesEvent<AfterCardDeath>
    {//每当1个友军“野兽”单位在己方回合被摧毁，便召唤1张同名牌。
        public Harpy(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (Game.GameRound.ToPlayerIndex(Game) == PlayerIndex && @event.Target.HasAllCategorie(Categorie.Beast) && @event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
            {
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                if (list.Count() == 0)
                {
                    return;
                }
                //只召唤最后一个
                if (Card == list.Last())
                {
                    await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                }

                return;
            }

            return;
        }

    }
}