using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64021")]//图尔赛克家族猎人
	public class TuirseachHunter : CardEffect
	{//造成5点伤害。
		public TuirseachHunter(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}