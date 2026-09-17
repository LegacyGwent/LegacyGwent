using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70134")]//鸦母德鲁伊 CrowClanDruid
    public class CrowClanDruid : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public CrowClanDruid(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await CreateCrowAtRight();
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            if (!Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Any(x => x.Status.CardId == CardId.Crow))
            {
                return;
            }

            await CreateCrowAtRight();
            await Card.Effect.Damage(1, Card);
        }

        private async Task CreateCrowAtRight()
        {
            if (!Card.Status.CardRow.IsOnPlace() ||
                Game.RowToList(PlayerIndex, Card.Status.CardRow).Count >= Game.RowMaxCount)
            {
                return;
            }

            await Game.CreateCardAtEnd(CardId.Crow, PlayerIndex, Card.Status.CardRow);
        }
    }
}
