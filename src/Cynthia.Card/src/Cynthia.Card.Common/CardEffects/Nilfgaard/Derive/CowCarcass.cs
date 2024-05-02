using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("35002")]//牛尸
    public class CowCarcass : CardEffect, IHandlesEvent<AfterTurnOver>
    {//间谍。2回合后，己方回合结束时，摧毁同排所有其他最弱的单位，并放逐自身。
        public CowCarcass(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.Status.Countdown > 0)
            {
                //await Game.Debug($"The cd of the cow corpse is reduced, before the cd was: {Card.Status.Countdown}, after that it will be reduced by 1").
                await Card.Effect.SetCountdown(offset: -1 );
                if (Card.Status.CardRow.IsOnPlace()) return;
                if (Card.Effect.Countdown <= 0)
                {//trigger effect
                    var list = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x != Card).WhereAllLowest().ToList();
                    foreach (var card in list)
                    {
                        await card.Effect.ToCemetery(CardBreakEffectType.Epidemic);
                    }
                    await Card.Effect.ToCemetery(CardBreakEffectType.Banish);
                }
            };
        }
    }
}