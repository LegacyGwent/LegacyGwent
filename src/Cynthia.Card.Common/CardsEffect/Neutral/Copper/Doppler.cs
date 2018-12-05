using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14022")]//变形怪
	public class Doppler : CardEffect
	{//随机生成 1 张己方阵营中的铜色单位牌。
		public Doppler(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var id = GwentMap.GetCards().Where(x=>x.Faction == Game.PlayersFaction[Card.PlayerIndex])
				.Where(x=>x.Group==Group.Copper)
				.Mess().First().CardId;
			await Game.CreatCard(id,Card.PlayerIndex,new CardLocation(RowPosition.MyStay,0));
			return 1;
		}
	}
}