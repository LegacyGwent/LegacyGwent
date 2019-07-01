using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33023")]//奴隶贩子
    public class SlaveDriver : CardEffect
    {//将一个友军单位的战力降至1点，并对一个敌军单位造成伤害，数值等同于该友方单位所失去的战力。
        public SlaveDriver(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = await Game.GetSelectPlaceCards(Card, filter: x => x.CardPoint() > 1, selectMode: SelectModeType.MyRow);
            if (cards.Count == 0) return 0;
            var card = cards.Single();
            var point = card.CardPoint() - 1;
            await card.Effect.Damage(point, Card, isPenetrate: true);
            cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (cards.Count == 0) return 0;
            await cards.Single().Effect.Damage(point, Card);
            return 0;
        }
    }
}