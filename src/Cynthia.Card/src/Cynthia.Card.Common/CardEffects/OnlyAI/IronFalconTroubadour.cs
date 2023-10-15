using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("89006")]//铁隼吟游诗人 IronFalconTroubadour
    public class IronFalconTroubadour : CardEffect
    {//无。
        public IronFalconTroubadour(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var count = Game.PlayersDeck[Card.PlayerIndex].Count();

            for (var i = 0; i < 4 + count; i++)
            {
                if (!Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.Status.CardId != CardId.GasconIronFalcon).TryMessOne(out var target, Game.RNG))
                {
                    break;
                }
                await target.Effect.Boost(1, Card);
            }

            return 0;
        }
    }
}
