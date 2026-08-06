using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70152")]//口莫拉汉姆家斟酒侍者 VanMoorlehemsCupbearer
    public class VanMoorlehemsCupbearer : CardEffect, IHandlesEvent<AfterTurnStart>
    {//每回合开始时，随机隐匿1张铜色手牌并使其获得1点增益。
        public VanMoorlehemsCupbearer(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var cards = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper)
                .ToList();
            if (!cards.TryMessOne(out var target, RNG))
            {
                return;
            }

            await target.Effect.Conceal(Card);
            await target.Effect.Boost(1, Card);
        }
    }
}
