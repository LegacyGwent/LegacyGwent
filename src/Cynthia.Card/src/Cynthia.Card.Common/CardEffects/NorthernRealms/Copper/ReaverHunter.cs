using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44021")]//掠夺者猎人
	public class ReaverHunter : CardEffect
	{//使手牌、牌组或己方半场所有同名牌获得1点增益。 每有1张同名牌打出，便再次触发此能力。
		public ReaverHunter(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}