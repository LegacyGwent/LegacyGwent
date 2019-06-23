using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63006")]//斯凡瑞吉·图尔赛克
	public class SvanrigeTuirseach : CardEffect
	{//抽1张牌，随后丢弃1张牌。
		public SvanrigeTuirseach(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}