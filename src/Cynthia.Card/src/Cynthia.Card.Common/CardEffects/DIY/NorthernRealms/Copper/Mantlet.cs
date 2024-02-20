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
                    if (i > 0)
                    {
                        await Card.Effect.Armor(6, Card);
                    }
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

            var currentRow = Card.Status.CardRow;
            var arrornum = Card.Status.Armor;
            var taget = Card.GetRangeCard(1, GetRangeType.HollowRight);
            if (taget.Count() == 0 || taget.Single().Status.Conceal)
            {
                return;
            }
            if (@event.Target == taget.Single() && @event.Target.PlayerIndex == Card.PlayerIndex && @event.Target != Card && arrornum > 0)
            {
                if(@event.Num >= arrornum)
                {
                    @event.Num=@event.Num-arrornum;
                    await Card.Effect.Damage(arrornum, Card);
                }
                if(@event.Num < arrornum)
                {
                   
                    await Card.Effect.Damage(@event.Num, Card);
                    @event.Num=0;
                }
                return;
            }
            await Task.CompletedTask;
            return;
        }
    }
}
