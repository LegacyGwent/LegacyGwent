using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44024")]//科德温亡魂
    public class KaedweniRevenant : CardEffect, IHandlesEvent<BeforeSpecialPlay>
    {//己方打出下一张“法术”或“道具”牌时，在所在排生成1张自身的佚亡原始同名牌。 1点护甲。
        public KaedweniRevenant(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(1, Card);
            return;
        }
        public async Task HandleEvent(BeforeSpecialPlay @event)
        {
            if (@event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.Status.Countdown >= 1)
            {
                await Card.Effect.SetCountdown(offset: -1);
                await Game.CreateCard(CardId.KaedweniRevenant, PlayerIndex, Card.GetLocation() + 1, x => x.IsDoomed = true);
            }
            return;
        }
    }
}