using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63001")]//茱塔·迪门
    public class JuttaAnDimun : CardEffect
    {//对自身造成1点伤害。
        public JuttaAnDimun(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!cards.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Damage(1, Card);
            await Card.Effect.Damage(1, Card);
            return 0;
        }
    }
}
