using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54028")]//贤者
	public class Sage : CardEffect
	{//复活1张铜色“炼金”或“法术”牌，随后将其放逐。
		public Sage(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}