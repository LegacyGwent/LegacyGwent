using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70007")]//先知雷比欧达

    public class ProphetLebioda : CardEffect, IHandlesEvent<BeforeCardBanish>, IHandlesEvent<AfterPlayerPass>
    {//被放逐或对方放弃跟牌时，使己方半场所有友军单位获得1点增益。
        public ProphetLebioda(GameCard card) : base(card) { }

        private const int boostPoint = 1;
        public async Task HandleEvent(BeforeCardBanish @event)
        {
            if (@event.Target == Card)
            {
                await BoostAllies();
            }
        }

        public async Task HandleEvent(AfterPlayerPass @event)
        {
            if (@event.PlayerIndex == AnotherPlayer && Card.IsAliveOnPlance())
            {
                await BoostAllies();
            }
        }

        private async Task BoostAllies()
        {
            var cards = Game.GetPlaceCards(PlayerIndex)
                .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == PlayerIndex)
                .ToList();
            foreach (var card in cards)
            {
                await card.Effect.Boost(boostPoint, Card);
            }
        }
    }
}
