using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12039")]//乌马的诅咒
	public class UmaSCurese : CardEffect
	{//不限阵营地创造1个非领袖金色单位。
		public UmaSCurese(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}