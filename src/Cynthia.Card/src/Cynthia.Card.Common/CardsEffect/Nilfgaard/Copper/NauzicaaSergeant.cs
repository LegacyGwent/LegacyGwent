using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34017")]//那乌西卡旅中士
	public class NauzicaaSergeant : CardEffect
	{//移除所在排的灾厄效果，并使1个友军单位或1个手牌中被揭示的单位获得3点增益。
		public NauzicaaSergeant(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}