using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24006")]//守桥巨魔
	public class BridgeTroll : CardEffect
	{//将对方半场上的1个灾厄效果移至另一排。
		public BridgeTroll(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}