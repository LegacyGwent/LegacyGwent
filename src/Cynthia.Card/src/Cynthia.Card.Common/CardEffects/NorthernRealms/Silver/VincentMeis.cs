using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43008")]//文森特·梅斯
    public class VincentMeis : CardEffect
    {//摧毁所有单位的护甲，获得被摧毁护甲数值一半的增益。
        public VincentMeis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //没有摧毁护甲api，用damage代替,增益值向下取整
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.Status.Armor >= 0).ToList();
            int damageint = 0;
            foreach (var card in cards)
            {
                damageint = damageint + card.Status.Armor;
                await card.Effect.Damage(card.Status.Armor, Card);
            }
            await Card.Effect.Boost(damageint / 2, Card);
            return 0;
        }
    }
}