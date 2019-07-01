using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23009")]//约顿
	public class Jotunn : CardEffect
	{//将3个敌军单位移至对方同排，并对它们造成2点伤害。若该排上有“刺骨冰霜”生效，则将伤害提高至3点。
		public Jotunn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}