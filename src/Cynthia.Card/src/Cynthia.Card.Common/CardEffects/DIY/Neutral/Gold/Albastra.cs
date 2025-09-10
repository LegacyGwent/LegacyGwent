using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70180")]//Deploy: Spawn Left and Right Wings. On turn start, if you have at least one wing, Boost self by 3, otherwise Destroy self. Immune.
    public class Albastra : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        public Albastra(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {   // Deploy: Spawn Left and Right Wings and give them 3 armor. Immune.
            // Gain immune status.
            Card.Status.IsImmue = true;
            // Spawn Left and Right Wings.
            var wing1 = await Game.CreateCard(CardId.AlbastraLeftWing, PlayerIndex, Card.GetLocation());
            await wing1.Effect.Armor(3, Card);
            var wing2 = await Game.CreateCard(CardId.AlbastraRightWing, PlayerIndex, Card.GetLocation() + 1);
            await wing2.Effect.Armor(3, Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            // On turn start, if you have at least one wing, Boost self by 3, otherwise Destroy self.
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;

            int wingcount = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex]).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.Status.CardId == CardId.AlbastraLeftWing || x.Status.CardId == CardId.AlbastraRightWing).ToList().Count();
            if (wingcount > 0)
            {
                await Card.Effect.Boost(3, Card);
                return;
            }
            await Card.Effect.ToCemetery(CardBreakEffectType.Scorch); // Destroy self if no wings are present.
            return;
        }
    }
}