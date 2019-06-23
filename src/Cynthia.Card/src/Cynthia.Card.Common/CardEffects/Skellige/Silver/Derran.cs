using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63014")]//迪兰
	public class Derran : CardEffect
	{//每当1个敌军单位受到伤害，便获得1点增益。
		public Derran(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}