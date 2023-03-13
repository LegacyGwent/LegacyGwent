using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12019")]//希里
    public class Ciri : CardEffect, IHandlesEvent<AfterRoundOver>
    {//己方输掉小局时返回手牌。
        public Ciri(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterRoundOver @event)
        {
            if (@event.WinPlayerIndex != AnotherPlayer || !Card.Status.CardRow.IsOnPlace()) return;
            Card.Effect.Repair(true);
            await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), Card);

        }
    }
}