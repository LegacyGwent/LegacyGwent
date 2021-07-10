using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;
namespace Cynthia.Card
{
    [CardEffectId("70077")]//火誓狂热者 FireswornZealot
    public class FireswornZealot : CardEffect, IHandlesEvent<AfterTurnOver>
    {//每4回合，在回合结束时对4个随机敌军单位造成2点伤害。场上每有一个被锁定的的单位，减少1次回合计数。
        public FireswornZealot(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 4);
            var cards = Game.GetAllCard(Card.PlayerIndex)
                .Where(x => x.Status.CardRow.IsOnPlace() && x.Status.IsLock==true).ToList();
            await Card.Effect.SetCountdown(offset: -1*cards.Count);
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    //重新倒计时
                    await Card.Effect.SetCountdown(value: 4);
                    var cards = Game.GetAllCard(Card.PlayerIndex)
                        .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex)
                        .Mess(RNG).Take(4).ToList();
                    foreach (var card in cards)
                    {
                        if (card.Status.CardRow.IsOnPlace())
                            await card.Effect.Damage(2, Card);
                    }
                }
            }
            return;
        }
    }
}