using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24031")]//孽鬼
    public class Nekker : CardEffect, IHandlesEvent<AfterCardDeath>, IHandlesEvent<AfterCardConsume>
    {//若位于手牌、牌组或己方半场：友军单位发动吞噬时获得1点增益。 遗愿：召唤1张同名牌。
        public Nekker(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardConsume @event)
        {
            //如果并非友军吞噬, 并且位于正确位置的话,不触发效果
            if (!(@event.Source.PlayerIndex == PlayerIndex &&
                (Card.Status.CardRow.IsInDeck() || Card.Status.CardRow.IsInHand() || Card.Status.CardRow.IsOnPlace())))
                return;
            await Card.Effect.Boost(1, Card);
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            var nekkers = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.CardInfo().CardId == Card.CardInfo().CardId).ToList();
            if (nekkers.Count == 0) return;
            var targetNekker = nekkers.Mess(Game.RNG).First();
            await targetNekker.Effect.Summon(@event.DeathLocation, Card);
        }
    }
}