using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24012")]//叉尾龙
	public class Forktail : CardEffect
	{//吞噬2个友军单位，并获得其战力的增益。
		public Forktail(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}