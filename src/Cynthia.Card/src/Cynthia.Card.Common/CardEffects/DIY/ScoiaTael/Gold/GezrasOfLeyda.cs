using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.GezrasOfLeyda)]
    public class GezrasOfLeyda : CardEffect, IHandlesEvent<BeforeCardDamage>
    {
        private const int MoveLimit = 7;

        public GezrasOfLeyda(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = Card.Status.CardRow;
            if (!row.IsOnPlace()) return 0;

            var availableSpace = Math.Max(0, Game.RowMaxCount - Game.RowToList(PlayerIndex, row).Count);
            var candidates = Game.GetPlaceCards(PlayerIndex)
                .Where(target => target.Status.CardRow != row &&
                                 target.Status.Type == CardType.Unit &&
                                 !target.Status.IsImmue &&
                                 target.HasAnyCategorie(Categorie.Witcher, Categorie.Elf))
                .ToList();
            var selectCount = Math.Min(MoveLimit, Math.Min(availableSpace, candidates.Count));
            var moved = 0;
            if (selectCount > 0)
            {
                var selected = await Game.GetSelectMenuCards(
                    PlayerIndex, candidates, selectCount, "选择最多7个友军猎魔人或精灵移至此排", isCanOver: true);
                foreach (var target in selected)
                {
                    if (target.PlayerIndex != PlayerIndex || target.Status.CardRow == row) continue;
                    if (await target.Effect.Move(new CardLocation(row, int.MaxValue), Card)) moved++;
                }
            }

            var excess = MoveLimit - moved;
            if (excess > 0)
            {
                var selectedTarget = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
                if (selectedTarget.TrySingle(out var target))
                    await target.Effect.Weaken(excess, Card);
            }
            return 0;
        }

        public async Task HandleEvent(BeforeCardDamage @event)
        {
            if (Card.Status.CardRow.IsOnPlace() &&
                @event.DamageType.IsHazard() &&
                @event.Target.PlayerIndex == Card.PlayerIndex &&
                @event.Target.Status.CardRow == Card.Status.CardRow)
                @event.IsCancel = true;

            await Task.CompletedTask;
        }
    }
}
