using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("51003")]//菲拉凡德芮
	public class Filavandrel : CardEffect
	{//生成1张己方起始牌组之外的银色中立“特殊”牌。
		public Filavandrel(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return await Card.CreateAndMoveStay(
                GwentMap.GetGenerateCardsId(
                    x => x.Is(Group.Silver, CardType.Special) &&
                        x.Faction == Faction.Neutral,
                    Card.GetMyBaseDeck().Select(x => x.CardId))
                .ToList());
		}
	}
}
