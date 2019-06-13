using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63010")]//德莱格·波·德乌
	public class DraigBonDhu : CardEffect
	{//使墓场中2个单位获得3点强化。
		public DraigBonDhu(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}