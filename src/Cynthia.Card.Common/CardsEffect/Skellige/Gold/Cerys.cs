using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62007")]//凯瑞丝·奎特
	public class Cerys : CardEffect
	{//位于墓场中时，在己方复活 4个单位后，复活单位。
		public Cerys(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}