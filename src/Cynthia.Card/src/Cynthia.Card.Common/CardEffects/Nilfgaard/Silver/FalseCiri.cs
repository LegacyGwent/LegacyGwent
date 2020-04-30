using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("33002")]//冒牌希里
    public class FalseCiri : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterPlayerPass>, IHandlesEvent<AfterCardDeath>
    {
        public FalseCiri(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target == Card)
            {
                var row = Game.RowToList(@event.Target.PlayerIndex, @event.DeathLocation.RowPosition).IgnoreConcealAndDead();
                var cards = row.WhereAllLowest().ToList();
                for (var i = 0; i < cards.Count; i++)
                {
                    await cards[i].Effect.ToCemetery(CardBreakEffectType.Scorch);
                }
            }
        }

        public async Task HandleEvent(AfterPlayerPass @event)
        {
            if (Card.PlayerIndex == @event.PlayerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Charm(Card);
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (Card.PlayerIndex == @event.PlayerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Boost(1, Card);
        }
    }
}