using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22006")]//呢喃婆：贡品
	public class WhispessTribute : CardEffect
	{//从牌组打出1张铜色/银色“有机”牌。
		public WhispessTribute(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}