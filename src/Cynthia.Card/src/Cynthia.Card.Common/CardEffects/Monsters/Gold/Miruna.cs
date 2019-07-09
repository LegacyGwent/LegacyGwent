using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22011")]//米卢娜
    public class Miruna : CardEffect, IHandlesEvent<AfterTurnStart>
    {//2回合后的回合开始时：魅惑对方同排最强的单位。
        public Miruna(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await SetCountdown(2);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if (Card.Status.IsCountdown)
            {
                await SetCountdown(offset: -1);
            }
            if (Countdown <= 0)
            {
                Card.Status.IsCountdown = false;
                var row = Card.Status.CardRow;
                if (!Game.GetPlaceCards(AnotherPlayer, row).WhereAllHighest().TryMessOne(out var target, Game.RNG))
                {
                    return;
                }
                await target.Effect.Charm(Card);
            }
        }
    }
}