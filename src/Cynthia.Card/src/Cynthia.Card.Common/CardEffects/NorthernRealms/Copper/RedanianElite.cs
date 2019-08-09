using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44006")]//瑞达尼亚精锐
    public class RedanianElite : CardEffect, IHandlesEvent<AfterCardArmorBreak>
    {//每当护甲归0，获得5点增益。 4点护甲。
        public RedanianElite(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(4, Card);
            return 0;
        }
        public async Task HandleEvent(AfterCardArmorBreak @event)
        {
            if (@event.Target == Card && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(4, Card);
            }
            return;
        }

    }
}