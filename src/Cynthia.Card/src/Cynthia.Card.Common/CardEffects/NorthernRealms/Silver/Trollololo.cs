using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43003")]//巨魔魔
    public class Trollololo : CardEffect, IHandlesEvent<AfterTurnStart>
    {//4点护甲，每回合开始时增加2点护甲。
        public Trollololo(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(4, Card);
            return 0;
        }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Armor(2, Card);
                return;
            }
        }
    }
}