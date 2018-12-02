using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13012")]//米尔加塔布雷克
	public class Myrgtabrakke : CardEffect
	{//造成2点伤害，再重复2次。
		public Myrgtabrakke(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}