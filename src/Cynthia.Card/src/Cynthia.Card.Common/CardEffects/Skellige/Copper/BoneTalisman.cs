using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64034")]//骨制护符
    public class BoneTalisman : CardEffect
    {//择一：复活1个铜色“野兽”或“呓语”单位；或治愈1名友军单位，并使其获得3点强化。
        public BoneTalisman(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var switchCard = await Card.GetMenuSwitch(("野兽", "复活1个铜色“野兽”或“呓语”单位"), ("再生", "治愈1名友军单位，并使其获得3点强化"));
            if (switchCard == 0)
            {
                //从我方墓地列出铜色“野兽”或“呓语”单位
                var list = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Beast, Categorie.Cultist)).Mess();
                if (list.Count() == 0)
                {
                    return 0;
                }

                //让玩家选择一张卡
                var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
                //如果玩家一张卡都没选择,没有效果
                if (result.Count() == 0)
                {
                    return 0;
                }
                //复活到玩家指定位置
                await result.First().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
                return 1;
            }

            if (switchCard == 1)

            {

                var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
                if (targets.Count() == 0)
                {
                    return 0;
                }
                //治愈和强化3
                foreach (var target in targets)
                {
                    await target.Effect.Heal(Card);
                    await target.Effect.Strengthen(3, Card);
                }

                return 0;
            }

            return 0;
        }
    }
}