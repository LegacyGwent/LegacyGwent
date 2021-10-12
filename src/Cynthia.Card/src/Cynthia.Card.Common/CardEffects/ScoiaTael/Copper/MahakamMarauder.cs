using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54012")] //玛哈坎劫掠者
    public class MahakamMarauder : CardEffect, IHandlesEvent<AfterCardBoost>, IHandlesEvent<AfterCardHurt>, IHandlesEvent<AfterCardStrengthen>, IHandlesEvent<AfterCardWeaken>
    {
        //战力改变时（被重置除外），获得2点增益。
        public MahakamMarauder(GameCard card) : base(card)
        {
        }

        private async Task BoostMyself(GameCard target, GameCard source)
        {
            if (target == Card && source != Card && Card.Status.CardRow.IsOnPlace())
                await BoostMyself();
        }

        private async Task BoostMyself()
        {
            await Card.Effect.Boost(boost, Card);
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            await BoostMyself(@event.Target, @event.Source);
        }

        public async Task HandleEvent(AfterCardBoost @event)
        {
            await BoostMyself(@event.Target, @event.Source);
        }

        public async Task HandleEvent(AfterCardStrengthen @event)
        {
            await BoostMyself(@event.Target, @event.Source);
        }

        public async Task HandleEvent(AfterCardWeaken @event)
        {
            await BoostMyself(@event.Target, @event.Source);
        }

        private const int boost = 2;
    }
}