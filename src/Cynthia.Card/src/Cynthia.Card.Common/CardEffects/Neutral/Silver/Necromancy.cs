using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13033")]//死灵术
    public class Necromancy : CardEffect
    {//从双方墓场放逐1个铜色/银色单位，其战力将成为1个友军单位的增益。
        public Necromancy(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = Game.PlayersCemetery[PlayerIndex].Concat(Game.PlayersCemetery[AnotherPlayer]).Where(x => x.IsAnyGroup(Group.Copper, Group.Silver) && x.Is(type: CardType.Unit)).ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, cards)).TrySingle(out var target))
            {
                return 0;
            }
            var point = target.CardPoint();
            await target.Effect.Banish();
            if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow)).TrySingle(out var target2))
            {
                return 0;
            }
            await target2.Effect.Boost(point, Card);
            return 0;
        }
    }
}