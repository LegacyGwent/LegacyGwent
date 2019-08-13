using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42009")]//夏妮
    public class Shani : CardEffect
    {//复活1个铜色/银色非“诅咒生物”单位，并使其获得2点护甲。
        public Shani(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //从我方墓地列出铜色/银色非诅咒生物
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Unit && !x.HasAnyCategorie(Categorie.Cursed)).Mess(Game.RNG);
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
            await result.First().Effect.Armor(2, Card);
            await result.First().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
        }
    }
}