using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12039")]//乌马的诅咒
	public class UmaSCurese : CardEffect
	{//不限阵营地创造1个非领袖金色单位。
		public UmaSCurese(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var list = GwentMap.GetCards()
				.Where(x=>x.CardId.CardInfo().CardType==CardType.Unit
						&&(x.Group == Group.Gold))
				.Mess().Take(3).Select(x=>x.CardId)
				.ToList();
			return await Card.CreateAndMoveStay(list);
		}
	}
}