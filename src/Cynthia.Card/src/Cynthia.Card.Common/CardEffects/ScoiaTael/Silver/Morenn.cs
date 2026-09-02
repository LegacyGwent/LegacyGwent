using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53006")]//莫丽恩
    public class Morenn : CardEffect, IHandlesEvent<BeforeUnitPlay>, IHandlesEvent<AfterUnitDown>
    {//伏击：在下个单位从任意方手牌打出至对方半场时翻开，对它造成7点伤害。
        public Morenn(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.PlayerIndex == PlayerIndex || !Card.IsAliveOnPlance() || !@event.IsFromHand)
            {
                return;
            }
            await Card.Effect.Ambush(async () =>
            {
                await Game.AddTask(async () =>
                {
                    var wasDoomed = @event.Target.Status.IsDoomed;
                    @event.Target.Status.IsDoomed = true;
                    await @event.Target.Effect.Damage(7, Card);
                    if (@event.Target.CardPoint() > 0)
                    {
                        @event.Target.Status.IsDoomed = wasDoomed;
                    }
                });
            });
        }


        public async Task HandleEvent(BeforeUnitPlay @event)
        {
            if (@event.PlayedCard != Card)
            {
                return;
            }
            await Card.Effect.PlanceConceal(Card);
            return;
        }
    }
}
