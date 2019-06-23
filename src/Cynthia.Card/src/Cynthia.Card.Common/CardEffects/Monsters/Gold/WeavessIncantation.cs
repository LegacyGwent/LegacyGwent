using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22012")]//织婆：咒文
	public class WeavessIncantation : CardEffect
	{//择一：使位于手牌、牌组和己方半场除自身外的所有“残物”单位获得2点强化；或从牌组打出1张铜色/银色“残物”牌，并使其获得2点强化。
		public WeavessIncantation(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}