using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64013")]//暴怒的狂战士
	public class RagingBerserker : CardEffect
	{//受伤或被削弱时变为“狂暴的熊”。
		public RagingBerserker(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}