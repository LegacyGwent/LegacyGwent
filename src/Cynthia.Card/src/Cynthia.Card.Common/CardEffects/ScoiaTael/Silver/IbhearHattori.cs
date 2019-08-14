using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53015")] //哈托利
    public class IbhearHattori : CardEffect
    {
        //复活1个战力不高于自身的铜色/银色“松鼠党”单位。
        public IbhearHattori(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersCemetery[PlayerIndex].FilterCards(type: CardType.Unit, filter:
                x => x.IsAnyGroup(Group.Copper, Group.Silver) && x.Status.Faction == Faction.ScoiaTael &&
                     (x.CardPoint()) <= (Card.CardPoint()));

            if (!(await Game.GetSelectMenuCards(PlayerIndex, cards.ToList())).TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Resurrect(new CardLocation(RowPosition.MyStay, 0), Card);
            return 1;
        }
    }
}