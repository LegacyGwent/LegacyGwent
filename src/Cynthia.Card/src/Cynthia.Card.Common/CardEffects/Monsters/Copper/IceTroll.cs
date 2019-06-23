using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24034")]//冰巨魔
	public class IceTroll : CardEffect
	{//与1个敌军单位对决。若它位于“刺骨冰霜”之下，则己方伤害翻倍。
		public IceTroll(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}