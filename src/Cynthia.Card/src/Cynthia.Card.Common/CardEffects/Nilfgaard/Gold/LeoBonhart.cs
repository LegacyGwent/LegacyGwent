using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32009")]//雷欧·邦纳特
    public class LeoBonhart : CardEffect
    {//揭示己方1张单位牌，对1个敌军单位造成等同于被揭示单位牌基础战力的伤害。
        public LeoBonhart(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.CardInfo().CardType == CardType.Unit && !x.Status.IsReveal);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (PlayerIndex, list.ToList(), 1);
            if (result.Count() == 0) return 0;
            var point = result.Single().Status.Strength;
            await result.Single().Effect.Reveal(Card);
            result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Damage(point, Card);
            return 0;
        }
    }
}