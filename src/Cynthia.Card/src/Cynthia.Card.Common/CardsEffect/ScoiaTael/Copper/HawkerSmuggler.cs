using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54010")] //私枭走私者
    public class HawkerSmuggler : CardEffect, IHandlesEvent<AfterUnitPlay>
    {
        //每有1个敌军单位被打出，便获得1点增益。
        public HawkerSmuggler(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.PlayerIndex != Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(boost);
            }
        }

        private int boost = 1;
    }
}