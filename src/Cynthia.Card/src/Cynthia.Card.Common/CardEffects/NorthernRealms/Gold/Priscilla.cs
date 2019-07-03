using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42011")]//普西拉
	public class Priscilla : CardEffect
	{//随机使5个友军单位获得3点增益。
		public Priscilla(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}