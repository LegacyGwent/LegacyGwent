using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44018")]//攻城槌
	public class BatteringRam : CardEffect
	{//对1个敌军单位造成3点伤害。若摧毁目标，则对另一个敌军单位造成3点伤害。 驱动：初始伤害提高1点。
		public BatteringRam(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}