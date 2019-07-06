using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12022")]//雷吉斯：高等吸血鬼
    public class RegisHigherVampire : CardEffect
    {//检视对方牌组3张铜色单位牌。选择1张吞噬，获得等同于其基础战力的增益。
        public RegisHigherVampire(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!(await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersDeck[AnotherPlayer].Where(x => x.Is(Group.Copper, CardType.Unit)).Take(3).ToList())).TrySingle(out var target))
            {
                return 0;
            }
            await Card.Effect.Consume(target, x => x.Status.Strength);
            return 0;
        }
    }
}