using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12019")]//希里
	public class Ciri : CardEffect
	{//己方输掉小局时返回手牌。 2点护甲。
		public Ciri(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}