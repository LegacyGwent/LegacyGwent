using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23022")]//寄生虫
	public class Parasite : CardEffect
	{//对1个敌军单位造成12点伤害；或使1个友军单位获得12点增益。
		public Parasite(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}