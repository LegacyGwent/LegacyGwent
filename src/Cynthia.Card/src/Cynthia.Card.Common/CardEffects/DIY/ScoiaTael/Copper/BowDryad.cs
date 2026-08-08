using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70114")]//长弓树精
    public class BowDryad : CardEffect, IHandlesEvent<AfterCardMove>
    {
        public BowDryad(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await DealDamage();
            return 0;
        }

        public async Task HandleEvent(AfterCardMove @event)
        {
            if (@event.Target == Card &&
                Card.Status.CardRow == RowPosition.MyRow2 &&
                (int)Game.GameRound == PlayerIndex)
            {
                await DealDamage();
            }
        }

        private async Task DealDamage()
        {
            var result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (result.TrySingle(out var target))
            {
                await target.Effect.Damage(4, Card);
            }
        }
    }
}
