using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("43010")]//休伯特·雷亚克
	public class HubertRejk : CardEffect
	{//汲食牌组中所有单位的增益，作为战力。
		public HubertRejk(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}