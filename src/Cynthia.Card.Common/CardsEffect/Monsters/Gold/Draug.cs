using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22002")]//战灵
	public class Draug : CardEffect
	{//将死去的单位复活为战力为1的“战鬼”，直至填满此排。
		public Draug(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}