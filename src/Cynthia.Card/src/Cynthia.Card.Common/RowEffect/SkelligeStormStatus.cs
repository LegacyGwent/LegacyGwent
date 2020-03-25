using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //史凯利杰风暴
    public class SkelligeStormStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.SkelligeStorm;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (PlayerIndex != @event.PlayerIndex) return;

            var cards = RowCards.Take(3).ToList();
            if (cards.Count > 0)
            {
                if (!cards[0].Status.Conceal)
                    await cards[0].Effect.Damage(2, null, damageType: DamageType.SkelligeStorm);
                if (cards.Count > 1)
                {
                    if (!cards[1].Status.Conceal)
                        await cards[1].Effect.Damage(1, null, damageType: DamageType.SkelligeStorm);
                    if (cards.Count > 2)
                    {
                        if (!cards[2].Status.Conceal)
                            await cards[2].Effect.Damage(1, null, damageType: DamageType.SkelligeStorm);
                    }
                }
            }
        }
    }
}