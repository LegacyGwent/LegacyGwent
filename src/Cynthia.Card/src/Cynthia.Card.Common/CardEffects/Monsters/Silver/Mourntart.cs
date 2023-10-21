using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23017")]//莫恩塔特
    public class Mourntart : CardEffect
    {//吞噬己方墓场所有铜色/银色单位。每吞噬1个单位，便获得1点增益。
        public Mourntart(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cardlist = Game.PlayersCemetery[PlayerIndex].FilterCards(type: CardType.Unit,filter: x => x.IsAnyGroup(Group.Copper, Group.Silver)).ToList();
            var count = cardlist.Count();
            foreach (var target in cardlist)
            {
                await target.Effect.Banish();
            }
            await Boost(count, Card);
            await Game.SendEvent(new AfterCardConsume(null, Card));
            return 0;
        }
    }
}
