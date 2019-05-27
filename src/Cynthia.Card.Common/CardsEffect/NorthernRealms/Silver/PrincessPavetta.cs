using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43015")]//帕薇塔公主
	public class PrincessPavetta : CardEffect
	{//将双方最弱的铜色/银色单位放回各自牌组。
		public PrincessPavetta(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}