using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13009")]//赛浦利安·威利
	public class CyprianWiley : CardEffect
	{//对1个单位造成4点削弱。
		public CyprianWiley(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Weaken(4,Card);
			return 0;
		}
	}
}