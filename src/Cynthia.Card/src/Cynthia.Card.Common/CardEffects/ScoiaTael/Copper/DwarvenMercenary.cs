using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54005")] //矮人佣兵
    public class DwarvenMercenary : CardEffect
    {
        //将1个单位移至它所在战场的同排。若为友军单位，则使它获得3点增益。
        public DwarvenMercenary(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = await Game.GetSelectPlaceCards(Card, filter: NoMySelfRow, selectMode: SelectModeType.AllRow);
            if (list.Count <= 0) return 0;
            var location = Card.GetLocation() + 1;
            var card = list.First();
            await card.Effect.Move(location, Card);
            if (card.PlayerIndex == Card.PlayerIndex)
            {
                await card.Effect.Boost(3, Card);
            }

            return 0;
        }

        private bool NoMySelfRow(GameCard card)
        {
            return Card.Status.CardRow != Card.Status.CardRow;
        }
    }
}