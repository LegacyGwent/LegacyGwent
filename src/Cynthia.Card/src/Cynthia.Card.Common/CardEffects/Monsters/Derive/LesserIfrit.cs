using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("25006")]//次级伊夫利特
    public class LesserIfrit : CardEffect
    {//对1个敌军随机单位造成1点伤害。
        public LesserIfrit(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            await DamageRandomEnemy();
        }
        private async Task DamageRandomEnemy()
        {
            var cards = Game.GetPlaceCards(AnotherPlayer);
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(1, Card);
        }
    }
}