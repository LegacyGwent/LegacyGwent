using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54006")] //先知
    public class Farseer : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardBoost>
    {
        //己方回合中，若有除自身外的友军单位或手牌中的单位获得增益，则回合结束时获得2点增益。
        public Farseer(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        private bool needBoost = false;

        public async Task HandleEvent(AfterCardBoost @event)
        {
            if (@event.Target.PlayerIndex == Card.PlayerIndex && Card != @event.Target && Card.Status.CardRow.IsOnPlace())
            {
                needBoost = true;
            }

            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == PlayerIndex)
                needBoost = false;

            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == PlayerIndex && needBoost)
            {
                await Card.Effect.Boost(boost);
            }
        }

        private const int boost = 2;
    }
}