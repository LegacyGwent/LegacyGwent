using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24001")]//独眼巨人
	public class Cyclops : CardEffect
	{//摧毁1个友军单位，对1个敌军单位造成等同于被摧毁友军单位战力的伤害。
		public Cyclops(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}