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

            foreach (var card in cards)
            {
                if (Card.Status.HealthStatus < 2)
                {
                    break;
                }

                var row = (card.Status.CardRow.MyRowToIndex()).IndexToMyRow();
                var targetRow = TurnType.My.GetRow();
                targetRow.Remove(row.IsMyRow() ? row : row.Mirror());
                var canMoveRow = targetRow.Where(x => Game.RowToList(card.PlayerIndex, x).Count < Game.RowMaxCount);
                if (!canMoveRow.TryMessOne(out var target, Game.RNG))
                {
                    continue;
                }
                await card.Effect.Move(new CardLocation(target, Game.RowToList(card.PlayerIndex, target).Count), Card);
                await Game.ShowCardNumberChange(Card, -2, NumberType.Normal);
                Card.Status.HealthStatus -= 2;
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
