using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44001")]//崔丹姆步兵
    public class TridamInfantry : CardEffect, IHandlesEvent<OnGameStart>
    {//4点护甲。
        public TridamInfantry(GameCard card) : base(card) { }

        public async Task HandleEvent(OnGameStart @event)
        {
            await Card.Effect.Lock(Card);
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(4, Card);
            return 0;
        }
    }
}
