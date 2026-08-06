using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24021")]//寒冰巨人
    public class IceGiant : CardEffect, IHandlesEvent<AfterWeatherApply>
    {
        private const int FrostBoost = 3;

        public IceGiant(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var frostCount = Game.GameRowEffect
                .SelectMany(playerRows => playerRows.Select(row => row.RowStatus))
                .Count(status => status == RowStatus.BitingFrost);
            await Boost(frostCount * FrostBoost, Card);
            return 0;
        }

        public async Task HandleEvent(AfterWeatherApply @event)
        {
            if (Card.Status.CardRow.IsOnPlace() && @event.Type == RowStatus.BitingFrost)
            {
                await Boost(FrostBoost, Card);
            }
        }
    }
}
