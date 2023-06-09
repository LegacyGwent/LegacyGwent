using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12013")]//兰伯特：剑术大师
    public class LambertSowrdmaster : CardEffect
    {//对一个敌军单位的所有同名牌造成4点伤害。
        public LambertSowrdmaster(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selected.TrySingle(out var target))
            {
                return 0;
            }
            var id = target.Status.CardId;
            var targetCards = Game.GetPlaceCards(AnotherPlayer).Where(x => x.Status.CardId == id);
            foreach (var card in targetCards)
            {
                await card.Effect.Damage(5, Card);
            }
            return 0;
        }
    }
}