using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33021")]//吊死鬼之毒
	public class Cadaverine : CardEffect
	{//择一：对1个敌军单位以及所有与它同类型的单位造成2点伤害；或摧毁1个铜色/银色“中立”单位。
		public Cadaverine(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}