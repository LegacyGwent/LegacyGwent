using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32012")]//叶奈法：死灵法师
    public class YenneferNecromancer : CardEffect
    {//从对方墓场中复活1张铜色/银色“士兵”牌
        public YenneferNecromancer(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //从敌方墓地取铜色单位卡
            var list = Game.PlayersCemetery[AnotherPlayer]
            .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.Status.Categories.Contains(Categorie.Soldier)).Mess(Game.RNG);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.Single().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.EnemyStay, CardIndex = 0 }, Card);
            return 1;
        }
    }
}