using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("51003")]//菲拉凡德芮
	public class Filavandrel : CardEffect
	{//创造1张银色“特殊”牌。
		public Filavandrel(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
				return await Card.CreateAndMoveStay(
                GwentMap.GetCreateCardsId(
                    x => (x.Group == Group.Silver)&&(x.CardInfo().CardType == CardType.Special),
                    RNG
                )
                .ToList()
            );
		}
	}
}