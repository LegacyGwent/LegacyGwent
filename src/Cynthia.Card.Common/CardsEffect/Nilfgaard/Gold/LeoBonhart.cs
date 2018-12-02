using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("32009")]//雷欧·邦纳特
	public class LeoBonhart : CardEffect
	{//揭示己方1张单位牌，对1个敌军单位造成等同于被揭示单位牌基础战力的伤害。
		public LeoBonhart(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}