using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70032")]//Gascon
    public class Gascon : CardEffect, IHandlesEvent<AfterCardMove>
    {
        public Gascon(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectedrow = await Game.GetSelectRow(PlayerIndex, Card, TurnType.All.GetRow());
            var cards = Game.RowToList(AnotherPlayer, selectedrow).IgnoreConcealAndDead()
                .Concat(Game.RowToList(PlayerIndex, selectedrow).IgnoreConcealAndDead())
                .Where(card => card != Card)
                .ToList();

            var moved = 0;
            foreach (var card in cards)
            {
                var row = (card.Status.CardRow.MyRowToIndex()).IndexToMyRow();
                var targetRow = TurnType.My.GetRow();
                targetRow.Remove(row.IsMyRow() ? row : row.Mirror());
                var canMoveRow = targetRow.Where(x => Game.RowToList(card.PlayerIndex, x).Count < Game.RowMaxCount);
                if (!canMoveRow.TryMessOne(out var target, Game.RNG))
                {
                    continue;
                }
                await card.Effect.Move(new CardLocation(target, Game.RowToList(card.PlayerIndex, target).Count), Card);
                moved++;
            }

            // This is loss of Boost, not damage.  It cannot take Gascon below
            // base power, trigger damage reactions, or kill him.
            var boostLost = Math.Min(moved, Math.Max(0, Card.Status.HealthStatus));
            if (boostLost > 0)
            {
                await Game.ShowCardNumberChange(Card, -boostLost, NumberType.Normal);
                Card.Status.HealthStatus -= boostLost;
                await Game.ShowSetCard(Card);
                await Game.SetPointInfo();
            }
            return 0;
        }
        public async Task HandleEvent(AfterCardMove @event)
        {
            if (Game.GameRound.ToPlayerIndex(Game) != PlayerIndex ||
                !(Card.Status.CardRow.IsInDeck() || Card.Status.CardRow.IsInHand()) ||
                @event.Target == Card ||
                @event.Target.Status.Type != CardType.Unit ||
                !@event.Target.Status.IsAnyGroup(Group.Copper, Group.Silver))
            {
                return;
            }

            await Card.Effect.Boost(1, Card);
        }
    }
}
