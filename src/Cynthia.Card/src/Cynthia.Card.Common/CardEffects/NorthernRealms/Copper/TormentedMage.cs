using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44031")]//受折磨的法师
	public class TormentedMage : CardEffect
	{//检视牌组中2张铜色“法术”/“道具”牌，打出1张。
		public TormentedMage(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}