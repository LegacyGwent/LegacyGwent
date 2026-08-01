using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12017")]//杰洛特：猎魔大师
    public class GeraltProfessional : CardEffect
    {//对1个敌军单位造成4点伤害。若它为“怪兽”单位，则直接将其摧毁。
        public GeraltProfessional(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (cards.Count == 0) return 0;
            var card = cards.Single();
            if (card.Status.Faction == Faction.Monsters)
            {
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            else
            {
                await card.Effect.Damage(4, Card);
            }
            return 0;
        }
    }
}