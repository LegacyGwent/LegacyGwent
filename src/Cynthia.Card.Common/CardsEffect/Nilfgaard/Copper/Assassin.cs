using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34029")]//刺客
	public class Assassin : CardEffect
	{//间谍对左侧单位造成10点伤害。
		public Assassin(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var rowIndex = Card.GetLocation(Card.PlayerIndex).CardIndex;
			var list = Game.RowToList(Card.PlayerIndex,Card.Status.CardRow).ToList();
			var taget = Card.GetRangeCard(1,GetRangeType.HollowLeft);
			if(taget.Count>0)
			{
				await taget.Single().Effect.Damage(10,Card);
			}
			return 0;
		}
	}
}