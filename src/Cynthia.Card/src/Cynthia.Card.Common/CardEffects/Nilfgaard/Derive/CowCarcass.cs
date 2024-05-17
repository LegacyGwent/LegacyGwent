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
                //await Game.Debug($"牛尸的cd减少啦,之前cd为:{Card.Status.Countdown},之后会在基础上减少1");
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.CardPoint() > 0) // we need to chek if it's still alive after updating the countdown
                {
                    if (Card.Effect.Countdown <= 0)
                    {//触发效果
                        var list = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x != Card).WhereAllLowest().ToList();
                        foreach (var card in list)
                        {
                            await card.Effect.ToCemetery(CardBreakEffectType.Epidemic);
                        }
                        await Card.Effect.ToCemetery(CardBreakEffectType.Banish);
                    }
                }
            };
        }
    }
}