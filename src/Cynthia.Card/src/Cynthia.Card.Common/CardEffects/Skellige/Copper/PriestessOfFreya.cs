using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64032")]//弗蕾雅女祭司
    public class PriestessOfFreya : CardEffect
    {//复活1个铜色“士兵”单位。
        public PriestessOfFreya(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //从我方墓地列出铜色“士兵单位”
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit && x.HasAllCategorie(Categorie.Soldier)).Mess();
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
    }
}