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
            var armorTotal = 0;
            var cards = Card.GetRangeCard(1, type: GetRangeType.HollowAll)
                .Append(Card)
                .ToList();
            foreach (var card in cards)
            {
                if (card.Status.Armor > 0)
                {
                    var removedArmor = card.Status.Armor;
                    armorTotal += removedArmor;
                    card.Status.Armor = 0;
                    await Game.ShowCardIconEffect(card, CardIconEffectType.BreakArmor);
                    await Game.SendEvent(new AfterCardSubArmor(card, removedArmor, Card));
                    await Game.ShowSetCard(card);
                    await Game.SendEvent(new AfterCardArmorBreak(card, Card));
                }
            }
            if (armorTotal <= 0)
            {
                return 0;
            }
            var result2 = await Game.GetSelectPlaceCards(Card);
            if (result2.Count <= 0) return 0;
            await result2.Single().Effect.Damage(armorTotal, Card);
            return 0;
        }
    }
}
