using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12037")]//附子草
	public class Wolfsbane : CardEffect
	{//在墓场停留3个回合后，在回合结束时，对最强的敌军单位造成6点伤害，使最弱的友军单位获得6点增益。
		public Wolfsbane(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}