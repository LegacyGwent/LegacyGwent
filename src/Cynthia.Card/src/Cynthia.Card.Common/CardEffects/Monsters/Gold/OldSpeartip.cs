using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22003")]//老矛头
    public class OldSpeartip : CardEffect
    {//对最多5个敌军同排单位造成2点伤害。
        public OldSpeartip(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            foreach (var card in Game.GetPlaceCards(AnotherPlayer, Card.Status.CardRow).Mess(Game.RNG).Take(5))
            {
                await card.Effect.Damage(2, Card);
            }
            return 0;
        }
    }
}