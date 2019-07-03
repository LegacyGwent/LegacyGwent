using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62012")]//希姆
	public class Hym : CardEffect
	{//择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
		public Hym(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}