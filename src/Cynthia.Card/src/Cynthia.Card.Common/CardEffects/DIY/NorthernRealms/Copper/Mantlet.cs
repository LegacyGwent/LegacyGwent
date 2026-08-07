using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70130")]//防盾 Mantlet
    public class Mantlet : CardEffect, IHandlesEvent<BeforeCardDamage>
    {//同排友军单位受到伤害时，优先抵扣自身护甲，驱动：获得6点护甲。
        public Mantlet(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Card.Status.CardRow.IsOnPlace())
            {
                for (var i = 0; i < 1 + Card.GetCrewedCount(); i++)
                {
                    await Card.Effect.Armor(6, Card);
                }
            }
            return 0;
        }
        public async Task HandleEvent(BeforeCardDamage @event)
        {
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var armor = Card.Status.Armor;
            var adjacentCards = Card.GetRangeCard(1, GetRangeType.HollowAll);
            if (!adjacentCards.Contains(@event.Target))
            {
                return;
            }
            if (@event.Target.PlayerIndex == Card.PlayerIndex && @event.Target != Card && armor > 0)
            {
                var absorbed = System.Math.Min(@event.Num, armor);
                @event.Num -= absorbed;
                Card.Status.Armor -= absorbed;
                await Game.ShowCardIconEffect(Card, CardIconEffectType.BreakArmor);
                await Game.SendEvent(new AfterCardSubArmor(Card, absorbed, @event.Source));
                await Game.ShowSetCard(Card);
                if (Card.Status.Armor == 0)
                {
                    await Game.SendEvent(new AfterCardArmorBreak(Card, @event.Source));
                }
                return;
            }
            await Task.CompletedTask;
            return;
        }
    }
}
