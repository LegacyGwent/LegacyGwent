using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("55001")]//焚烧陷阱
	public class IncineratingTrap : CardEffect
	{//对同排除自身外所有单位造成2点伤害，并在回合结束时放逐自身。
		public IncineratingTrap(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}