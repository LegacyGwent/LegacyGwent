using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42001")]//丹德里恩
	public class Dandelion : CardEffect
	{//使牌组3个单位获得2点增益。
		public Dandelion(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}