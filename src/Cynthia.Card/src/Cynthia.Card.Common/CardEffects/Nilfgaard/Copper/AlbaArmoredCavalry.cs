using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34014")]//阿尔巴师装甲骑兵
	public class AlbaArmoredCavalry : CardEffect
	{//每有1个友军单位被打出，便获得1点增益。
		public AlbaArmoredCavalry(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}