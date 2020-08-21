using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12009")]//先祖麦酒
    public class AleOfTheAncestors : CardEffect, IHandlesEvent<AfterCardMove>//, IHandlesEvent<AfterCardDeath>
    {//在所在排洒下“黄金酒沫”。被移动时重复此能力。遗愿：在所在排洒下“黄金酒沫”。
        public AleOfTheAncestors(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            // await Game.ApplyWeather(PlayerIndex,Card.Status.CardRow,RowStatus.GoldenFroth);
            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
                .SetStatus<GoldenFrothStatus>();
            return 0;
        }

        // public async Task HandleEvent(AfterCardHurt @event)
        // {
        //     if (@event.Target != Card || !Card.Status.CardRow.IsOnPlace())
        //     {
        //         return;
        //     }
        //     await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
        //        .SetStatus<GoldenFrothStatus>();
        //     return;
        // }

        // public async Task HandleEvent(AfterCardDeath @event)
        // {
        //     if (@event.Target == Card)
        //     {
        //         await Game.GameRowEffect[PlayerIndex][@event.DeathLocation.RowPosition.MyRowToIndex()]
        //        .SetStatus<GoldenFrothStatus>();
        //     }
        // }

        public async Task HandleEvent(AfterCardMove @event)
        {
            if (@event.Target != Card)
            {
                return;
            }

            await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
                 .SetStatus<GoldenFrothStatus>();
        }

        // public async Task HandleEvent(AfterTurnOver @event)
        // {
        //     if (!(Card.Status.CardRow.IsOnPlace() && PlayerIndex == @event.PlayerIndex)) return;
        //     await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()]
        //         .SetStatus<GoldenFrothStatus>();
        //     return;
        // }
    }
}