using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70033")]//杰洛特：亚登法印
    public class WarElephant : CardEffect
    {//摧毁己方单排所有单位的护甲，并造成扣除护甲值的伤害。
        public WarElephant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int damagenum = 0;
            var list = Card.GetRangeCard(1, type: GetRangeType.HollowAll).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
            foreach (var card in list)
            {
                if (card.Status.Armor > 0)
                {
                    damagenum = damagenum + card.Status.Armor;
                    await card.Effect.Damage(card.Status.Armor, Card);
                }
            }
            if (Card.Status.Armor > 0)
            {
                damagenum = damagenum + Card.Status.Armor;
                await Card.Effect.Damage(Card.Status.Armor, Card);
            }
            var result2 = await Game.GetSelectPlaceCards(Card);
            if (result2.Count <= 0) return 0;
            await result2.Single().Effect.Damage(damagenum, Card);
            return 0;
        }
    }
}