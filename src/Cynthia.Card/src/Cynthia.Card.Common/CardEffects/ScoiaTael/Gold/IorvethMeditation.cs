using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52012")]//伊欧菲斯：冥想
	public class IorvethMeditation : CardEffect
	{//迫使2个同排的敌军单位互相对决。
		public IorvethMeditation(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}