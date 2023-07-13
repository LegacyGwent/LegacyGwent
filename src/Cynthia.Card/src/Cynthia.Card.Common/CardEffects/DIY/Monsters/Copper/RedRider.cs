using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70083")]//红骑士
    public class RedRider : CardEffect, IHandlesEvent<AfterCardHurt>
    {//每当有敌军单位被“刺骨冰霜”摧毁时，从牌组召唤1张它的同名牌。
        public RedRider(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardHurt @event)
        {
            bool isBitingFrost = (@event.DamageType == DamageType.BitingFrost);
            if (Game.GameRound.ToPlayerIndex(Game) == AnotherPlayer && @event.Target.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInDeck() && isBitingFrost && @event.Target.IsDead)

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