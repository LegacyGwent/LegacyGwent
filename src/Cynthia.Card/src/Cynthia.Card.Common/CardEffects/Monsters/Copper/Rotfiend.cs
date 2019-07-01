using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24011")]//腐食魔
	public class Rotfiend : CardEffect
	{//遗愿：对对方同排所有单位造成2点伤害。
		public Rotfiend(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}