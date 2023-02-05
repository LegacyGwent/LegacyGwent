using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22009")]//伊勒瑞斯：临终之日
    public class ImlerithSabbath : CardEffect, IHandlesEvent<AfterTurnOver>
    {//2点护甲。 回合结束时，与最强的敌军单位对决。若存活，则回复2点战力并获得2点护甲。一共可生效3次。
        public ImlerithSabbath(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 3);
            await Armor(2, Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace() || Card.Status.Countdown < 1)
            {
                return;
            }
            if (!Game.GetPlaceCards(AnotherPlayer).WhereAllHighest().TryMessOne(out var target, Game.RNG))
            {
                return;
            }
            await Duel(target, Card);
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.IsDead || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            await Armor(2, Card);
            await Reply(2, Card);
        }
    }
}