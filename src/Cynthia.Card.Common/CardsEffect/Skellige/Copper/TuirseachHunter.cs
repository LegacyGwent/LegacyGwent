using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64021")]//图尔赛克家族猎人
	public class TuirseachHunter : CardEffect
	{//造成5点伤害。
		public TuirseachHunter(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}