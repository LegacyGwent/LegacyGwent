using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53015")]//哈托利
	public class IbhearHattori : CardEffect
	{//复活1个战力不高于自身的铜色/银色“松鼠党”单位。
		public IbhearHattori(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}