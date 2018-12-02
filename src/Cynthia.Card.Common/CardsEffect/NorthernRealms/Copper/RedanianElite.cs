using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44006")]//瑞达尼亚精锐
	public class RedanianElite : CardEffect
	{//每当护甲归0，获得5点增益。 4点护甲。
		public RedanianElite(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}