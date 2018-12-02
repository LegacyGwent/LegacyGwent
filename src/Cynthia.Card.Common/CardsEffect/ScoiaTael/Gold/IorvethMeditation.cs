using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("52012")]//伊欧菲斯：冥想
	public class IorvethMeditation : CardEffect
	{//迫使2个同排的敌军单位互相对决。
		public IorvethMeditation(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}