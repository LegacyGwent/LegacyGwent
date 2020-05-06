using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70023")]//齐齐摩战士
    public class KikimoreWarrior : CardEffect
    {
        //吞噬己方牌组中1个战力不大于自身的非同名铜色单位牌，获得等同于其基础战力的增益
        public KikimoreWarrior(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!(await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersDeck[PlayerIndex]
            .Where(x => x.Is(Group.Copper, CardType.Unit) && x.CardInfo().CardId != Card.CardInfo().CardId && x.CardPoint() <= (Card.CardPoint()))
            .ToList())).TrySingle(out var target))
            {
                return 0;
            }
            await Card.Effect.Consume(target, x => x.Status.Strength);
            return 0;
        }
    }
}
