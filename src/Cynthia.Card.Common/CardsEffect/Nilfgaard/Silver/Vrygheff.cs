using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("33014")]//维尔海夫
	public class Vrygheff : CardEffect
	{//从牌组打出1张铜色“机械”牌。
		public Vrygheff(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}