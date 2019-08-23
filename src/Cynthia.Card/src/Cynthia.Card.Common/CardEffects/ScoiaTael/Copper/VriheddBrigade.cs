using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54004")] //维里赫德旅
    public class VriheddBrigade : CardEffect
    {
        //移除所在排的灾厄，并将1个单位移至它所在半场的同排。
        public VriheddBrigade(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<NoneStatus>();
            var list = await Game.GetSelectPlaceCards(Card, filter: NoMySelfRow, selectMode: SelectModeType.AllRow);
            if (list.Count <= 0) return 0;
            var location = Card.GetLocation() + 1;
            var card = list.First();
            await card.Effect.Move(location, Card);
            return 0;
        }

        private bool NoMySelfRow(GameCard card)
        {
            return Card.Status.CardRow != card.Status.CardRow;
        }
    }
}