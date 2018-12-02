using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("31002")]//恩希尔·恩瑞斯
	public class EmhyrVarEmreis : CardEffect
	{//打出1张手牌，随后将1个友军铜色/银色单位收回手牌。
		public EmhyrVarEmreis(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}