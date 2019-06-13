using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12008")]//维伦特瑞坦梅斯
	public class Villentretenmerth : CardEffect
	{//3回合后的回合开始时：摧毁场上除自身外所有最强的单位。 3点护甲。
		public Villentretenmerth(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}