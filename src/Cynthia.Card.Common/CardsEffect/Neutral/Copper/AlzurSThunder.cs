using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14003")]//阿尔祖落雷术
	public class AlzurSThunder : CardEffect
	{//造成9点伤害。
		public AlzurSThunder(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}