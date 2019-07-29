using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23003")]//哥亚特
    public class Golyat : CardEffect, IHandlesEvent<AfterCardHeal>
    {//获得7点增益。 每次被伤害时，额外承受2点伤害。
        public Golyat(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardHeal @event)
        {
            if (@event.Target != Card || @event.Source == Card)
            {
                return;
            }
            await Card.Effect.Damage(2, Card);
        }
    }
}