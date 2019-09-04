using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53008")]//玛丽娜
    public class Malena : CardEffect, IHandlesEvent<BeforeUnitPlay>, IHandlesEvent<AfterTurnStart>
    {//伏击：2回合后的回合开始时：翻开，在战力不超过5点的铜色/银色敌军单位中魅惑其中最强的一个。
        public Malena(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.IsAliveOnPlance() && Card.Status.IsCountdown && Card.Status.Conceal)
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    await Ambush(async () =>
                    {
                        var targets = Game.GetPlaceCards(AnotherPlayer).FilterCards(type: CardType.Unit, filter: x => x.CardPoint() <= 5 && x.IsAnyGroup(Group.Copper, Group.Silver));
                        if (!targets.TryMessOne(out var target, Game.RNG))
                        {
                            return;
                        }
                        await target.Effect.Charm(Card);
                    });
                }
            }
        }

        public async Task HandleEvent(BeforeUnitPlay @event)
        {
            if (@event.PlayedCard != Card)
            {
                return;
            }
            await Card.Effect.SetCountdown(value: 2);
            await Card.Effect.PlanceConceal(Card);
            return;
        }
    }
}