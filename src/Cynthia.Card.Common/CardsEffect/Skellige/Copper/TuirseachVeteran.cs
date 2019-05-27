using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64016")]//图尔赛克家族老兵
	public class TuirseachVeteran : CardEffect
	{//使位于手牌、牌组和己方半场除自身外的所有“图尔赛克家族”单位获得1点强化。
		public TuirseachVeteran(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}