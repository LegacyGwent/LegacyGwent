using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12009")]//先祖麦酒
    public class AleOfTheAncestors : CardEffect, IHandlesEvent<AfterCardHurt>, IHandlesEvent<AfterCardMove>
    {//在所在排洒下“黄金酒沫”。被移动或伤害时重复此能力。
        public AleOfTheAncestors(GameCard card) : base(card) { }
        
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            // await Game.ApplyWeather(PlayerIndex,Card.Status.CardRow,RowStatus.GoldenFroth);
            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
                .SetStatus<GoldenFrothStatus>();
            return 0;
        }
        
        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (@event.Target != Card || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
               .SetStatus<GoldenFrothStatus>();
            return;
        }

        public async Task HandleEvent(AfterCardMove @event)
        {
            if (@event.Target == Card)
            {
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
               .SetStatus<GoldenFrothStatus>();
            }
        }

        /*
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && PlayerIndex == @event.PlayerIndex)) return;
            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
                .SetStatus<GoldenFrothStatus>();
            return;
        }
        */
    }
}