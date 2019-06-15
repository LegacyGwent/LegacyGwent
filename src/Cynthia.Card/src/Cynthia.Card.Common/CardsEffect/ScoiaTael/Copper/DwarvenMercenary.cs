using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54005")]//矮人佣兵
	public class DwarvenMercenary : CardEffect
	{//将1个单位移至它所在战场的同排。若为友军单位，则使它获得3点增益。
		public DwarvenMercenary(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (list.Count <= 0) return 0;
            var location = Card.GetLocation();
            var card = list.First();
            await card.Effect.Move(location, Card);
            if (card.PlayerIndex == Card.PlayerIndex)
            {
                await card.Effect.Boost(3,Card);
            }
            return 0;
		}
	}
}