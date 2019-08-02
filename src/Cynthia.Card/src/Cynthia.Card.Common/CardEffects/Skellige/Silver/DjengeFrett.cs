using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card {
	[CardEffectId ("63003")] //邓戈·费特
	public class DjengeFrett : CardEffect { //对2个友军单位造成1点伤害。每影响一个单位，便获得2点强化。
		public DjengeFrett (GameCard card) : base (card) { }
		public override async Task<int> CardPlayEffect (bool isSpying, bool isReveal) {
			//选取至多两个单位
			var targets = await Game.GetSelectPlaceCards (Card, 2, x => x.CardInfo ().CardType == CardType.Unit, SelectModeType.MyRow);
			if(targets.Count()==0)return 0;
			//同时造成伤害
			foreach (var target in targets) {
				await target.Effect.Damage (1, Card);
			}
			//强化
			await Card.Effect.Strengthen (2 * targets.Count (), Card);

			return 1;

		}
	}
}