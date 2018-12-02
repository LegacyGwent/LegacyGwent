using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("15013")]//晴空
	public class ClearSkies : CardEffect
	{//使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄。
		public ClearSkies(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}