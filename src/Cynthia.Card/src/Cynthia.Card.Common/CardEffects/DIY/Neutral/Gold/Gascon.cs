using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70032")]//Gascon
    public class Gascon : CardEffect, IHandlesEvent<AfterCardMove>
    {//将所有单位移至随机排，每移动1个单位，便受到2点伤害。若位于牌组或手牌：己方回合中，每有1个单位被改变所在排别时获得1点增益。
        public Gascon(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int moveCount = (Card.Status.Strength + Card.Status.HealthStatus - 1)/2;
            int i = 0;
            var cards = Game.GetPlaceCards(AnotherPlayer).ToList();
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
                i++;
                if (i >= moveCount)
                {
                    break;
                }
            }
            await Card.Effect.Damage(2*i, Card);
            return 0;
        }
        public async Task HandleEvent(AfterCardMove @event)
        {
            if (Game.GameRound.ToPlayerIndex(Game) == PlayerIndex 
                && (Card.Status.CardRow.IsInDeck() || Card.Status.CardRow.IsInHand())
                && @event.Target != @event.Source)
            {
                await Card.Effect.Boost(1, Card);
            }
            return;
        }
    }
}
