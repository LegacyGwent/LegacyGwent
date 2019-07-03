using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13028")]//诱饵
    public class Decoy : CardEffect
    {//重新打出1个铜色/银色友军单位，并使它获得3点增益。
        public Decoy(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectPlaceCards(Card, filter: (x => x.Status.Group == Group.Copper || x.Status.Group == Group.Silver), selectMode: SelectModeType.MyRow);
            if (result.Count() == 0) return 0;
            var targetCard = result.Single();
            targetCard.Effect.Repair(true);
            await targetCard.MoveToCardStayFirst();
            await targetCard.Effect.Boost(3, Card);
            return 1;
        }
    }
}