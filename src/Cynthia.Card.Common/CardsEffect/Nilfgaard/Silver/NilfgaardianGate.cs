using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("33022")]//尼弗迦德大门
	public class NilfgaardianGate : CardEffect
	{//从牌组打出1张铜色/银色“军官”牌，并使其获得1点增益。
		public NilfgaardianGate(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}