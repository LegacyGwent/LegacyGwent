using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("32002")]//沙斯希乌斯
	public class Xarthisius : CardEffect
	{//检视对方牌组，将其中1张牌置于底端。
		public Xarthisius(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}