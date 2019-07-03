using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42008")]//迪杰斯特拉
	public class SigismundDijkstra : CardEffect
	{//间谍。 从牌组随机打出2张牌。
		public SigismundDijkstra(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}