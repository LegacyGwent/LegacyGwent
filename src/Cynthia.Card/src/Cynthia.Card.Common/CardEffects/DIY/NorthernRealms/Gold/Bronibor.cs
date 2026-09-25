using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Bronibor)]
    public class Bronibor : CardEffect, IHandlesEvent<AfterUnitDown>
    {
        private bool _isUsed;

        public Bronibor(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(CardId.PoorFIngInfantry, PlayerIndex,
                new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.Status.CardId != CardId.PoorFIngInfantry ||
                @event.Target.PlayerIndex != PlayerIndex ||
                _isUsed ||
                !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _isUsed = true;
            var selectedRow = await Game.GetSelectRow(PlayerIndex, Card, TurnType.My.GetRow());
            var soldiers = Game.RowToList(PlayerIndex, selectedRow)
                .IgnoreConcealAndDead()
                .Where(x => x.HasAnyCategorie(Categorie.Soldier))
                .ToList();
            foreach (var soldier in soldiers)
            {
                await soldier.Effect.Armor(1, Card);
            }

            if (soldiers.Count == 0)
            {
                return;
            }

            var targets = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (targets.TrySingle(out var target))
            {
                await target.Effect.Damage(soldiers.Count, Card);
            }
        }
    }
}
