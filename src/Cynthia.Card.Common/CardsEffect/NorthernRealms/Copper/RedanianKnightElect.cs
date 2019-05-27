using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44013")]//瑞达尼亚当选骑士
	public class RedanianKnightElect : CardEffect
	{//回合结束时，若受护甲保护，则使相邻单位获得1点增益。 2点护甲。
		public RedanianKnightElect(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}