using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44019")]//攻城大师
	public class SiegeMaster : CardEffect
	{//治愈一个铜色/银色友军“机械”单位，并再次触发其能力。 操控。
		public SiegeMaster(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}