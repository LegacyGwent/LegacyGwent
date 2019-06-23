using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24027")]//猫人
	public class Werecat : CardEffect
	{//对1个敌军单位造成5点伤害，随后对位于“血月”之下的所有敌军单位造成1点伤害。
		public Werecat(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}