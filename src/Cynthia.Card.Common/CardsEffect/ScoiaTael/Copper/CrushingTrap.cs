using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54032")]//碎骨陷阱
	public class CrushingTrap : CardEffect
	{//使对方单排左右两侧末端的单位各受到6点伤害。
		public CrushingTrap(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}