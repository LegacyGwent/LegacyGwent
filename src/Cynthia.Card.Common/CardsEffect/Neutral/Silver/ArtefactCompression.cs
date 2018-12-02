using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13025")]//化器封形
	public class ArtefactCompression : CardEffect
	{//将1个铜色/银色单位变为“翡翠人偶”。
		public ArtefactCompression(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}