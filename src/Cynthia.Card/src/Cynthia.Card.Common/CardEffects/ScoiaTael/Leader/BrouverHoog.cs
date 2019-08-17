using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("51004")]//布罗瓦尔·霍格
	public class BrouverHoog : CardEffect
	{//从牌组打出1张非间谍银色单位牌或铜色“矮人”牌。
		public BrouverHoog(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => NotSliverSpy(x) || IsCopperDwarf(x));
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, cards.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (!result.Any()) return 0;
            await result.Single().MoveToCardStayFirst();
            return 1;
        }

        private bool IsCopperDwarf(GameCard gameCard)
        {
            return gameCard.Status.Group == Group.Copper && gameCard.CardInfo().Categories.Contains((Categorie.Dwarf));
        }

        private bool NotSliverSpy(GameCard gameCard)
        {
            return gameCard.Status.Group == Group.Silver && gameCard.CardInfo().CardType == CardType.Unit &&
                   gameCard.CardInfo().CardUseInfo == CardUseInfo.MyRow;
        }
    }
}