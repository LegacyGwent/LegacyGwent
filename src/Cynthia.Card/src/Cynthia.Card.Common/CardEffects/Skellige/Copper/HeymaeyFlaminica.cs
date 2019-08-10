using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64003")]//海玫家族佛兰明妮卡
    public class HeymaeyFlaminica : CardEffect
    {//移除所在排的灾厄，并将2个友军单位移至该排。
        public HeymaeyFlaminica(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<NoneStatus>();
            var movelist = await Game.GetSelectPlaceCards(Card, 2, filter: (x => x.Status.CardRow != Card.Status.CardRow), selectMode: SelectModeType.MyRow);
            if (movelist.Count() == 0)
            {
                return 0;
            }
            foreach (var card in movelist)
            {
                var row = Card.Status.CardRow;
                var population = Game.RowToList(Card.PlayerIndex, row).Count();
                //满了的话
                await card.Effect.Move(new CardLocation() { RowPosition = row, CardIndex = population }, Card);
            }
            return 0;
        }
    }
}