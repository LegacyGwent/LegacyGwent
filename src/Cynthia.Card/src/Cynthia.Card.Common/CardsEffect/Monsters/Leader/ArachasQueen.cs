using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("21002")]//蟹蜘蛛女王
	public class ArachasQueen : CardEffect
	{//吞噬3个友军单位，获得其战力作为增益。 免疫。
		public ArachasQueen(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var result = await Game.GetSelectPlaceCards(Card, 3 , selectMode: SelectModeType.MyRow);
			foreach(var card in result)
			{
			await Card.Effect.Drain(card.ToHealth().health, card);
			}
			return 0;
		}
	}
}
