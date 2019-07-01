using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63015")]//史凯裘
	public class Skjall : CardEffect
	{//从牌组随机打出1张铜色/银色“诅咒生物”单位牌。
		public Skjall(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}