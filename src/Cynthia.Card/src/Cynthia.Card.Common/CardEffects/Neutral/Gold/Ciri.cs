using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12019")]//希里
    public class Ciri : CardEffect, IHandlesEvent<AfterRoundOver>
    {//获得护盾。己方输掉小局时返回手牌。 2点护甲。
        public Ciri(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            Card.Status.IsShield = true;
            return 0;
        }

        public async Task HandleEvent(AfterRoundOver @event)
        {
            if (@event.WinPlayerIndex != AnotherPlayer || !Card.Status.CardRow.IsOnPlace()) return;
            Card.Effect.Repair(true);
            await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), Card);

        }
    }
}