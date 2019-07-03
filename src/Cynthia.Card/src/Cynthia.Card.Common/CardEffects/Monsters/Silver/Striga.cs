using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23013")]//吸血妖鸟
	public class Striga : CardEffect
	{//对1个非“怪兽”单位造成8点伤害。
		public Striga(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}