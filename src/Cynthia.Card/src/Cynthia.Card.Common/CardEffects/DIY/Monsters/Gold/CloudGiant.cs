using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70170")]// CloudGiant
    public class CloudGiant : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        private int _immuneTurns;

        public CloudGiant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            _immuneTurns = Game.GameRowEffect[AnotherPlayer]
                .Count(row => row.RowStatus == RowStatus.ImpenetrableFog);
            Card.Status.IsImmue = _immuneTurns > 0;
            await Card.Effect.Resilience(Card);
            await Game.ShowSetCard(Card);
            return 0;            
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _immuneTurns = System.Math.Max(0, _immuneTurns - 1);
            if (_immuneTurns == 0 && Card.Status.IsImmue)
            {
                Card.Status.IsImmue = false;
                await Game.ShowSetCard(Card);
            }
        }
    }
}
