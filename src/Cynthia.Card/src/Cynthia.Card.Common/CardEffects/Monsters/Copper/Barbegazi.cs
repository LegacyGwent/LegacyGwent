using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24025")]//须岩怪
	public class Barbegazi : CardEffect
	{//坚韧。 吞噬1个友军单位，获得其战力作为增益。
		public Barbegazi(GameCard card) : base(card){ }

		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            await Card.Effect.Resilience(Card);
            
            var card = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow)).SingleOrDefault();
            if (card != default)
            {
                await Card.Effect.Consume(card);
            }
			return 0;
		}
	}
}