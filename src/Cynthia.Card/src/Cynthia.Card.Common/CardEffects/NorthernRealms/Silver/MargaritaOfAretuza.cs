using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43011")]//玛格丽塔
	public class MargaritaOfAretuza : CardEffect
	{//重置1个单位，并改变它的锁定状态。
		public MargaritaOfAretuza(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}