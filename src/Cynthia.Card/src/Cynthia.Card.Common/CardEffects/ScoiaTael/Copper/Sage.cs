using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54028")] //贤者
    public class Sage : CardEffect
    {
        //复活1张铜色“炼金”或“法术”牌，随后将其放逐。
        public Sage(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersCemetery[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper &&
                            (x.CardInfo().Categories.Contains(Categorie.Spell) ||
                             x.CardInfo().Categories.Contains(Categorie.Alchemy))).Mess();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (!result.Any()) return 0;
            var card = result.Single();
            card.Status.IsDoomed = true;
            await card.Effect
                .Resurrect(new CardLocation() {RowPosition = RowPosition.MyStay, CardIndex = 0}, Card);
            return 1;
        }
    }
}