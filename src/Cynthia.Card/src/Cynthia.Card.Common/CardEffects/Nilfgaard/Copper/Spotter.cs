using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34021")]//侦察员
    public class Spotter : CardEffect
    {//获得等同于1张被揭示铜色/银色单位牌基础战力的增益。
        public Spotter(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards
                (Card, filter: x => x.Status.IsReveal && (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver), selectMode: SelectModeType.AllHand);
            if (result.Count() == 0) return 0;
            var point = result.Single().Status.Strength;
            await Card.Effect.Boost((point +1)/2, Card);
            return 0;
        }
    }
}