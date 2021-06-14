using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("240250")]//须岩怪：晋升
    public class BarbegaziPro : CardEffect
    {//坚韧。 吞噬1个友军单位，获得其战力作为增益。
        public BarbegaziPro(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            var card = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow)).SingleOrDefault();
            if (card != default)
            {
                await Card.Effect.Consume(card);
            }
            await Card.Effect.Resilience(Card);
            return 0;
        }
    }
}